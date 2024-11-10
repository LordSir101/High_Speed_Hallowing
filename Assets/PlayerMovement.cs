using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
//using System.Numerics;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField]
    private InputActionReference movement, dash;

    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private GameObject actionWindowIndicatorPrefab;
    private GameObject reflectDashArrow = null;

    [Header("Wall Jump")]
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    private bool isOnWall = false;
    private Collider2D touchingWall;
    //private Vector3 prevCollisionNormal;

    Rigidbody2D rb;
    List<GameObject> currentCollisions;
    List<GameObject> removalQueue;
    PlayerImpact playerImpact;

    Vector3 enemyPos;

    private float dashSpeed = 50f;
    private float dashTime = 0.3f;
    private float baseMoveSpeed = 1;

    private bool canMove = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentCollisions = new List <GameObject> ();
        removalQueue = new List<GameObject> ();
        playerImpact= GetComponent<PlayerImpact> ();
    }

    void Update()
    {
        if(canMove) 
        {
            Vector2 movementInput = movement.action.ReadValue<Vector2>();

            // move using current speed, with a minimum of base move speed
            rb.velocity = rb.velocity.magnitude > baseMoveSpeed ? movementInput * rb.velocity.magnitude : movementInput * baseMoveSpeed ;
            float playerRadValue = Mathf.Atan2(rb.velocity.y, rb.velocity.x);
            float playerAngle= playerRadValue * (180/Mathf.PI);
            rb.transform.localRotation = Quaternion.Euler(0,0,playerAngle -90);
            
        }

        if(reflectDashArrow)
        {
            Vector2 dir = movement.action.ReadValue<Vector2>();
            float radvalue = Mathf.Atan2(dir.y, dir.x);
            float angle= radvalue * (180/Mathf.PI);
            reflectDashArrow.transform.localRotation = Quaternion.Euler(0,0,angle -90);

            reflectDashArrow.transform.position = transform.position;
        }

        RemoveNullCollisions();

    }

    private void RemoveNullCollisions()
    {
        foreach(GameObject obj in removalQueue)
        {
            currentCollisions.Remove(obj);
        }
        removalQueue.Clear();
    }

    private void OnEnable()
    {
        dash.action.performed += Dash;
        
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        // foreach (var contact in collision.contacts)
        // {
        //     Debug.DrawRay(contact.point, contact.normal, Color.red, 2f);

        //     Debug.Log("Hit normal: " + contact.normal);
        // }

        //TODO: use if want to add angled walls
        //prevCollisionNormal = collision.GetContact(0).normal;
    }

    private void Dash(InputAction.CallbackContext context)
    {
        SetMovementAbility(false);

        touchingWall = Physics2D.OverlapCircle(wallCheck.position, 0.6f, wallLayer);
        Vector2 direction = movement.action.ReadValue<Vector2>();

        if(touchingWall && direction != Vector2.zero)
        {
            BoxCollider2D wall = touchingWall.gameObject.GetComponent<BoxCollider2D>();
            
            Vector3 wallSize = wall.bounds.size;
            Vector3 wallPos = touchingWall.transform.position;
            Vector3 dir;
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
            
            if(wallPos.x + wallSize.x/2 < transform.position.x)
            {
                Debug.Log(wallPos.x + wallSize.x/2);
                Debug.Log(transform.position.x);
                Debug.Log("wall left");
                dir = new Vector3(dashSpeed * 1.5f, direction.y * dashSpeed * 1.5f, 0);
                rb.AddForce(dir,  ForceMode2D.Impulse);
            }
            // wall is vertical to the right
            else if(wallPos.x - wallSize.x/2 > transform.position.x)
            {
                Debug.Log(wallPos.x + wallSize.x/2);
                Debug.Log(transform.position.x);
                Debug.Log("wall right");
                dir = new Vector3(-dashSpeed * 1.5f, direction.y * dashSpeed * 1.5f, 0);
                rb.AddForce(dir,  ForceMode2D.Impulse);
            }
            // wall on botom
            else if(wallPos.y + wallSize.y/2 < transform.position.y)
            {
                Debug.Log(wallPos.y + wallSize.y/2);
                Debug.Log(transform.position.y);
                Debug.Log("wall down");
                dir = new Vector3(direction.x * dashSpeed * 1.5f, dashSpeed * 1.5f, 0);
                rb.AddForce(dir,  ForceMode2D.Impulse);
            }
            // wall on top
            else if(wallPos.y - wallSize.y/2 > transform.position.y)
            {
                Debug.Log(wallPos.y - wallSize.y/2);
                Debug.Log(transform.position.y);
                Debug.Log("wall top");
                dir = new Vector3(direction.x * dashSpeed * 1.5f, -dashSpeed * 1.5f,0);
                rb.AddForce(dir,  ForceMode2D.Impulse);
            }

            //Vector3 diff = touchingWall.transform.position - transform.position;
            // Vector3 dir = diff.normalized * -1;
            // Debug.Log(dir);
            //rb.AddForce(dir * (dashSpeed * 1.5f + rb.velocity.magnitude), ForceMode2D.Impulse);
        }
        else if(currentCollisions.Count > 0)
        {
            ReflectDashSetup();
            //ReflectDash(direction);
        }
        else
        {
            rb.AddForce(direction * (dashSpeed + rb.velocity.magnitude), ForceMode2D.Impulse);
        }

        // turn player
        float playerRadValue = Mathf.Atan2(rb.velocity.y, rb.velocity.x);
        float playerAngle= playerRadValue * (180/Mathf.PI);
        rb.transform.localRotation = Quaternion.Euler(0,0,playerAngle -90);

        Invoke("EndDash", dashTime);
    }

    private void ReflectDashSetup()
    {
        
        //SetMovementAbility(false);

        Vector2 direction = movement.action.ReadValue<Vector2>();
        GameObject closestEnemy = null;

        float closestEnemyDistance = float.MaxValue;

        foreach(GameObject col in currentCollisions)
        {
            if(col.gameObject.activeInHierarchy)
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
            dash.action.canceled += ReflectDash;

            // get direction vector relative to enemy
            enemyPos = closestEnemy.transform.position;

            // Teleport the player to the enemy center
            Vector2 teleportLocation = new Vector2(enemyPos.x, enemyPos.y);
            rb.position = teleportLocation;

            reflectDashArrow = Instantiate(arrowPrefab, new Vector3(rb.position.x, rb.position.y, 0), transform.rotation);
        }
     
    }

    private void ReflectDash(InputAction.CallbackContext context)
    {
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

        float reboundDashSpeed = dashSpeed * 2 + playerImpact.ImpactSpeed;
        rb.AddForce(direction * reboundDashSpeed, ForceMode2D.Impulse);

        dash.action.canceled -= ReflectDash;
        SetMovementAbility(true);

        Destroy(reflectDashArrow);
    }

    private void EndDash()
    {
        canMove = true;
    }
    // Since the dash reads from movement input, this allows turning on/off basic movement but allowing dashes
    public void SetMovementAbility(bool isActive)
    {
        canMove = isActive;
    }

    
}
