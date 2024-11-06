using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerImpact : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Shake cameraShake;

    public float ImpactSpeed { get; set; } = 0f;

    
    private float impactActionWindow = 0.5f;
    private float impactActionTimer = 0f;
    private bool actionWindowIsActive = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (actionWindowIsActive)
        {
            impactActionTimer += Time.deltaTime;

            if(impactActionTimer >= impactActionWindow)
            {
                ResetImpactSpeed();
                actionWindowIsActive = false;
            }
        }
        
    }

    // stop player's velecoity on impact but allows them to move through enemies still
    // TODO: add visual indicator when ur speed is saved.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            StopCoroutine(cameraShake.Shaking());
            StartCoroutine(cameraShake.Shaking());
            ImpactSpeed += rb.velocity.magnitude;
            //rb.velocity = Vector2.zero;
            rb.velocity = rb.velocity.normalized;
            // Window to input a dash to accumulate speed based on how fast the player was going at impact
            impactActionTimer = 0f;
            actionWindowIsActive = true;
            //StartCoroutine(ResetImpactSpeed());
        }
    }

    private void ResetImpactSpeed()
    {
        ImpactSpeed = 0;
    }
}
