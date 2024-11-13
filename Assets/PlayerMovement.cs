using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
//using System.Numerics;

public class PlayerMovement : MonoBehaviour
{
    
    public float initialDrag;
    [SerializeField]
    public InputActionReference movement, dash;

    private bool isDashing = false;

    // [Header("Reflect Dash")]
    // [SerializeField] private GameObject arrowPrefab;
    // [SerializeField] private GameObject actionWindowIndicatorPrefab;
    // private GameObject reflectDashArrow = null;
    // private GameObject relfectDashtarget = null;
    // private float reflectDashSpeedModifier = 2f;

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
    //public bool isWallJumping = false;
    //private Vector3 wallJumpVelo;
    //private Vector3 prevCollisionNormal;

    Rigidbody2D rb;
    // List<GameObject> currentCollisions;
    // List<GameObject> removalQueue;
    PlayerImpact playerImpact;

    // Vector3 enemyPos;

    public float dashSpeed {get; }= 3f;
    private float dashTime = 0.3f;
    private float baseMoveSpeed = 1;

    public bool CanMove {get; set;} = true;

    public float prevFrameSpeed;

    void Start()
    {
        
        //Physics.IgnoreLayerCollision(6, 8, true);
        rb = GetComponent<Rigidbody2D>();
        initialDrag = rb.drag;
        // currentCollisions = new List <GameObject> ();
        // removalQueue = new List<GameObject> ();
        playerImpact= GetComponent<PlayerImpact> ();
    }

    void Update()
    {
        // if(isWallJumping)
        // {
        //    Debug.Log("update velo " + rb.velocity);
        // }
        // if(CanMove) 
        // {
        //     //Debug.Log("moving");
        //     Vector2 movementInput = movement.action.ReadValue<Vector2>();

        //     // move using current speed, with a minimum of base move speed
        //     rb.velocity = rb.velocity.magnitude > baseMoveSpeed ? movementInput * rb.velocity.magnitude : movementInput * baseMoveSpeed ;

        //     // Rotate player. If the player is reflect dashing, only rotate the arrow, not the player
        //     if(!reflectDashArrow)
        //     {
        //         float playerRadValue = Mathf.Atan2(rb.velocity.y, rb.velocity.x);
        //         float playerAngle= playerRadValue * (180/Mathf.PI);
        //         rb.transform.localRotation = Quaternion.Euler(0,0,playerAngle -90);
        //     }
            
        // }
        // else{
        //     Debug.Log("not moving");
        // }

        // if(reflectDashArrow)
        // {
        //     Vector2 dir = movement.action.ReadValue<Vector2>();
        //     float radvalue = Mathf.Atan2(dir.y, dir.x);
        //     float angle= radvalue * (180/Mathf.PI);
        //     reflectDashArrow.transform.localRotation = Quaternion.Euler(0,0,angle -90);

        //     reflectDashArrow.transform.position = transform.position;
        // }

        //RemoveNullCollisions();

    }

    void FixedUpdate()
    {
        if(CanMove) 
        {
            //Debug.Log("moving");
            Vector2 movementInput = movement.action.ReadValue<Vector2>();

            // move using current speed, with a minimum of base move speed
            rb.velocity = rb.velocity.magnitude > baseMoveSpeed ? movementInput * rb.velocity.magnitude : movementInput * baseMoveSpeed ;

            float playerRadValue = Mathf.Atan2(rb.velocity.y, rb.velocity.x);
            float playerAngle= playerRadValue * (180/Mathf.PI);
            rb.transform.localRotation = Quaternion.Euler(0,0,playerAngle -90);

            // Rotate player. If the player is reflect dashing, only rotate the arrow, not the player
            // if(!gameObject.GetComponent<PlayerReflectDash>().reflectDashing)
            // {
            //     float playerRadValue = Mathf.Atan2(rb.velocity.y, rb.velocity.x);
            //     float playerAngle= playerRadValue * (180/Mathf.PI);
            //     rb.transform.localRotation = Quaternion.Euler(0,0,playerAngle -90);
            // }
            
        }
        // Debug.Log(isWallJumping);
        // if(isWallJumping)
        // {
        //     Debug.Log("wj velo " + wallJumpVelo);
        //     rb.velocity = wallJumpVelo;
        // }
    }

