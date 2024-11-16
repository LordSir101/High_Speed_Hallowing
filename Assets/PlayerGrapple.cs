using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGrapple : MonoBehaviour
{
    [SerializeField]
    private InputActionReference grapple, pointerPos;//, movement;
    private LineRenderer lineRenderer;
    Rigidbody2D rb;

    PlayerMovement playerMovement;

    private float maxGrappleDistance = 6;
    private float initialGrappleSpeed = 1.5f;
    private float maxGrappleAccel = 8f;
    private float grappleAcceleration = 1.5f;
    private float grappleShotAnimationTime = 0.3f;
    private bool grappling = false;

    
    private Vector2 grappleLocation;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();
        playerMovement = GetComponent<PlayerMovement>();
    }

     private void OnEnable()
    {
        grapple.action.performed += StartGrapple;
    }

    private void StartGrapple(InputAction.CallbackContext context)
    {
        Vector2 grappleDirection = GetGrappleDirection();

        grapple.action.Disable();
        grappling = true;
        rb.drag = 0;

        // project settings -> physics2D -> quries start in colliders unchecked so raycast does not detect origin
        RaycastHit2D hitTarget = Physics2D.Raycast(gameObject.transform.position, grappleDirection, distance: maxGrappleDistance);

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
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        Vector2 direction = mousePos - rb.position;

        return direction.normalized;
    }

    IEnumerator PerformGrapple(GameObject target)
    {
        //Debug.Log("grapple");
        yield return StartCoroutine(AnimateGrappleShot());

        CancelOtherMovment();
        
       
        rb.velocity = (grappleLocation - rb.position).normalized * (initialGrappleSpeed + rb.velocity.magnitude);
        float initalSpeed = rb.velocity.magnitude;
        float maxGrappleSpeed = initalSpeed + maxGrappleAccel;

        StartCoroutine(AnimateGrapple());

        while (grappling)
        {

            // prevents player from flying off the screen if they grapple past thier target
            if(target.gameObject.tag == "Enemy")
            {
                Debug.Log("targeting enemy");
                grappleLocation = target.transform.position;
            }

            // Temp fix for bug where movement does not get disabled if a move input is pressed while being disabled
            if(playerMovement.CanMove)
            {
                playerMovement.CanMove = false;
                rb.velocity = (grappleLocation - rb.position).normalized * (initialGrappleSpeed + playerMovement.currSpeed);
            }

            float speed = Mathf.Clamp(rb.velocity.magnitude * grappleAcceleration, initialGrappleSpeed, maxGrappleSpeed);
            rb.velocity =  speed * (grappleLocation - rb.position).normalized;

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
    }

    // end grapple when a collision occurs
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            if(grappling)
            {
                EndGrapple();
            }
        }
    }

    
    private void OnCollisionEnter2D(Collision2D col)
    {
        if(grappling)
        {
            EndGrapple();
        }
        
    }

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

    IEnumerator AnimateGrappleShot()
    {
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
