using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] Shake cameraShake;
    [SerializeField] Rigidbody2D rb;
    private PlayerAnimation playerAnimation;
    private PlayerReflectDash playerReflectDash;
    private PlayerGrapple playerGrapple;
    private PlayerAudio playerAudio;

    public float DamageBonusPercent = 0;

    void Start()
    {
        playerAnimation = transform.parent.GetComponent<PlayerAnimation>();
        playerReflectDash = transform.parent.GetComponent<PlayerReflectDash>();
        playerGrapple = transform.parent.GetComponent<PlayerGrapple>();
        playerAudio = transform.parent.GetComponentInChildren<PlayerAudio>();
    }
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if(other.gameObject.tag == "Enemy" && this.enabled)
        {   
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
                playerAnimation.AttackAnimation(impact);
                playerGrapple.EndGrapple();
            }
            else{
                impact = rb.velocity;
                playerAnimation.AttackAnimation(rb.velocity);
            }

            other.gameObject.GetComponent<EnemyHealth>().DealDamage(impact, DamageBonusPercent);
            playerAudio.PlayAttackSound();

            cameraShake.StopShake();
            float duration = impact.magnitude * 0.07f;
            cameraShake.StartShake(duration);
            //pauseControl.HitPause(0.07f);
            //StartCoroutine(HitPause(other.gameObject, gameObject));
            
            
            
            //rb.velocity = rb.velocity.normalized;

            
        }
    }
}
