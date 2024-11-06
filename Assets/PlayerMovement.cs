using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField]
    private InputActionReference movement, dash;

    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private GameObject actionWindowIndicatorPrefab;
    private GameObject reflectDashArrow = null;

    Rigidbody2D rb;
    List<GameObject> currentCollisions;
    List<GameObject> removalQueue;
    PlayerImpact playerImpact;

    Vector3 enemyPos;

    private float dashSpeed = 50f;
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

    private void Dash(InputAction.CallbackContext context)
    {
        Vector2 direction = movement.action.ReadValue<Vector2>();

        if(currentCollisions.Count > 0)
        {
            ReflectDashSetup();
            //ReflectDash(direction);
        }
        else
        {
            rb.AddForce(direction * (dashSpeed + rb.velocity.magnitude), ForceMode2D.Impulse);
        }
    }

    private void ReflectDashSetup()
    {
        dash.action.canceled += ReflectDash;
        SetMovementAbility(false);

        Vector2 direction = movement.action.ReadValue<Vector2>();
        GameObject closestEnemy = null;

        float closestEnemyDistance = float.MaxValue;

        foreach(GameObject col in currentCollisions)
        {
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

        // get direction vector relative to enemy
        enemyPos = closestEnemy.transform.position;

        // Teleport the player to the enemy center
        Vector2 teleportLocation = new Vector2(enemyPos.x, enemyPos.y);
        rb.position = teleportLocation;

        reflectDashArrow = Instantiate(arrowPrefab, new Vector3(rb.position.x, rb.position.y, 0), transform.rotation);
    }

    private void ReflectDash(InputAction.CallbackContext context)
    {
        Vector2 direction = movement.action.ReadValue<Vector2>();

        // Teleport the player a small distance along the new direction vector
        Vector2 teleportLocation = new Vector2(enemyPos.x, enemyPos.y) + direction;
        rb.position = teleportLocation;

        // Indicate to the player that they dashed withtin the windo to accumulate speed
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


    // Since the dash reads from movement input, this allows turning on/off basic movement but allowing dashes
    public void SetMovementAbility(bool isActive)
    {
        canMove = isActive;
    }

    
}
