using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerReflectDash : MonoBehaviour
{
    [SerializeField] private InputActionReference reflectDash, movement, pointerPos;
    [Header("Reflect Dash")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private GameObject actionWindowIndicatorPrefab;
    [SerializeField] private LayerMask bufferLayer;
    private GameObject reflectDashArrow = null;
    private GameObject relfectDashtarget = null;
    private float reflectDashSpeed= 12f;
    public bool reflectDashing = false;
    public Vector2 prevVelocity;
    private Vector2 reflectDashDirection;
    //private float reflectDashSpeed = 4f;
    private float reflectDashTime = 0.2f;
    Vector3 enemyPos;

    List<GameObject> currentCollisions;
    List<GameObject> removalQueue;
    Rigidbody2D rb;

    PlayerMovement playerMovement;
    PlayerImpact playerImpact;
    PlayerAudio playerAudio;
    PlayerCooldowns playerCooldowns;

    Coroutine currDash;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentCollisions = new List <GameObject> ();
        removalQueue = new List<GameObject> ();
        playerMovement = gameObject.GetComponent<PlayerMovement>();
        playerImpact = gameObject.GetComponent<PlayerImpact>();
        playerAudio = gameObject.GetComponentInChildren<PlayerAudio>();
        playerCooldowns = gameObject.GetComponent<PlayerCooldowns>();
    }

    void Update()
    {
        RemoveNullCollisions();

        if(reflectDashing)
        {
            if(movement.action.ReadValue<Vector2>() != Vector2.zero)
            {
                reflectDashDirection = movement.action.ReadValue<Vector2>();
            }
            
        }
    }


    private void OnEnable()
    {
        reflectDash.action.performed += CheckReflectDash;
        
    }
    private void OnDisable()
    {
        reflectDash.action.performed -= CheckReflectDash;
        
    }

    private void CheckReflectDash(InputAction.CallbackContext context)
    {
        // We can only reflect dash off of an enemy
        // if(currentCollisions.Count > 0)
        // {
        //     ReflectDashSetup();
        // }
        ReflectDashSetup();
    }

    private void ReflectDashSetup()
    {
        
        

        // Vector2 direction = movement.action.ReadValue<Vector2>();
        // GameObject closestEnemy = null;

        // float closestEnemyDistance = float.MaxValue;

        // foreach(GameObject col in currentCollisions)
        // {
        //     // must check if gameobject is null to check if destroyed. (ActiveInHierarchy does not do this)
        //     if(col.gameObject != null)
        //     {
        //         float distance = (col.GetComponent<Rigidbody2D>().position - rb.position).magnitude;
        //         if(distance < closestEnemyDistance)
        //         {
        //             closestEnemyDistance = distance;
        //             closestEnemy = col;
        //         }
        //     }
        //     else
        //     {
        //         // When an enemy dies on impact, the trigger exit does not happen, so need to remove the collision here
        //         removalQueue.Add(col);
        //     }
        // };

        // if(closestEnemy != null)
        // {
        Vector2 dir = GetDashDirection();
        RaycastHit2D hitTarget = Physics2D.Raycast(gameObject.transform.position, dir, distance: 2f, layerMask: bufferLayer);
        if(hitTarget)
        {
            CancelOtherMovement();
            reflectDashing = true;
            prevVelocity = rb.velocity;

            reflectDash.action.canceled += ReflectDash;
            //relfectDashtarget = closestEnemy;
            relfectDashtarget = hitTarget.transform.gameObject;

            //Stop enemy from moving away
            relfectDashtarget.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;

            //Freeze position
            rb.velocity = Vector2.zero;

            // get direction vector relative to enemy
            enemyPos = relfectDashtarget.transform.position;

            // Teleport the player to the enemy center
            Vector2 teleportLocation = new Vector2(enemyPos.x, enemyPos.y);
            rb.position = teleportLocation;

            reflectDashArrow = Instantiate(arrowPrefab, new Vector3(rb.position.x, rb.position.y, 0), transform.rotation);

            playerAudio.PlayReflectDashAudio();

        }
            
        //}
     
    }

    private Vector2 GetDashDirection()
    {
        Vector2 mousePos = pointerPos.action.ReadValue<Vector2>();
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        Vector2 direction = mousePos - rb.position;

        return direction.normalized;
    }

    private void ReflectDash(InputAction.CallbackContext context)
    {
        // Once the dash has started, conditional dashes are allowed, but not basic dashes
        playerMovement.dash.action.Enable();
        playerMovement.DisableBasicDash();

        // Vector2 direction = movement.action.ReadValue<Vector2>();

        // Teleport the player a small distance along the new direction vector, gives the sense they "bounced" off the enemy
        // Vector2 teleportLocation = new Vector2(enemyPos.x, enemyPos.y) + direction * 1.3f;
        // rb.position = teleportLocation;

        // Indicate to the player that they dashed within the window to add the speed they were going at impact
        if(playerImpact.actionWindowIsActive)
        {
            // GameObject animation = Instantiate(actionWindowIndicatorPrefab, transform.position, transform.rotation);
            // animation.transform.SetParent(transform, false);
            playerCooldowns.EndAllCooldowns();
            //GetComponent<PlayerAnimation>().PlayCooldownRefreshAnimation();
            //playerMovement.currSpeed += PlayerImpact.IMPACTSPEEDINCREASE;
        }

        //float reboundDashSpeed = playerMovement.dashSpeed; //+ playerMovement.currSpeed

        reflectDash.action.canceled -= ReflectDash;

        if(relfectDashtarget != null)
        {
            relfectDashtarget.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        }
        
        currDash = StartCoroutine(PerformDash(reflectDashDirection* reflectDashSpeed, reflectDashTime));

        Destroy(reflectDashArrow);
    }

    private void CancelOtherMovement()
    {
        playerMovement.EndDash();
        gameObject.GetComponent<PlayerGrapple>().EndGrapple();
        playerMovement.CanMove = false;
        playerMovement.dash.action.Disable();

        // allows to start new reflect dash during a reflect dash
        if(currDash != null)
        {
            StopCoroutine(currDash);
            reflectDash.action.Enable();
        }
        
    }

    private void RemoveNullCollisions()
    {
        foreach(GameObject obj in removalQueue)
        {
            currentCollisions.Remove(obj);
        }
        removalQueue.Clear();
    }

    void OnTriggerEnter2D (Collider2D col) 
    {
		// You can only reflect dash off of enemies
        if(col.gameObject.tag == "Buffer")
        {
            currentCollisions.Add(col.gameObject.transform.parent.gameObject);
        }
	}

	void OnTriggerExit2D (Collider2D col) 
    {
        if(col.gameObject.tag == "Buffer")
        {
            currentCollisions.Remove(col.gameObject.transform.parent.gameObject);
        }
		
	}

    public void EndReflectDash()
    {
        if(reflectDashing)
        {
            StopAllCoroutines();
            ResetDashStatus();
        }
    }

    void ResetDashStatus()
    {
        playerMovement.CanMove = true;
        playerMovement.dash.action.Enable();
        playerMovement.EnableBasicDash();
        reflectDashing = false;
        rb.drag = gameObject.GetComponent<PlayerGrapple>().initialDrag;
    }

    IEnumerator PerformDash(Vector2 force, float time)
    {

        //rb.AddForce(force, ForceMode2D.Impulse);
        float startTime = Time.time;
        rb.drag = 0;

        // // rotate player
        // float playerRadValue = Mathf.Atan2(rb.velocity.normalized.y, rb.velocity.normalized.x);
        // float playerAngle= playerRadValue * (180/Mathf.PI);
        // rb.transform.localRotation = Quaternion.Euler(0,0,playerAngle - 90);
        reflectDash.action.Disable();
        // instead of teleporting at the beginning of the dash, just move really quickly instead
        while (Time.time - startTime <= 0.05)
		{
			rb.velocity = force * 2;
            Vector2 normalized = rb.velocity.normalized;
            float playerRadValue = Mathf.Atan2(normalized.y, normalized.x);
            float playerAngle= playerRadValue * (180/Mathf.PI);
            rb.transform.localRotation = Quaternion.Euler(0,0,playerAngle -90);
            //prevDashVelocity = rb.velocity;
			//Pauses the loop until the next frame, creating something of a Update loop. 
			//This is a cleaner implementation opposed to multiple timers and this coroutine approach is actually what is used in Celeste :D
			yield return null;
		}

        startTime = Time.time;
        while (Time.time - startTime <= time)
		{
            // don't allow another reflect dash until current dash is half done
            if(Time.time - startTime > time * 0.5)
            {
                reflectDash.action.Enable();
            }
			rb.velocity = force;
            Vector2 normalized = rb.velocity.normalized;
            float playerRadValue = Mathf.Atan2(normalized.y, normalized.x);
            float playerAngle= playerRadValue * (180/Mathf.PI);
            rb.transform.localRotation = Quaternion.Euler(0,0,playerAngle -90);
            //prevDashVelocity = rb.velocity;
			//Pauses the loop until the next frame, creating something of a Update loop. 
			//This is a cleaner implementation opposed to multiple timers and this coroutine approach is actually what is used in Celeste :D
			yield return null;
		}

		//Begins the "end" of our dash where we return some control to the player but still limit run acceleration (see Update() and Run())

		rb.velocity = force.magnitude * 0.7f * force.normalized;

        yield return new WaitForSeconds(time);

        ResetDashStatus();
       
    }
}
