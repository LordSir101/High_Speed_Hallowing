using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    public InputActionReference movement, dash;

    private bool isDashing = false;


    [Header("Wall Jump")]
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private GameObject actionWindowIndicatorPrefab;
    private float wallJumpSpeedModifier = 1.5f;
    private Collider2D touchingWall;
    public bool wallJumpQueued = false;
    private Vector2 wallJumpDir;
    private float wallJumpTime = 0.3f;

    Rigidbody2D rb;
    PlayerImpact playerImpact;

    public float dashSpeed {get; }= 3f;
    private float dashTime = 0.3f;
    

    public bool CanMove {get; set;} = true;
    Vector2 movementInput;
    public float baseMoveSpeed = 3;
    public float currSpeed;
    public float linearDrag = 10f;
    private float speedLossTimer = 0;
    private float speedLossWindow = 0.002f;


    public Vector2 prevFrameVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerImpact= GetComponent<PlayerImpact> ();
        currSpeed = baseMoveSpeed;
        // StartCoroutine(CalculateDrag());
    }

    void Update()
    {
        movementInput = movement.action.ReadValue<Vector2>();

        // calculate drag for curr speed using linear approximation, same as Unity does normally.
        // currSpeed = currSpeed * ( 1 - Time.deltaTime * linearDrag);
        // Debug.Log(currSpeed);

        // if(currSpeed < 0.5)
        // {
        //     currSpeed = 0;
        // }
        speedLossTimer += Time.deltaTime;
        
        if(speedLossTimer >= speedLossWindow)
        {
            currSpeed = currSpeed * ( 1 - Time.deltaTime * linearDrag);

            if(currSpeed < 0.5)
            {
                currSpeed = 0;
            }
            speedLossTimer = 0;
        }
    }

    // IEnumerator CalculateDrag()
    // {
    //     while(true)
    //     {
    //         currSpeed = currSpeed * ( 1 - Time.deltaTime * linearDrag);
    //         Debug.Log(currSpeed);

    //         if(currSpeed < 0.5)
    //         {
    //             currSpeed = 0;
    //         }

    //         yield return new WaitForSeconds(0.1f);
    //     }
    // }

    // public void ResetSpeedLossWindow()
    // {
    //     speedLossTimer = 0;
    // }

    void FixedUpdate()
    {
        /*
        curr speed is how fast the player moves with the direction input. Dash speed is based off of this.
        curr speed is tracked seperately, so that it can be increased at a controled rate instead of basing the increases off of current velocity.
        if a player hits a wall, they can still move at currSpeed, instead of velocity being set to 0.
        */

        if(CanMove) 
        {

            if(movementInput != Vector2.zero)
            {
                // move using current speed, with a minimum of base move speed
                rb.velocity = currSpeed > baseMoveSpeed ? movementInput * currSpeed : movementInput * baseMoveSpeed ;

                // float playerRadValue = Mathf.Atan2(movementInput.y, movementInput.x);
                // float playerAngle= playerRadValue * (180/Mathf.PI);
                // rb.transform.localRotation = Quaternion.Euler(0,0,playerAngle -90);
            }
            else
            {
                //rb.velocity = currSpeed * rb.velocity.normalized;
                rb.velocity = Vector2.zero;
            }

            // float playerRadValue = Mathf.Atan2(rb.velocity.y, rb.velocity.x);
            // float playerAngle= playerRadValue * (180/Mathf.PI);
            // rb.transform.localRotation = Quaternion.Euler(0,0,playerAngle -90);
            
         }

    }

    void LateUpdate()
    {
        prevFrameVelocity = rb.velocity;
    }

    private void OnEnable()
    {
        dash.action.performed += Dash;
        
    }

    // Allows walljump while grappling but not normal dashes
    public void DisableBasicDash()
    {
        dash.action.performed -= Dash;
        dash.action.performed += ConditionalDash;
    }

    public void EnableBasicDash()
    {
        dash.action.performed += Dash;
        dash.action.performed -= ConditionalDash;
    }

    // In certain circumstances like grappling, we can wall jump but not normal dash
    private void ConditionalDash(InputAction.CallbackContext context)
    {
        QueueWallJump();
    }

    private void QueueWallJump()
    {
        touchingWall = Physics2D.OverlapCircle(wallCheck.position, 0.6f, wallLayer);
        
        Vector2 direction = movement.action.ReadValue<Vector2>();

        if(touchingWall && direction != Vector2.zero)
        {
            wallJumpDir = direction;
            wallJumpQueued = true;

            StartCoroutine(TurnOffWallJump());
            
        }
    }

    private void Dash(InputAction.CallbackContext context)
    {
        QueueWallJump();

        if(!wallJumpQueued)
        {
            CanMove = false;
            DisableBasicDash();
            
            Vector2 direction = movement.action.ReadValue<Vector2>();
            StartCoroutine(PerformDash(direction * (dashSpeed + rb.velocity.magnitude), dashTime));
        }
    }

    // if they player does not hit a wall in time, dont wall jump
    IEnumerator TurnOffWallJump()
    {
        yield return new WaitForSeconds(0.5f);
        wallJumpQueued = false;
    }

    public void StartWallJump()
    {
        WallJump(wallJumpDir);
    }
    private void WallJump(Vector2 direction)
    {
        BoxCollider2D wall = touchingWall.gameObject.GetComponent<BoxCollider2D>();
            
        Vector3 wallSize = wall.bounds.size;
        Vector3 wallPos = touchingWall.transform.position;
        Vector2 dir = Vector2.zero;
        
        CancelOtherMovement();

        // if the player walljumps within the action window, the player's speed increases
        if(playerImpact.actionWindowIsActive)
        {
            GameObject animation = Instantiate(actionWindowIndicatorPrefab, transform.position, transform.rotation);
            animation.transform.SetParent(transform, false);
            currSpeed += PlayerImpact.IMPACTSPEEDINCREASE;
        }

        float wallJumpSpeed = dashSpeed + currSpeed;

        if(wallPos.x + wallSize.x/2 < transform.position.x)
        {
            dir = new Vector2(wallJumpSpeed, direction.y * wallJumpSpeed);
        }
        // wall is vertical to the right
        else if(wallPos.x - wallSize.x/2 > transform.position.x)
        {
            dir = new Vector2(-wallJumpSpeed, direction.y * wallJumpSpeed);
        }
        // wall on botom
        else if(wallPos.y + wallSize.y/2 < transform.position.y)
        {
            dir = new Vector2(direction.x * wallJumpSpeed, wallJumpSpeed);
        }
        // wall on top
        else if(wallPos.y - wallSize.y/2 > transform.position.y)
        {

            dir = new Vector2(direction.x * wallJumpSpeed, -wallJumpSpeed);
        }

        StartCoroutine(PerformDash(dir, wallJumpTime));
    }

    private void CancelOtherMovement()
    {
        gameObject.GetComponent<PlayerGrapple>().EndGrapple();
        gameObject.GetComponent<PlayerReflectDash>().EndReflectDash();
        CanMove = false;
        DisableBasicDash();

        EndDash();
    }
    public void EndDash()
    {
        if(isDashing)
        {
            StopAllCoroutines();
        }
    }

    private void ResetDashStatus()
    {
        CanMove = true;
        EnableBasicDash();
        dash.action.Enable();
        isDashing = false;
    }
    IEnumerator PerformDash(Vector2 force, float time)
    {
        isDashing = true;

        rb.AddForce(force, ForceMode2D.Impulse);

        // rotate player
        // float playerRadValue = Mathf.Atan2(rb.velocity.y, rb.velocity.x);
        // float playerAngle= playerRadValue * (180/Mathf.PI);
        // rb.transform.localRotation = Quaternion.Euler(0,0,playerAngle -90);

        yield return new WaitForSeconds(time);

        ResetDashStatus();
    }

}
