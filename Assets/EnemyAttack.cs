using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField]
    private GameObject attackType;

    [SerializeField]
    private float attackCooldown;

    public Boolean isAttacking {get; set;}


    private float cooldownTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        GameObject attack = Instantiate(attackType);
        attack.transform.position  = transform.position;

        attack.GetComponent<TH_Ring>().Init( 1.5f, 0.5f, 3, 0.7f, 2.5f);
    }

    // Update is called once per frame
    void Update()
    {
        // cooldownTimer += Time.deltaTime;

        // if(cooldownTimer >= attackCooldown)
        // {
            

        //     cooldownTimer = 0f;
        // }
    }
}
