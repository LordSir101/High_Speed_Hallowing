using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TH_Ring : TelegraphedHitbox
{
    public override void Setup()
    {
        // set size of mask to make the inner circle of the ring
        // component with mask has collider to determine safe area of ring
        gameObject.transform.GetChild(0).localScale = new Vector3(StartingTelegaphPercentSize, StartingTelegaphPercentSize, 0);
    }

    public override IEnumerator ActiveAttackTime()
    {
        SetAllCollidersStatus(true);

        yield return new WaitForSeconds(ActiveTime);
        
        SetAllCollidersStatus(false);
        EndAttack();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerHealth>().TakeDamage(Damage);
        }
    }

   

}
