using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class RingEnemyAttack : MonoBehaviour
{
    private EnemyTelegraphAttack attackStats;
    private RingEnemyBehaviour behaviourScript;
    private TH_Ring attackScript;

    [SerializeField]
    private GameObject attackType;

    public Animator animator;

    public float windupTimer = 0;

    private bool animationComplete = false;

    

    void Start()
    {
        attackStats = gameObject.GetComponent<EnemyTelegraphAttack>();
        animator = gameObject.GetComponentInChildren<Animator>();
        behaviourScript = gameObject.GetComponent<RingEnemyBehaviour>();

        GameObject attack = Instantiate(attackType);
        
        attack.transform.position  = transform.position;
        //attack.transform.parent = gameObject.transform;
        attack.transform.parent = transform; // make the attack a child of enemy 

        attackScript = attack.GetComponent<TH_Ring>();

        // the telegraph starts at 70% of the total hitbox since the hitbox is a ring.
        attackScript.Init(attackStats.windupTime, attackStats.activeTime, attackStats.cooldownTime, attackStats.Size, attackStats.Damage, attackStats.StartingTelegaphPercentSize, attackStats.animationStartPercent);

        float delay = UnityEngine.Random.Range(0,2);

        behaviourScript.ChangeState("kiting");
        StartCoroutine(AttackDelay(delay));

    }

    void Update()
    {
        if(attackScript.attackReady)
        {
            behaviourScript.ChangeState("attacking");
            attackScript.StartAttack();
            
            //ToggleAttackReady(false);
            //StartCoroutine(attackScript.StartCooldown(ToggleAttackReady));
        }

        // start animation slightly before active frames
        if(attackScript.windupProgress >= attackStats.animationStartPercent && !animationComplete)
        {
            // windupTimer += Time.deltaTime;

            // if(windupTimer / attackStats.windupTime >= 0.8)
            // {
            
                animator.SetTrigger("Attack");
                animationComplete = true;
                //attackScript.startAnimation = false;
                //windupTimer = 0;
                //attackScript.startAnimation = false;
            //}
            
        }
        if(attackScript.attackEnded)
        {
            behaviourScript.ChangeState("kiting");
            animationComplete = false;
            //StartCoroutine(attackScript.StartCooldown());
            //attackScript.attackEnded = false;
        }
    }

    IEnumerator AttackDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(attackScript.StartCooldown());
    }

    public void PlayAttackSound()
    {
        GetComponentInChildren<AudioSource>().Play();
    }

    // public void AnimateActiveFrames()
    // {
    //     animator.SetTrigger("Attack");
    // }

    // void ToggleAttackReady(bool isReady)
    // {
    //     attackStats.attackReady = isReady;
    // }
}
