using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTelegraphAttack : MonoBehaviour
{
    [SerializeField]
    public float windupTime, activeTime, cooldownTime, Size, StartingTelegaphPercentSize;

    [SerializeField]
    public int ringAttackDamage = 10;

    public bool attackReady = false;
    // Start is called before the first frame update
}
