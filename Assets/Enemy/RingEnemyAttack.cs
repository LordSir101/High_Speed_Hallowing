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
    private TH_Ring attackScript;

    [SerializeField]
    private GameObject attackType;

    public Animator animator;

    

    void Start()
    {
        attackStats = gameObject.GetComponent<EnemyTelegraphAttack>();
        animator = gameObject.GetComponent<Animator>();

        GameObject attack = Instantiate(attackType);
        
        attack.transform.position  = transform.position;
        attack.transform.parent = gameObject.transform;
        attack.transform.parent = transform; // make the attack a child of enemy 

        attackScript = attack.GetComponent<TH_Ring>();

        // the telegraph starts at 70% of the total hitbox since the hitbox is a ring.
        attackScript.Init(attackStats.windupTime, attackStats.activeTime, attackStats.cooldownTime, attackStats.Size, attackStats.Damage, attackStats.StartingTelegaphPercentSize);

        StartCoroutine(attackScript.StartCooldown());

    }

    void Update()
    {
        if(attackScript.attackReady)
        {
            attackScript.StartAttack();
            
            //ToggleAttackReady(false);
            //StartCoroutine(attackScript.StartCooldown(ToggleAttackReady));
        }
        if(attackScript.startAnimation)
        {
            animator.SetTrigger("Attack");
            attackScript.startAnimation = false;
        }
        // if(attackScript.attackEnded)
        // {
        //     //StartCoroutine(attackScript.StartCooldown());
        //     //attackScript.attackEnded = false;
        // }
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
