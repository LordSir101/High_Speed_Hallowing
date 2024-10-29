using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Grapple : MonoBehaviour
{
    [SerializeField]
    private InputActionReference grapple, pointerPos, movement;
    private LineRenderer lineRenderer;
    Rigidbody2D rb;

    private float maxGrappleDistance = 6;
    private float initialGrappleSpeed = 1.5f;
    private float grappleAcceleration = 1.5f;
    private bool grappling = false;
    private Vector2 grappleLocation;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
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
        grapple.action.performed += StartGrapple;
    }

    private void StartGrapple(InputAction.CallbackContext context)
    {
        movement.action.Disable();

        Vector2 grappleDirection = GetGrappleDirection();

        // project settings -> physics2D -> quries start in colliders unchecked so raycast does not detect origin
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
        

        while (grappling)
        {
            // Temp fix for bug where movement does not get disabled if a move input is pressed while being disabled
            if(movement.action.enabled)
            {
                movement.action.Disable();
                rb.velocity = (grappleLocation - rb.position).normalized * initialGrappleSpeed;
            }

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
