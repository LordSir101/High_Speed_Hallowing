using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField]
    private InputActionReference movement, dash;

    Rigidbody2D rb;
    List<GameObject> currentCollisions;
    PlayerImpact playerImpact;

    Vector3 enemyPos;

    private float dashSpeed = 50f;
    private float baseMoveSpeed = 1;

    private bool canMove = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentCollisions = new List <GameObject> ();
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

    }

    private void OnEnable()
    {
        dash.action.performed += Dash;
        
    }

    void OnTriggerEnter2D (Collider2D col) 
    {

		// You can only reflect dash off of enemies
        if(col.gameObject.tag == "Enemy")
        {
            currentCollisions.Add(col.gameObject);
        }
	}

	void OnTriggerExit2D (Collider2D col) 
    {
		currentCollisions.Remove(col.gameObject);
	}

    private void Dash(InputAction.CallbackContext context)
    {
        Vector2 direction = movement.action.ReadValue<Vector2>();

        if(currentCollisions.Count > 0)
        {
            dash.action.canceled += ReflectDash;
            ReflectDashSetup();
            //ReflectDash(direction);
        }
        else
        {
            rb.AddForce(direction * (dashSpeed + rb.velocity.magnitude), ForceMode2D.Impulse);
        }

        // if(currentCollisions.Count < 0)
        // {
        //     rb.AddForce(direction * (dashSpeed + rb.velocity.magnitude), ForceMode2D.Impulse);
        // }

        //StartCoroutine(DisableMovement());
    }

    private void ReflectDashSetup()
    {
        SetMovementAbility(false);
        Vector2 direction = movement.action.ReadValue<Vector2>();
        GameObject closestEnemy = null;
        float closestEnemyDistance = float.MaxValue;

        foreach(GameObject col in currentCollisions)
        {
            float distance = (col.GetComponent<Rigidbody2D>().position - rb.position).magnitude;
            if(distance < closestEnemyDistance)
            {
                closestEnemyDistance = distance;
                closestEnemy = col;
            }
        };

        // get direction vector relative to enemy
        enemyPos = closestEnemy.transform.position;

        // Teleport the player to the enemy center
        Vector2 teleportLocation = new Vector2(enemyPos.x, enemyPos.y);
        rb.position = teleportLocation;
    }

    // TODO: Add visual indicator for dash direction
    private void ReflectDash(InputAction.CallbackContext context)
    {
        Vector2 direction = movement.action.ReadValue<Vector2>();

        // Teleport the player a small distance along the new direction vector
        Vector2 teleportLocation = new Vector2(enemyPos.x, enemyPos.y) + direction;
        rb.position = teleportLocation;

        float reboundDashSpeed = dashSpeed * 2 + playerImpact.ImpactSpeed;
        rb.AddForce(direction * reboundDashSpeed, ForceMode2D.Impulse);

        dash.action.canceled -= ReflectDash;
        SetMovementAbility(true);
    }


    // Since the dash reads from movement input, this allows turning on/off basic movement but allowing dashes
    public void SetMovementAbility(bool isActive)
    {
        canMove = isActive;
    }

    
}