    void LateUpdate()
    {
        prevFrameSpeed = rb.velocity.magnitude;
        // if(isWallJumping)
        // {
        //     //  // // turn player
        //     // float playerRadValue = Mathf.Atan2(rb.velocity.y, rb.velocity.x);
        //     // float playerAngle= playerRadValue * (180/Mathf.PI);
        //     // rb.transform.localRotation = Quaternion.Euler(0,0,playerAngle -90);

        //     // // Debug.Log(CanMove);
        //     // Debug.Log(rb.velocity);

        //     // EnableBasicDash();
        //     // Invoke("EndDash", dashTime);

        //     // rb.AddForce(wallJumpDir,  ForceMode2D.Impulse);
        //     //Physics.IgnoreLayerCollision(8, 6, false);
        //     isWallJumping = false;
        // }
    }

    // private void RemoveNullCollisions()
    // {
    //     foreach(GameObject obj in removalQueue)
    //     {
    //         currentCollisions.Remove(obj);
    //     }
    //     removalQueue.Clear();
    // }

    private void OnEnable()
    {
        dash.action.performed += Dash;
        
    }

    // void OnTriggerEnter2D (Collider2D col) 
    // {

	// 	// You can only reflect dash off of enemies
    //     if(col.gameObject.tag == "Buffer")
    //     {
    //         currentCollisions.Add(col.gameObject.transform.parent.gameObject);
    //     }
	// }

	// void OnTriggerExit2D (Collider2D col) 
    // {
    //     if(col.gameObject.tag == "Buffer")
    //     {
    //         currentCollisions.Remove(col.gameObject.transform.parent.gameObject);
    //     }
		
	// }

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
        //CanMove = false;

