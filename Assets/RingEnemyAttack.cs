using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class RingEnemyAttack : MonoBehaviour
{
    [SerializeField]
    private GameObject attackType;

    [SerializeField]
    public float windupTime, activeTime, cooldownTime, Size, StartingTelegaphPercentSize;

    void Start()
    {
        GameObject attack = Instantiate(attackType);
        
        attack.transform.position  = transform.position;
        attack.transform.parent = gameObject.transform;

        // the telegraph starts at 70% of the total hitbox since the hitbox is a ring.
        attack.GetComponent<TH_Ring>().Init(windupTime, activeTime, cooldownTime, Size, StartingTelegaphPercentSize);
    }

    void Update()
    {

    }
}
