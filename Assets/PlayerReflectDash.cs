using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerReflectDash : MonoBehaviour
{
    [SerializeField] private InputActionReference reflectDash, movement;
    [Header("Reflect Dash")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private GameObject actionWindowIndicatorPrefab;
    private GameObject reflectDashArrow = null;
    private GameObject relfectDashtarget = null;
    private float reflectDashSpeed= 12f;
    public bool reflectDashing = false;
    public Vector2 prevVelocity;
    //private float reflectDashSpeed = 4f;
    private float reflectDashTime = 0.3f;
    Vector3 enemyPos;

    List<GameObject> currentCollisions;
    List<GameObject> removalQueue;
    Rigidbody2D rb;

    PlayerMovement playerMovement;
    PlayerImpact playerImpact;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentCollisions = new List <GameObject> ();
        removalQueue = new List<GameObject> ();
        playerMovement = gameObject.GetComponent<PlayerMovement>();
        playerImpact = gameObject.GetComponent<PlayerImpact>();
    }

    void Update()
    {
        RemoveNullCollisions();
    }


    private void OnEnable()
    {
        reflectDash.action.performed += CheckReflectDash;
        
    }

    private void CheckReflectDash(InputAction.CallbackContext context)
    {
        // We can only reflect dash off of an enemy
        if(currentCollisions.Count > 0)
        {
            ReflectDashSetup();
        }
    }

    private void ReflectDashSetup()
    {
        
        CancelOtherMovement();
        reflectDashing = true;
        prevVelocity = rb.velocity;

        Vector2 direction = movement.action.ReadValue<Vector2>();
        GameObject closestEnemy = null;

        float closestEnemyDistance = float.MaxValue;

        foreach(GameObject col in currentCollisions)
        {
            // must check if gameobject is null to check if destroyed. (ActiveInHierarchy does not do this)
            if(col.gameObject != null)
            {
                float distance = (col.GetComponent<Rigidbody2D>().position - rb.position).magnitude;
                if(distance < closestEnemyDistance)
                {
                    closestEnemyDistance = distance;
                    closestEnemy = col;
                }
            }
            else
            {
                // When an enemy dies on impact, the trigger exit does not happen, so need to remove the collision here
                removalQueue.Add(col);
            }
        };

        if(closestEnemy != null)
        {
            reflectDash.action.canceled += ReflectDash;
            relfectDashtarget = closestEnemy;

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
        }
     
    }

    private void ReflectDash(InputAction.CallbackContext context)
    {
        // Once the dash has started, conditional dashes are allowed, but not basic dashes
        playerMovement.dash.action.Enable();
        playerMovement.DisableBasicDash();

        Vector2 direction = movement.action.ReadValue<Vector2>();

        // Teleport the player a small distance along the new direction vector, gives the sense they "bounced" off the enemy
        Vector2 teleportLocation = new Vector2(enemyPos.x, enemyPos.y) + direction * 1.3f;
        rb.position = teleportLocation;

        // Indicate to the player that they dashed within the window to add the speed they were going at impact
        if(playerImpact.actionWindowIsActive)
        {
            // GameObject animation = Instantiate(actionWindowIndicatorPrefab, transform.position, transform.rotation);
            // animation.transform.SetParent(transform, false);
            GetComponent<PlayerCooldowns>().EndAllCooldowns();
            GetComponent<PlayerAnimation>().PlayCooldownRefreshAnimation();
            //playerMovement.currSpeed += PlayerImpact.IMPACTSPEEDINCREASE;
        }

        //float reboundDashSpeed = playerMovement.dashSpeed; //+ playerMovement.currSpeed

        reflectDash.action.canceled -= ReflectDash;

        if(relfectDashtarget != null)
        {
            relfectDashtarget.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        }
        
        StartCoroutine(PerformDash(direction* reflectDashSpeed, reflectDashTime));

        Destroy(reflectDashArrow);
    }

    private void CancelOtherMovement()
    {
        playerMovement.EndDash();
        gameObject.GetComponent<PlayerGrapple>().EndGrapple();
        playerMovement.CanMove = false;
        playerMovement.dash.action.Disable();
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
    }

    IEnumerator PerformDash(Vector2 force, float time)
    {

        rb.AddForce(force, ForceMode2D.Impulse);

        // // rotate player
        // float playerRadValue = Mathf.Atan2(rb.velocity.y, rb.velocity.x);
        // float playerAngle= playerRadValue * (180/Mathf.PI);
        // rb.transform.localRotation = Quaternion.Euler(0,0,playerAngle -90);

        yield return new WaitForSeconds(time);

        ResetDashStatus();
       
    }
}
