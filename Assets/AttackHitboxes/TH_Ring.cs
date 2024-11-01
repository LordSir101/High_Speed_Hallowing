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
        gameObject.transform.GetChild(0).localScale = new Vector3(StartingTelegaphPercentSize, StartingTelegaphPercentSize, 0);
    }

    public override IEnumerator ActiveAttackTime()
    {
        yield return new WaitForSeconds(ActiveTime);
        EndAttack();
    }

}
