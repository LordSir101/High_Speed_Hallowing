using System;
using System.Collections;
using UnityEngine;

public class PlayerImpact : MonoBehaviour
{
    
    
    [SerializeField] PauseControl pauseControl;

    //public float ImpactSpeed { get; set; } = 0f;
    public const float IMPACTSPEEDINCREASE = 2;
    

    
    private float impactActionWindow = 0.3f;
    private float impactActionTimer = 0f;
    public bool actionWindowIsActive = false;

    
    //private PlayerMovement playerMovement;

    void Start()
    {
        
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

    // reset the action window when the player hurtbox collides with an enemy. (for the right click mechanic)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            ResetActionWindow();
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
