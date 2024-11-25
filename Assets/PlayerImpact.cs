using System;
using UnityEngine;

public class PlayerImpact : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Shake cameraShake;

    //public float ImpactSpeed { get; set; } = 0f;
    public const float IMPACTSPEEDINCREASE = 2;
    public int DamageBonus = 0;

    
    private float impactActionWindow = 0.3f;
    private float impactActionTimer = 0f;
    public bool actionWindowIsActive = false;

    private PlayerAnimation playerAnimation;
    private PlayerReflectDash playerReflectDash;
    private PlayerGrapple playerGrapple;
    //private PlayerMovement playerMovement;

    void Start()
    {
        playerAnimation = GetComponent<PlayerAnimation>();
        playerReflectDash = GetComponent<PlayerReflectDash>();
        playerGrapple = GetComponent<PlayerGrapple>();
        //playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (actionWindowIsActive)
        {
            impactActionTimer += Time.deltaTime;

            if(impactActionTimer >= impactActionWindow)
            {
                actionWindowIsActive = false;
            }
        }
        
    }

    // stop player's velecoity on impact but allows them to move through enemies still
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            StopCoroutine(cameraShake.Shaking());
            StartCoroutine(cameraShake.Shaking());
            
            //Deal damage
            int damage;
            //Reflect dashes teleport and freeze the enemy, so we need to get the velocity before the dash for damage
            // Maybe make refelct dashes do no damage instead?
            if(playerReflectDash.reflectDashing)
            {
                damage = (int) Math.Floor(playerReflectDash.prevVelocity.magnitude) + DamageBonus;
                playerAnimation.AttackAnimation(playerReflectDash.prevVelocity);
            }
            else if(playerGrapple.grappling)
            {
                damage = (int) Math.Floor(rb.velocity.magnitude) + DamageBonus;
                playerGrapple.EndGrapple();
            }
            else{
                damage = (int) Math.Floor(rb.velocity.magnitude) + DamageBonus;
                playerAnimation.AttackAnimation(rb.velocity);
            }
            other.gameObject.GetComponent<EnemyHealth>().DealDamage(damage);
            
            ResetActionWindow();

            //rb.velocity = rb.velocity.normalized;

            
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        
        ResetActionWindow();
        //rb.velocity = rb.velocity.normalized;

        if(gameObject.GetComponent<PlayerMovement>().wallJumpQueued)
        {
            TriggerWallJump();
        }
    }

    void OnCollisionStay2D(Collision2D collisionInfo)
    {
        if(gameObject.GetComponent<PlayerMovement>().wallJumpQueued)
        {
            TriggerWallJump();
        }
    }

    // Window to input a dash to accumulate speed based on how fast the player was going at impact
    private void ResetActionWindow()
    {
        impactActionTimer = 0f;
        actionWindowIsActive = true;
    }

    private void TriggerWallJump()
    {
        gameObject.GetComponent<PlayerMovement>().StartWallJump();
        gameObject.GetComponent<PlayerMovement>().wallJumpQueued = false;
    }
}
