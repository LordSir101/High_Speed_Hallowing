using System;
using System.Collections;
using UnityEngine;

public class PlayerImpact : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Shake cameraShake;
    [SerializeField] PauseControl pauseControl;

    //public float ImpactSpeed { get; set; } = 0f;
    public const float IMPACTSPEEDINCREASE = 2;
    public int DamageBonus = 0;

    
    private float impactActionWindow = 0.3f;
    private float impactActionTimer = 0f;
    public bool actionWindowIsActive = false;

    private PlayerAnimation playerAnimation;
    private PlayerReflectDash playerReflectDash;
    private PlayerGrapple playerGrapple;
    private PlayerAudio playerAudio;
    //private PlayerMovement playerMovement;

    void Start()
    {
        playerAnimation = GetComponent<PlayerAnimation>();
        playerReflectDash = GetComponent<PlayerReflectDash>();
        playerGrapple = GetComponent<PlayerGrapple>();
        playerAudio = GetComponentInChildren<PlayerAudio>();
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
            Vector2 impact;
            //Reflect dashes teleport and freeze the enemy, so we need to get the velocity before the dash for damage
            // Maybe make refelct dashes do no damage instead?
            if(playerReflectDash.reflectDashing)
            {
                impact = playerReflectDash.prevVelocity;
                playerAnimation.AttackAnimation(playerReflectDash.prevVelocity);
            }
            else if(playerGrapple.grappling)
            {
                impact = rb.velocity;
                playerGrapple.EndGrapple();
            }
            else{
                impact = rb.velocity;
                playerAnimation.AttackAnimation(rb.velocity);
            }

            other.gameObject.GetComponent<EnemyHealth>().DealDamage(impact, DamageBonus);
            playerAudio.PlayAttackSound();
            pauseControl.HitPause(0.07f);
            //StartCoroutine(HitPause(other.gameObject, gameObject));
            
            
            ResetActionWindow();

            //rb.velocity = rb.velocity.normalized;

            
        }
    }

    // IEnumerator HitPause(GameObject enemy, GameObject player)
    // {
    //     // check if enemy died during the hit
    //     Rigidbody2D enemyrb = null;
    //     Vector2 enemyOrigionalVel = Vector2.zero;

    //     if(enemy != null)
    //     {
    //         enemyrb = enemy.GetComponent<Rigidbody2D>();
    //         enemyOrigionalVel = enemyrb.velocity;
    //         enemyrb.velocity = Vector2.zero;
    //     }
       

    //     Rigidbody2D playerrb = player.GetComponent<Rigidbody2D>();
    //     Vector2 playerOrigionalVel = playerrb.velocity;
    //     playerrb.velocity = Vector2.zero;

    //     yield return new WaitForSeconds(1f);

    //     playerrb.velocity = playerOrigionalVel;
    //     if(enemyrb != null)
    //     {
    //         enemyrb.velocity = enemyOrigionalVel;
    //     }
        
    // }

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
