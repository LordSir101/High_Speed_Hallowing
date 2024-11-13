using System;
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
    private float reflectDashSpeedModifier = 2f;
    public bool reflectDashing = false;
    //private float reflectDashSpeed = 4f;
    private float reflectDashTime = 0.3f;
    Vector3 enemyPos;

    List<GameObject> currentCollisions;
    List<GameObject> removalQueue;
    Rigidbody2D rb;

    PlayerMovement playerMovement;
    PlayerImpact playerImpact;
    

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentCollisions = new List <GameObject> ();
        removalQueue = new List<GameObject> ();
        playerMovement = gameObject.GetComponent<PlayerMovement>();
        playerImpact = gameObject.GetComponent<PlayerImpact>();
    }

    // Update is called once per frame
    void Update()
    {
        RemoveNullCollisions();
    }
    // void FixedUpdate()
    // {
        
    // }

    private void OnEnable()
    {
        reflectDash.action.performed += CheckReflectDash;
        
    }

    private void CheckReflectDash(InputAction.CallbackContext context)
    {
        if(currentCollisions.Count > 0)
            {
                ReflectDashSetup();
                //ReflectDash(direction);
            }
    }

    private void ReflectDashSetup()
    {
        
        //CanMove = false;
        
        playerMovement.EndDash();
        gameObject.GetComponent<PlayerGrapple>().EndGrapple();
        playerMovement.CanMove = false;
        playerMovement.dash.action.Disable();
        reflectDashing = true;

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

            // get direction vector relative to enemy
            enemyPos = relfectDashtarget.transform.position;

            // Teleport the player to the enemy center
            Vector2 teleportLocation = new Vector2(enemyPos.x, enemyPos.y);
            rb.position = teleportLocation;

            reflectDashArrow = Instantiate(arrowPrefab, new Vector3(rb.position.x, rb.position.y, 0), transform.rotation);
        }
        Debug.Log(playerMovement.CanMove);
     
    }

    private void ReflectDash(InputAction.CallbackContext context)
    {
        
        //playerMovement.DisableBasicDash();
        Vector2 direction = movement.action.ReadValue<Vector2>();

        // Teleport the player a small distance along the new direction vector, gives the sense they "bounced" off the enemy
        Vector2 teleportLocation = new Vector2(enemyPos.x, enemyPos.y) + direction;
        rb.position = teleportLocation;

        // Indicate to the player that they dashed within the window to add the speed they were going at impact
        if(playerImpact.ImpactSpeed > 0)
        {
            GameObject animation = Instantiate(actionWindowIndicatorPrefab, transform.position, transform.rotation);
            animation.transform.SetParent(transform, false);
        }

        float reboundDashSpeed = playerMovement.dashSpeed * reflectDashSpeedModifier + playerImpact.ImpactSpeed;
        Debug.Log("speed " + reboundDashSpeed);
        rb.AddForce(direction * reboundDashSpeed, ForceMode2D.Impulse);

        reflectDash.action.canceled -= ReflectDash;

        if(relfectDashtarget != null)
        {
            relfectDashtarget.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        }

        Debug.Log("dash " + rb.velocity.magnitude);
        

        //TODO: change this to the new version if needed
        //Invoke("EndDash", dashTime);
        StartCoroutine(EndDash(reflectDashTime));

        Destroy(reflectDashArrow);
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

    IEnumerator EndDash(float time)
    {
        yield return new WaitForSeconds(time);
        Debug.Log("ended");
        playerMovement.CanMove = true;
        playerMovement.dash.action.Enable();
        reflectDashing = false;
        //EnableBasicDash();
        //dash.action.Enable();
        
        //Debug.Log("ended");
       
        //isWallJumping = false;
        //Physics.IgnoreLayerCollision(8, 6, false);
    }
}
