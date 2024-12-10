// using System;
// using System.Collections;
// using System.Collections.Generic;
// using Unity.VisualScripting;
using System;
using Unity.VisualScripting;
using UnityEngine;

public class TH_Ring : TelegraphedHitbox
{
    private float damageMod;
    void Start()
    {
        // set size of mask to make the inner circle of the ring
        // component with mask has collider to determine safe area of ring
        gameObject.transform.GetChild(0).localScale = new Vector3(StartingTelegaphPercentSize, StartingTelegaphPercentSize, 0);
        damageMod = transform.parent.GetComponent<EnemyInfo>().damageMod;
    }
    // public override void Setup()
    // {
    //     // set size of mask to make the inner circle of the ring
    //     // component with mask has collider to determine safe area of ring
        
    // }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            //Debug.Log(transform.parent.GetComponent<EnemyInfo>().damageMod);
            // increase damage based on difficulty
            int damage = (int) Math.Ceiling(Damage * damageMod);
            other.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
            SetAllCollidersStatus(false);
        }
    }

}
