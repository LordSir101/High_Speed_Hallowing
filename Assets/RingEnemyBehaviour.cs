using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class RingEnemyBehaviour : MonoBehaviour
{
    [SerializeField]
    private float speed;

    [SerializeField]
    Rigidbody2D enemyRb;

    [SerializeField]
    RingEnemyAttack ringAttackInfo;

    GameObject player;

    private float maxAttackRange;
    private float minAttackRange;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        // Size is the local scale of ring attack. divide by 2 for radius
        maxAttackRange = ringAttackInfo.Size / 2;
        minAttackRange = maxAttackRange * ringAttackInfo.StartingTelegaphPercentSize;

        StartCoroutine(Move());
    }

    // move towards player until within ring hitbox
    IEnumerator Move()
    {
        while (true)
        {
            Vector3 distanceToPlayer =  player.transform.position - transform.position;

            if(distanceToPlayer.magnitude > maxAttackRange)
            {
                enemyRb.velocity = distanceToPlayer.normalized * speed;
            }
            else if(distanceToPlayer.magnitude < minAttackRange)
            {
                enemyRb.velocity = distanceToPlayer.normalized * speed * -1;
            }
            else
            {
                enemyRb.velocity = Vector3.zero;
            }

            yield return new WaitForSeconds(0.5f);

        }
    }
}
