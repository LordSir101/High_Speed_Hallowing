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

    private float dashSpeed = 50f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentCollisions = new List <GameObject> ();
    }

    // Update is called once per frame
    void Update()
    {
        if(movement.action.enabled) 
        {
            Vector2 movementInput = movement.action.ReadValue<Vector2>();
            rb.velocity = movementInput;
        }

    }


    private void OnEnable()
    {
        dash.action.performed += Dash;
    }

    void OnTriggerEnter2D (Collider2D col) {
		// Add the GameObject collided with to the list.
        if(col.gameObject.tag == "Enemy")
        {
            currentCollisions.Add(col.gameObject);
        }
		
		
	}

	void OnTriggerExit2D (Collider2D col) {

		// Remove the GameObject collided with from the list.
		currentCollisions.Remove(col.gameObject);
	}

    private void Dash(InputAction.CallbackContext context)
    {
        Vector2 direction = movement.action.ReadValue<Vector2>();

        if(currentCollisions.Count > 0)
        {
            ReflectDash(direction);
        }
        else
        {
            rb.AddForce(direction * dashSpeed, ForceMode2D.Impulse);
        }

        StartCoroutine(DisableMovement());
    }

    // TODO: hold down dash button to charge a stronger dash
    private void ReflectDash(Vector2 direction)
    {
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
        Vector3 enemyPos = closestEnemy.transform.position;

        // Teleport the player a small distance along the new direction vector
        Vector2 teleportLocation = new Vector2(enemyPos.x, enemyPos.y) + direction;
        rb.position = teleportLocation;

        float reboundDashSpeed = dashSpeed * 2;
        rb.AddForce(direction * reboundDashSpeed, ForceMode2D.Impulse);
    }

    IEnumerator DisableMovement()
    {
        movement.action.Disable();
        yield return new WaitForSeconds(0.5f);
        movement.action.Enable();
    }

    
}
