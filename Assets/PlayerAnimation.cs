using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] GameObject shadow;
    [SerializeField] ParticleSystem particleSys;
    private Vector3 shadowInitialPosition;
    private PlayerMovement playerMovement;

    private Rigidbody2D rb;

    Animator animator;

    private bool attackAnimationStarted = false;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        shadowInitialPosition = shadow.transform.localPosition;
        playerMovement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        if(animator.GetCurrentAnimatorStateInfo(0).IsName("attack"))
        {
            attackAnimationStarted = true;
        }
        
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("idle"))
        {
            //animator.speed = playerMovement.currSpeed / 3 + 1;
            animator.speed = 1 + rb.velocity.magnitude;
        }

        // if state is idle and attacking is true, than the atack animation is finished
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("idle") && attackAnimationStarted)
        {
            rb.transform.localRotation = Quaternion.Euler(0,0,0);
            attackAnimationStarted = false;
        }
        
    }
    void LateUpdate ()
    {
        // keep the shadow at the bottom of the player
        shadow.transform.rotation = Quaternion.identity;
        shadow.transform.position = transform.position + shadowInitialPosition;
    }

    public void AttackAnimation(Vector2 velocity)
    {
        // rotate the player just for the attack
        float playerRadValue = Mathf.Atan2(velocity.y, velocity.x);
        float playerAngle= playerRadValue * (180/Mathf.PI);
        rb.transform.localRotation = Quaternion.Euler(0,0,playerAngle -90);

        animator.speed = 1;
        animator.SetTrigger("Attack");
        
    }

    public void PlayCooldownRefreshAnimation()
    {
        particleSys.Play();
    }
}