        QueueWallJump();
        //Invoke("EndDash", dashTime);
    }

    // void OnCollisionEnter2D(Collision2D collision)
    // {
    //     // foreach (var contact in collision.contacts)
    //     // {
    //     //     Debug.DrawRay(contact.point, contact.normal, Color.red, 2f);

    //     //     Debug.Log("Hit normal: " + contact.normal);
    //     // }

    //     //TODO: use if want to add angled walls
    //     //prevCollisionNormal = collision.GetContact(0).normal;
    // }

    private void QueueWallJump()
    {
        touchingWall = Physics2D.OverlapCircle(wallCheck.position, 0.6f, wallLayer);
        Vector2 direction = movement.action.ReadValue<Vector2>();

        if(touchingWall && direction != Vector2.zero)
        {
            wallJumpDir = direction;
            wallJumpQueued = true;
            // wallJumpDir = direction;
            StartCoroutine(TurnOffWallJump());
            //WallJump(direction);
            
        }
    }

    private void Dash(InputAction.CallbackContext context)
    {
        //CanMove = false;

        touchingWall = Physics2D.OverlapCircle(wallCheck.position, 0.6f, wallLayer);
        Vector2 direction = movement.action.ReadValue<Vector2>();

        QueueWallJump();

        if(!wallJumpQueued)
        {
            // if(currentCollisions.Count > 0)
            // {
            //     ReflectDashSetup();
            //     //ReflectDash(direction);
            // }
            // else
            // {
                
                //Invoke("EndDash", dashTime);
                //Vector2 force = direction * (dashSpeed + rb.velocity.magnitude);
                StartCoroutine(PerformDash(direction * (dashSpeed + rb.velocity.magnitude), dashTime));
            //}

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
        //CanMove = false;
        // prevent collisions with walls from stopping wall jump
        //Physics.IgnoreLayerCollision(8, 6, true);

        
        BoxCollider2D wall = touchingWall.gameObject.GetComponent<BoxCollider2D>();
            
        Vector3 wallSize = wall.bounds.size;
        Vector3 wallPos = touchingWall.transform.position;
        Vector2 dir = Vector2.zero;
        
        CancelOtherMovement();
        // gameObject.GetComponent<PlayerGrapple>().EndGrapple();
        // //StopCoroutine(endDash); // In case a normal dash was canceled
        // EndDash();
        // CanMove = false;
        //DisableBasicDash();
        //dash.action.Disable();

       
        //Find out wall orientation compared to player

        // // wall is vertical
        // if(prevCollisionNormal.y == 0)
        // {
        //     dir = new Vector3(prevCollisionNormal.x * dashSpeed * 1.5f, direction.y * dashSpeed * 1.5f, 0);
        //     rb.AddForce(dir,  ForceMode2D.Impulse);
        // }
        // // wall is horizontal
        // else if(prevCollisionNormal.x == 0)
        // {
        //     dir = new Vector3(direction.x * dashSpeed * 1.5f, prevCollisionNormal.y * dashSpeed * 1.5f, 0);
        //     rb.AddForce(dir,  ForceMode2D.Impulse);
        // }
        // // wall is diagonal under and left
        // else if(prevCollisionNormal.x > 0 && prevCollisionNormal.y > 0)
        // {
        //     vector = Quaternion.Euler(0, , 0) * vector;
        //     dir = new Vector3(dashSpeed * 1.5f, direction.y * dashSpeed * 1.5f, 0);
        //     rb.AddForce(dir,  ForceMode2D.Impulse);
        // }
        // // wall is vertical to the right
        // else if(wallPos.x - wallSize.x/2 > transform.position.x)
        // {
        //     dir = new Vector3(-dashSpeed * 1.5f, direction.y * dashSpeed * 1.5f, 0);
        //     rb.AddForce(dir,  ForceMode2D.Impulse);
        // }
        // // wall on botom
        // else if(wallPos.y + wallSize.y/2 < transform.position.x)
        // {
        //     dir = new Vector3(direction.x * dashSpeed * 1.5f, dashSpeed * 1.5f, 0);
        //     rb.AddForce(dir,  ForceMode2D.Impulse);
        // }
        // // wall on top
        // else if(wallPos.y - wallSize.y/2 > transform.position.x)
        // {
        //     dir = new Vector3(direction.x * dashSpeed * 1.5f, -dashSpeed * 1.5f,0);
        //     rb.AddForce(dir,  ForceMode2D.Impulse);
        // }
        
        // 90 degree walls only
        // wall is vertical to the left of player
        
         // Indicate to the player that they dashed within the window to add the speed they were going at impact
        //Debug.Log("pre wj velo " + rb.velocity);

        if(playerImpact.ImpactSpeed > 0)
        {
            GameObject animation = Instantiate(actionWindowIndicatorPrefab, transform.position, transform.rotation);
            animation.transform.SetParent(transform, false);
        }

        float wallJumpSpeed = dashSpeed * wallJumpSpeedModifier + playerImpact.ImpactSpeed;
        Debug.Log("wspeed " + wallJumpSpeed);

        //Set velocity to 0 so the dash force is not mitigated by current velocity
        rb.velocity = Vector3.zero;
        //Vector2 force;

        if(wallPos.x + wallSize.x/2 < transform.position.x)
        {
            dir = new Vector2(wallJumpSpeed, direction.y * wallJumpSpeed);
            //rb.AddForce(dir,  ForceMode2D.Impulse);
            //force = dir
        }
        // wall is vertical to the right
        else if(wallPos.x - wallSize.x/2 > transform.position.x)
        {
            dir = new Vector2(-wallJumpSpeed, direction.y * wallJumpSpeed);
            //rb.AddForce(dir,  ForceMode2D.Impulse);
        }
        // wall on botom
        else if(wallPos.y + wallSize.y/2 < transform.position.y)
        {
            dir = new Vector2(direction.x * wallJumpSpeed, wallJumpSpeed);
            //rb.AddForce(dir,  ForceMode2D.Impulse);
        }
        // wall on top
        else if(wallPos.y - wallSize.y/2 > transform.position.y)
        {

            dir = new Vector2(direction.x * wallJumpSpeed, -wallJumpSpeed);
            //rb.AddForce(dir,  ForceMode2D.Impulse);
        }

        //isWallJumping = true;

        // // turn player
        // float playerRadValue = Mathf.Atan2(rb.velocity.y, rb.velocity.x);
        // float playerAngle= playerRadValue * (180/Mathf.PI);
        // rb.transform.localRotation = Quaternion.Euler(0,0,playerAngle -90);

        //wallJumpVelo = rb.velocity;

        //Debug.Log(rb.velocity);
        //Debug.Log("WallJump");

        //EnableBasicDash();
        StartCoroutine(PerformDash(dir, wallJumpTime));
        //Invoke("EndDash", dashTime);

        

        //Vector3 diff = touchingWall.transform.position - transform.position;
        // Vector3 dir = diff.normalized * -1;
        // Debug.Log(dir);
        //rb.AddForce(dir * (dashSpeed * 1.5f + rb.velocity.magnitude), ForceMode2D.Impulse);
    }

    private void CancelOtherMovement()
    {
        gameObject.GetComponent<PlayerGrapple>().EndGrapple();
        gameObject.GetComponent<PlayerReflectDash>().EndReflectDash();
        //StopCoroutine(endDash); // In case a normal dash was canceled
        EndDash();
    }

    // IEnumerator WJ()
    // {
    //     yield return new W;
    // }

    // TODO: make it so that you can cancel grapple with reflect dash
    // private void ReflectDashSetup()
    // {
        
    //     CanMove = false;

    //     Vector2 direction = movement.action.ReadValue<Vector2>();
    //     GameObject closestEnemy = null;

    //     float closestEnemyDistance = float.MaxValue;

    //     foreach(GameObject col in currentCollisions)
    //     {
    //         // must check if gameobject is null to check if destroyed. (ActiveInHierarchy does not do this)
    //         if(col.gameObject != null)
    //         {
    //             float distance = (col.GetComponent<Rigidbody2D>().position - rb.position).magnitude;
    //             if(distance < closestEnemyDistance)
    //             {
    //                 closestEnemyDistance = distance;
    //                 closestEnemy = col;
    //             }
    //         }
    //         else
    //         {
    //             // When an enemy dies on impact, the trigger exit does not happen, so need to remove the collision here
    //             removalQueue.Add(col);
    //         }
    //     };

    //     if(closestEnemy != null)
    //     {
    //         dash.action.canceled += ReflectDash;
    //         relfectDashtarget = closestEnemy;

    //         //Stop enemy from moving away
    //         relfectDashtarget.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;

    //         // get direction vector relative to enemy
    //         enemyPos = relfectDashtarget.transform.position;

    //         // Teleport the player to the enemy center
    //         Vector2 teleportLocation = new Vector2(enemyPos.x, enemyPos.y);
    //         rb.position = teleportLocation;

    //         reflectDashArrow = Instantiate(arrowPrefab, new Vector3(rb.position.x, rb.position.y, 0), transform.rotation);
    //     }
     
    // }

    // private void ReflectDash(InputAction.CallbackContext context)
    // {
    //     dash.action.Disable();
    //     Vector2 direction = movement.action.ReadValue<Vector2>();

    //     // Teleport the player a small distance along the new direction vector, gives the sense they "bounced" off the enemy
    //     Vector2 teleportLocation = new Vector2(enemyPos.x, enemyPos.y) + direction;
    //     rb.position = teleportLocation;

    //     // Indicate to the player that they dashed within the window to add the speed they were going at impact
    //     if(playerImpact.ImpactSpeed > 0)
    //     {
    //         GameObject animation = Instantiate(actionWindowIndicatorPrefab, transform.position, transform.rotation);
    //         animation.transform.SetParent(transform, false);
    //     }

    //     float reboundDashSpeed = dashSpeed * reflectDashSpeedModifier + playerImpact.ImpactSpeed;
    //     rb.AddForce(direction * reboundDashSpeed, ForceMode2D.Impulse);

    //     dash.action.canceled -= ReflectDash;
    //     relfectDashtarget.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;

    //     //TODO: change this to the new version if needed
    //     //Invoke("EndDash", dashTime);
    //     endDash = StartCoroutine(EndDash(dashTime));

    //     Destroy(reflectDashArrow);
    // }

    // private void EndDash()
    // {
       
    //     CanMove = true;
    //     EnableBasicDash();
    //     dash.action.Enable();
        
    //     //Debug.Log("ended");
       
    //     //isWallJumping = false;
    //     //Physics.IgnoreLayerCollision(8, 6, false);
    // }
    public void EndDash()
    {
        if(isDashing)
        {
            StopAllCoroutines();
        }
    }

    private void ResetDashStatus()
    {
        Debug.Log("reset");
        CanMove = true;
        EnableBasicDash();
        dash.action.Enable();
        isDashing = false;
    }
    IEnumerator PerformDash(Vector2 force, float time)
    {
        CanMove = false;
        isDashing = true;
        DisableBasicDash();
        rb.AddForce(force, ForceMode2D.Impulse);
        Debug.Log("wdash " + rb.velocity.magnitude);

        // rotate player
        float playerRadValue = Mathf.Atan2(rb.velocity.y, rb.velocity.x);
        float playerAngle= playerRadValue * (180/Mathf.PI);
        rb.transform.localRotation = Quaternion.Euler(0,0,playerAngle -90);

        yield return new WaitForSeconds(time);

        ResetDashStatus();
        
        //Debug.Log("ended");
       
        //isWallJumping = false;
        //Physics.IgnoreLayerCollision(8, 6, false);
    }
    // Since the dash reads from movement input, this allows turning on/off basic movement but allowing dashes
    // public void SetMovementAbility(bool isActive)
    // {
    //     canMove = isActive;
    // }

    
}
