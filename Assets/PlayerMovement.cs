using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;



public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    public InputActionReference movement, dash;

    public AnimationCurve animCurve;
    private PlayerAudio playerAudio;
    private PlayerCooldowns playerCooldowns;

    private bool isDashing = false;
    public bool canDash {get;set;}= true;

    [SerializeField] PauseControl pauseControl;
    


    [Header("Wall Jump")]
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private GameObject actionWindowIndicatorPrefab;
    [SerializeField] private float wallJumpForce = 13f;
    [SerializeField] private float wallJumpTime = 0.2f, wallJumpPause = 0.01f;
    [SerializeField] private GameObject wallJumpEffect;
    private int wallJumpCombo = 0;
    
    private Collider2D touchingWall;
    public bool wallJumpQueued = false;
    //private Vector2 wallJumpDir;

    [Header("Dash")]
    [SerializeField] public float dashSpeed {get; set;}= 14f;
    [SerializeField] private float dashTime = 0.3f, dashPause = 0.05f;
    

    Rigidbody2D rb;
    PlayerImpact playerImpact;

    
    //private float dashCooldown = 3;
    
    [Header("Movement")]
    [SerializeField] public float baseMoveSpeed;
    public float initialBaseMoveSpeed{get; set;}
    [SerializeField] float accel, deaccel, velPower;

    public bool CanMove {get; set;} = true;
    Vector2 movementInput;
    private Vector2 prevDashVelocity;

    private IEnumerator currAction;


    public Vector2 prevFrameVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerImpact= GetComponent<PlayerImpact> ();
        playerAudio = GetComponentInChildren<PlayerAudio>();
        playerCooldowns = GetComponent<PlayerCooldowns>();
        initialBaseMoveSpeed = baseMoveSpeed;
    }

    void Update()
    {

        movementInput = movement.action.ReadValue<Vector2>();

        // calculate drag for curr speed using linear approximation, same as Unity does normally.
        // currSpeed = currSpeed * ( 1 - Time.deltaTime * linearDrag);
        // Debug.Log(currSpeed);

    }

    void FixedUpdate()
    {
        /*
        curr speed is how fast the player moves with the direction input. Dash speed is based off of this.
        curr speed is tracked seperately, so that it can be increased at a controled rate instead of basing the increases off of current velocity.
        if a player hits a wall, they can still move at currSpeed, instead of velocity being set to 0.
        */

        if(CanMove) 
        {

            // if(rb.velocity.magnitude < baseMoveSpeed)
            // {
            //     rb.AddForce(movementInput * baseMoveSpeed*2, ForceMode2D.Force);
            //     //rb.velocity = rb.velocity.normalized * baseMoveSpeed;
            // }
            // else
            // {
            //     Vector2 diff = rb.velocity.normalized - movementInput;
            //     rb.AddForce(diff * 1.5f, ForceMode2D.Force);
            // }

            Vector2 targetVel = movementInput * baseMoveSpeed;

            // get difference betwen current velocity and velocity we want to accelerate to
            //targetVel = Vector2.Lerp(rb.velocity, targetVel, 0.5f);
            Vector2 diff = targetVel - rb.velocity;
            // rate of change in speed. set accel and deccel = to base move speed for instant movement
            float accelRate = Mathf.Abs(targetVel.magnitude) > 0.01f ? accel : deaccel;

            float speed = Mathf.Pow(diff.magnitude * accelRate, velPower);

            // add force gives us slightly laggy movement
            rb.AddForce(movementInput * speed);

            Vector2 normalized = rb.velocity.normalized;
            float playerRadValue = Mathf.Atan2(normalized.y, normalized.x);
            float playerAngle= playerRadValue * (180/Mathf.PI);
            rb.transform.localRotation = Quaternion.Euler(0,0,playerAngle -90);

            

            if(rb.velocity.magnitude < 1f && movementInput == Vector2.zero)
            {
                rb.velocity = Vector2.zero;
                rb.transform.localRotation = Quaternion.Euler(0,0,0);
            }
            

        
            // if(movementInput != Vector2.zero)
            // {
            //     // move using current speed, with a minimum of base move speed
            //     //rb.velocity = currSpeed > baseMoveSpeed ? movementInput * currSpeed : movementInput * baseMoveSpeed ;

            //     //rb.velocity = rb.velocity.magnitude > baseMoveSpeed ? movementInput * rb.velocity.magnitude : movementInput * baseMoveSpeed ;
            //     rb.velocity = rb.velocity.magnitude > baseMoveSpeed ? movementInput * rb.velocity.magnitude : movementInput * baseMoveSpeed ;
            
            

            //     // float playerRadValue = Mathf.Atan2(movementInput.y, movementInput.x);
            //     // float playerAngle= playerRadValue * (180/Mathf.PI);
            //     // rb.transform.localRotation = Quaternion.Euler(0,0,playerAngle -90);
            // }
            // else
            // {
            //     rb.velocity = rb.velocity.magnitude > baseMoveSpeed ? movementInput * rb.velocity.magnitude : movementInput * baseMoveSpeed ;
            //     //rb.velocity = Vector2.zero;
            // }
            
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
    private void OnDisable()
    {
        dash.action.performed -= Dash;
        
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

    // queue wall jump so player can press wall jump slightly early before hitting a wall
    private void QueueWallJump()
    {
        touchingWall = Physics2D.OverlapCircle(transform.position, 0.6f, wallLayer);
        
        Vector2 direction = movement.action.ReadValue<Vector2>();

        if(touchingWall && direction != Vector2.zero)
        {
            //wallJumpDir = direction;
            wallJumpQueued = true;

            StartCoroutine(TurnOffWallJump());
            
        }
    }

    private void Dash(InputAction.CallbackContext context)
    {
        QueueWallJump();

        if(!wallJumpQueued)
        {
            if(canDash)
            {   
                CancelOtherMovement();
                StartCoroutine(DashWindup(dashPause));

                // pause the game for a bit before dashing. if more forgiving for choosing direction and makes dashes feel strong
                pauseControl.ActionPause(dashPause, StartDashing);
                
            }
        }
    }

    void StartDashing()
    {
        Vector2 direction = movement.action.ReadValue<Vector2>();
        currAction = PerformDash(direction * dashSpeed, dashTime);
        StartCoroutine(currAction);
    }

    

    // if they player does not hit a wall in time, dont wall jump
    IEnumerator TurnOffWallJump()
    {
        yield return new WaitForSeconds(0.5f);
        wallJumpQueued = false;
    }

    public void StartWallJump()
    {
        // add slight pause befoe wall jumping
        StartCoroutine(DashWindup(wallJumpPause));
        pauseControl.ActionPause(wallJumpPause, WallJump);
        //WallJump(wallJumpDir);
    }
    void WallJump()
    {
        // StartCoroutine(DashWindup(wallJumpPause));
        // GameObject.FindGameObjectWithTag("PauseControl").GetComponent<PauseControl>().ActionPause(wallJumpPause, WallJump());
        //GameObject.FindGameObjectWithTag("PauseControl").GetComponent<PauseControl>().Sleep(wallJumpPause);
        Vector2 direction = movement.action.ReadValue<Vector2>();
        BoxCollider2D wall = touchingWall.gameObject.GetComponent<BoxCollider2D>();
            
        Vector3 wallSize = wall.bounds.size;
        Vector3 wallPos = touchingWall.transform.position;
        Vector2 dir = Vector2.zero;
        
        CancelOtherMovement();

        float wallJumpSpeed = wallJumpForce;

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

        // // Teleport the player a small distance along the new direction vector, gives the sense they "bounced" off the enemy
        // Vector2 teleportLocation = new Vector2(rb.position.x, rb.position.y) + dir.normalized * 1.1f;
        // rb.position = teleportLocation;
        float rad = Mathf.Atan2(dir.y, dir.x);
        float angle = rad * (180/Mathf.PI);
        Quaternion rotation = Quaternion.Euler(0,0, angle -90);
        //Vector2 position = transform.position + (transform.position - (Vector3)dir).normalized * 0.5f;
        Instantiate(wallJumpEffect, transform.position, rotation);
        

        currAction = PerformWalljump(dir, wallJumpTime);
        StartCoroutine(currAction);
    }

    private void CancelOtherMovement()
    {
        gameObject.GetComponent<PlayerGrapple>().EndGrapple();
        gameObject.GetComponent<PlayerReflectDash>().EndReflectDash();
        // CanMove = false;
        // DisableBasicDash();

        EndDash();
    }
    public void EndDash()
    {
        if(isDashing)
        {
            //StopAllCoroutines();
            StopCoroutine(currAction);
            ResetDashStatus();
        }
    }

    private void ResetDashStatus()
    {
        CanMove = true;
        EnableBasicDash();
        dash.action.Enable();
        isDashing = false;
        rb.drag = gameObject.GetComponent<PlayerGrapple>().initialDrag;
    }


    // squish the player a bit before dashing for visual dash indication
    IEnumerator DashWindup(float time)
    {
        // Vector2 scale = dir * 0.5f;
        // transform.localScale = new Vector2(0.7f, 0.7f);
        // yield return new WaitForSecondsRealtime(time);
        // transform.localScale = new Vector2(1,1);
        float startTime = Time.time;

        while(Time.time - startTime <= time)
        {
            float ratio = (Time.time - startTime) / time;
            //float scale = Mathf.Lerp(1, 0.7f, ratio);
            float scale = animCurve.Evaluate(ratio);
            transform.localScale = new Vector2(scale, scale);
            yield return null;
        }
        transform.localScale = new Vector2(1, 1);
    }
    IEnumerator PerformDash(Vector2 force, float time)
    {
        playerCooldowns.StartDashCooldown();
        isDashing = true;
        canDash = false;

        //CanMove = false;
        DisableBasicDash();

        float startTime = Time.time;
        rb.drag = 0;

        playerAudio.PlayDashSound();

        while (Time.time - startTime <= dashTime)
		{
			rb.velocity = force.normalized * dashSpeed;
            prevDashVelocity = rb.velocity;
			//Pauses the loop until the next frame, creating something of a Update loop. 
			//This is a cleaner implementation opposed to multiple timers and this coroutine approach is actually what is used in Celeste :D
			yield return null;
		}

		//Begins the "end" of our dash where we return some control to the player but still limit run acceleration (see Update() and Run())

		rb.velocity = dashSpeed * 0.5f * force.normalized;


        //rb.AddForce(force, ForceMode2D.Impulse);
        

        // rotate player
        // float playerRadValue = Mathf.Atan2(rb.velocity.y, rb.velocity.x);
        // float playerAngle= playerRadValue * (180/Mathf.PI);
        // rb.transform.localRotation = Quaternion.Euler(0,0,playerAngle -90);

        //yield return new WaitForSeconds(time);

        ResetDashStatus();
    }
    
    IEnumerator PerformWalljump(Vector2 force, float time)
    {
        wallJumpCombo += 1;
        if(wallJumpCombo == 1)
        {
            StartCoroutine(StartWallJumpComboTimer());
        }
        
        isDashing = true;

        // if the player walljumps within the action window, the player's speed increases
        if(playerImpact.actionWindowIsActive)
        {
            // GameObject animation = Instantiate(actionWindowIndicatorPrefab, transform.position, transform.rotation);
            // animation.transform.SetParent(transform, false);
            playerCooldowns.EndAllCooldowns();
            // GetComponent<PlayerAnimation>().PlayCooldownRefreshAnimation();
            //currSpeed += PlayerImpact.IMPACTSPEEDINCREASE;
        }


        // float forceRad = Mathf.Atan2(-force.y, -force.x);
        // float angle = forceRad * (180/Mathf.PI);
        // particleSys.transform.rotation = Quaternion.Euler(0,0,angle);

        

        // // rb.velocity = force.normalized;
        // // rb.AddForce(force, ForceMode2D.Impulse);
        // prevDashVelocity = rb.velocity;

        float startTime = Time.time;
        rb.drag = 0;

        playerAudio.PlayJumpSound();

        while (Time.time - startTime <= dashTime)
		{
			rb.velocity = force.normalized * wallJumpForce;
            prevDashVelocity = rb.velocity;
			//Pauses the loop until the next frame, creating something of a Update loop. 
			//This is a cleaner implementation opposed to multiple timers and this coroutine approach is actually what is used in Celeste :D
			yield return null;
		}

        //startTime = Time.time;

		//Begins the "end" of our dash where we return some control to the player but still limit run acceleration (see Update() and Run())

		rb.velocity = wallJumpForce * 0.5f * force.normalized;
        //rb.drag = gameObject.GetComponent<PlayerGrapple>().initialDrag;

        // rotate player
        // float playerRadValue = Mathf.Atan2(rb.velocity.y, rb.velocity.x);
        // float playerAngle= playerRadValue * (180/Mathf.PI);
        // rb.transform.localRotation = Quaternion.Euler(0,0,playerAngle -90);

        //yield return new WaitForSeconds(time);

        ResetDashStatus();
    }

    IEnumerator StartWallJumpComboTimer()
    {
        float wallJumpComboTime = 1.5f;
        float wallJumpComboStart = Time.time;

        while (Time.time - wallJumpComboStart <= wallJumpComboTime)
        {
            if(wallJumpCombo == 2)
            {
                // use the grapple spin animation as a cool wall jump
                GetComponent<PlayerAnimation>().GrappleSpin();
                break;
            }
            yield return null;
        }

        wallJumpCombo = 0;

    }

    public int GetWallJumpCombo()
    {
        return wallJumpCombo;
    }

}
