using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{

    [SerializeField]
    private InputActionReference movement, dash, grapple, pointerPos;
    private LineRenderer lineRenderer;

    Rigidbody2D rb;
    private float dashSpeed = 50f;
    private float maxGrappleDistance = 6;
    private float initialGrappleSpeed = 1.5f;
    private float grappleAcceleration = 1.5f;
    private bool grappling = false;
    private Vector2 grappleLocation;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(movement.action.enabled) 
        {
            Vector2 movementInput = movement.action.ReadValue<Vector2>();
            rb.velocity = movementInput;
        }

        UpdateGrapple();
    }

    private void UpdateGrapple()
    {
        if(grappling)
        {
            Vector3 start = new Vector3(gameObject.transform.position.x,gameObject.transform.position.y,0);

            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0,start);
            lineRenderer.SetPosition(1, grappleLocation);

            if(Vector2.Distance(rb.position, grappleLocation) <= 0.5)
            {
                grappling = false;
            }
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }

    private void OnEnable()
    {
        dash.action.performed += Dash;
        grapple.action.performed += Grapple;
    }

    private void Dash(InputAction.CallbackContext context)
    {
        Vector2 direction = movement.action.ReadValue<Vector2>();
        
        rb.AddForce(direction * dashSpeed, ForceMode2D.Impulse);

        StartCoroutine(DisableMovement());
    }

    IEnumerator DisableMovement()
    {
        movement.action.Disable();
        yield return new WaitForSeconds(0.5f);
        movement.action.Enable();
    }

    private void Grapple(InputAction.CallbackContext context)
    {
        Vector2 grappleDirection = GetGrappleDirection();

        RaycastHit2D hitTarget = Physics2D.Raycast(gameObject.transform.position, grappleDirection, distance: maxGrappleDistance);

        if(hitTarget)
        {
            grappleLocation = hitTarget.point;
            StartCoroutine(PerformGrapple());
        }
        else 
        {
            grappleLocation = rb.position + grappleDirection.normalized * maxGrappleDistance;
            StartCoroutine(PerformMissedGrapple());
        }

    }

    private Vector2 GetGrappleDirection()
    {
        Vector2 mousePos = pointerPos.action.ReadValue<Vector2>();
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector2 direction = mousePos - rb.position;

        return direction.normalized;
    }

    IEnumerator PerformGrapple()
    {
        grappling = true;

        rb.velocity = (grappleLocation - rb.position).normalized * initialGrappleSpeed;
        rb.drag = 0;
        movement.action.Disable();

        while (grappling)
        {
            rb.velocity = rb.velocity.magnitude * grappleAcceleration * rb.velocity.normalized;
            yield return new WaitForSeconds(0.1f);
        }

        rb.velocity = Vector2.zero;
        rb.drag = 3;
        movement.action.Enable();
    }

    IEnumerator PerformMissedGrapple()
    {
        grappling = true;

        yield return new WaitForSeconds(0.5f);

        grappling = false;
    }
}
