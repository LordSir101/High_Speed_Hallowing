using System;
using System.Collections;
//using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGrapple : MonoBehaviour
{
    [SerializeField]
    private InputActionReference grapple, pointerPos, controllerGrappleAim;//, movement;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] LineRenderer grappleGuide;

    private LineRenderer lineRenderer;
    Rigidbody2D rb;

    PlayerMovement playerMovement;
    private PlayerAnimation playerAnimation;

    private float maxGrappleDistance = 8;
    private float initialGrappleSpeed = 1.5f;
    private float maxGrappleAccel = 8f;
    private float grappleAcceleration = 1.5f;
    private float grappleShotAnimationTime = 0.2f;
    public bool grappling = false;
    public float initialDrag {get; set;}
    public bool canGrapple {get;set;} = true;
    private bool holdingGrapple = false;

    private PlayerAudio playerAudio;
    private PlayerCooldowns playerCooldowns;
    //private float grappleCooldown = 3.5f;

    
    private Vector2 grappleLocation;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        initialDrag = rb.drag;
        lineRenderer = GetComponent<LineRenderer>();
        playerMovement = GetComponent<PlayerMovement>();
        playerAnimation = GetComponent<PlayerAnimation>();
        playerAudio = GetComponentInChildren<PlayerAudio>();
        playerCooldowns = GetComponent<PlayerCooldowns>();
    }

    private void OnEnable()
    {
        grapple.action.performed += StartGrapple;
    }

    private void OnDisable()
    {
        grapple.action.performed -= StartGrapple;
        
    }

    private void StartGrapple(InputAction.CallbackContext context)
    {
        if(canGrapple)
        {

            canGrapple = false;
            
            grapple.action.canceled += GrappleSetUp;
            StartCoroutine(ShowGrappleGuide());

            // grapple.action.Disable();
            // grappling = true;
            // rb.drag = 0;

            // // project settings -> physics2D -> quries start in colliders unchecked so raycast does not detect origin
            // RaycastHit2D hitTarget = Physics2D.Raycast(gameObject.transform.position, grappleDirection, distance: maxGrappleDistance + 0.2f);

            // playerCooldowns.StartGrappleCooldown();

            // if(hitTarget)
            // {
            //     grappleLocation = hitTarget.point;
            //     StartCoroutine(PerformGrapple(hitTarget.collider.gameObject));
            // }
            // else 
            // {
            //     grappleLocation = rb.position + grappleDirection.normalized * maxGrappleDistance;
            //     StartCoroutine(PerformMissedGrapple());
            // }

            //GetComponent<PlayerCooldowns>().StartCooldown(grappleCooldown, EndGrappleCooldown);

        }
        
    }

    IEnumerator ShowGrappleGuide()
    {
        holdingGrapple = true;
        grappleGuide.enabled = true;

        while(holdingGrapple)
        {
            Vector2 grappleDir = GetGrappleDirection();
            RaycastHit2D hitTarget = Physics2D.Raycast(gameObject.transform.position, grappleDir, distance: maxGrappleDistance + 0.2f);
            Vector2 endPoint;

            if(hitTarget)
            {
                // the grapple indicator will stop at enemy buffer, so manually set indicator to snap to enemy position instead
                if(hitTarget.transform.gameObject.tag == "Enemy")
                {
                    endPoint = hitTarget.transform.position;
                }
                else
                {
                    endPoint = hitTarget.point;
                }
                   
            }
            else 
            {
                endPoint = new Vector2(transform.position.x, transform.position.y) + grappleDir.normalized * maxGrappleDistance;
            }
            grappleGuide.SetPosition(0, transform.position);
            grappleGuide.SetPosition(1, endPoint);
            yield return null;
        }
        
    }

    void GrappleSetUp(InputAction.CallbackContext context)
    {
        grapple.action.canceled -= GrappleSetUp;
        holdingGrapple = false;
        grappleGuide.enabled = false;

        Vector2 grappleDirection = GetGrappleDirection();
        grapple.action.Disable();
        grappling = true;
        rb.drag = 0;

        // project settings -> physics2D -> quries start in colliders unchecked so raycast does not detect origin
        RaycastHit2D hitTarget = Physics2D.Raycast(gameObject.transform.position, grappleDirection, distance: maxGrappleDistance + 0.2f);

        playerCooldowns.StartGrappleCooldown();

        if(hitTarget)
        {
            grappleLocation = hitTarget.point;
            StartCoroutine(PerformGrapple(hitTarget.collider.gameObject));
        }
        else 
        {
            grappleLocation = rb.position + grappleDirection.normalized * maxGrappleDistance;
            StartCoroutine(PerformMissedGrapple());
        }

    }

    void Update()
    {
        // stop grappling if we are close to target destination
        // this stops bug where player gets stuck in grapple animation bouncing back and forth
        if(((grappleLocation - rb.position).magnitude < 0.2f) && grappling)
        {
            EndGrapple();
        }
    }

    // void EndGrappleCooldown()
    // {
    //     Debug.Log("grapple cooldown");
    //     canGrapple = true;
    // }

    public void EndGrapple()
    {
        if(grappling)
        {
            grappling = false;
            StopAllCoroutines();
            ResetGrapppleStatus();
        }
       
    }

    private Vector2 GetGrappleDirection()
    {
        Vector2 mousePos = pointerPos.action.ReadValue<Vector2>();

        // controller will give us the direction the left stick is pointing
        if(playerInput.currentControlScheme == "Controller")
        {
            Debug.Log(controllerGrappleAim.action.ReadValue<Vector2>());
            if(controllerGrappleAim.action.ReadValue<Vector2>() != Vector2.zero)
            {
                return controllerGrappleAim.action.ReadValue<Vector2>();
            }
            return mousePos;
        }

        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        Vector2 direction = mousePos - rb.position;

        return direction.normalized;
    }

    IEnumerator PerformGrapple(GameObject target)
    {
        //Debug.Log("grapple");
        yield return StartCoroutine(AnimateGrappleShot(target));

        CancelOtherMovment();
        
       
        rb.velocity = (grappleLocation - rb.position).normalized * (initialGrappleSpeed + rb.velocity.magnitude);
        float initalSpeed = rb.velocity.magnitude;
        float maxGrappleSpeed = initalSpeed + maxGrappleAccel;

        StartCoroutine(AnimateGrapple());

        playerAudio.PlayGrappleSound();

        while (grappling)
        {

            // prevents player from flying off the screen if they grapple past thier target
            if(target != null)
            {
                if(target.gameObject.tag == "Enemy" || target.gameObject.tag == "Buffer")
                {
                    grappleLocation = target.transform.position;
                }

            }

            // Temp fix for bug where movement does not get disabled if a move input is pressed while being disabled
            if(playerMovement.CanMove)
            {
                playerMovement.CanMove = false;
                rb.velocity = (grappleLocation - rb.position).normalized * (initialGrappleSpeed + rb.velocity.magnitude);
            }

            float speed = Mathf.Clamp(rb.velocity.magnitude * grappleAcceleration, initialGrappleSpeed, maxGrappleSpeed);
            rb.velocity =  speed * (grappleLocation - rb.position).normalized;

            float diff = (grappleLocation - rb.position).magnitude;
            if(diff < 1f)
            {
                EndGrapple();
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private void CancelOtherMovment()
    {
        gameObject.GetComponent<PlayerMovement>().EndDash();
        gameObject.GetComponent<PlayerReflectDash>().EndReflectDash();
        playerMovement.CanMove = false;
        playerMovement.DisableBasicDash();
    }

    
    // To show where the grapple hook went
    IEnumerator PerformMissedGrapple()
    {
        yield return StartCoroutine(AnimateGrappleShot());

        EndGrapple();
        ResetGrapppleStatus();
    }

    public void ResetGrapppleStatus()
    {
        grapple.action.Enable();
        playerMovement.EnableBasicDash();
        lineRenderer.enabled = false;
        playerMovement.CanMove = true;
        rb.drag = initialDrag;
        rb.velocity = rb.velocity.normalized * 0.7f * rb.velocity.magnitude;
    }

    
    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     if(other.gameObject.tag == "Enemy" || other.gameObject.tag == "GrappleBuffer")
    //     {
    //         if(grappling)
    //         {
    //             EndGrapple();
    //         }
    //     }
    // }

    // end grapple when a collision occurs
    // collisions with enemy handled in player impact so damage can be assigned before the player velocity is set    
    // private void OnCollisionEnter2D(Collision2D col)
    // {
    //     if(grappling)
    //     {
    //         EndGrapple();
    //     }
        
    // }

    IEnumerator AnimateGrapple()
    {
        
        while(grappling)
        {
            // animate the grapple hook while actively grappling
            Vector3 start = new Vector3(gameObject.transform.position.x,gameObject.transform.position.y,0);

            //lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, grappleLocation);

            yield return null;

        }
    }

    IEnumerator AnimateGrappleShot(GameObject target = null)
    {
        playerAudio.PlayGrappleShootSound();
        float distance = (grappleLocation - rb.position).magnitude;

        // Vector2 normalized = (grappleLocation - rb.position).normalized;
        // float playerRadValue = Mathf.Atan2(normalized.y, normalized.x);
        // float playerAngle= playerRadValue * (180/Mathf.PI);
        // rb.transform.localRotation = Quaternion.Euler(0,0,playerAngle -90);


        if(distance > 5.5 && target != null)
        {
            playerAnimation.GrappleSpin();
        }
        else if(distance > 3 && target != null)
        {
            playerAnimation.GrappleAnimation();
        }
        
        lineRenderer.enabled = true;
        float grappleAnimationTimer = 0;
        float interpolationRatio;

        do
        {
            Vector3 start = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y,0);

            interpolationRatio = grappleAnimationTimer / grappleShotAnimationTime;

            Vector3 interpolatedPos = Vector3.Lerp(start, grappleLocation, interpolationRatio);

            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, interpolatedPos);

            grappleAnimationTimer += Time.deltaTime;
            yield return null; // wait for the end of frame
        }
        while (interpolationRatio < 1);
        
    }
}
