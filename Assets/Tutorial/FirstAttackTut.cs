using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstAttackTut : MonoBehaviour
{
    [SerializeField] Castle1Tut tuorialScript;
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "AttackHitbox")
        {
            tuorialScript.CheckFirstEnemyAttacked();   
        }
    }
}
