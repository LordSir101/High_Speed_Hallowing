using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceEnemyBehaviour : MonoBehaviour
{
    [SerializeField]
    private float speed;

    [SerializeField]
    Rigidbody2D rb;

    [SerializeField]
    LayerMask playerLayer;

    EnemyTelegraphAttack attackInfo;
    private IceEnemyConeAttack coneAttack;

    GameObject player;

    private float maxAttackRange;
    private float minAttackRange;

    private float reactiontime = 0.5f;

    public bool playerInRange = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        attackInfo = gameObject.GetComponent<EnemyTelegraphAttack>();
        coneAttack = gameObject.GetComponent<IceEnemyConeAttack>();

        // Size is the local scale
        maxAttackRange = attackInfo.Size;
        minAttackRange = maxAttackRange * attackInfo.StartingTelegaphPercentSize;

        StartCoroutine(Move());
    }
    void Update()
    {
        Collider2D playerObjInRange = Physics2D.OverlapCircle(transform.position, maxAttackRange, playerLayer);

        if (playerObjInRange != null)
        {
            playerInRange = true;
        }
        else
        {
            playerInRange = false;
        }
    }

    // move towards player until within ring hitbox
    IEnumerator Move()
    {
        while (true)
        {
            if(!coneAttack.attacking)
            {
                Vector3 distanceToPlayer =  player.transform.position - transform.position;

                if(distanceToPlayer.magnitude > maxAttackRange)
                {
                    rb.velocity = distanceToPlayer.normalized * speed;
                }
                else if(distanceToPlayer.magnitude < minAttackRange)
                {
                    rb.velocity = distanceToPlayer.normalized * speed * -1;
                }
                else
                {
                    rb.velocity = Vector3.zero;
                }
            }
            else{
                rb.velocity = Vector3.zero;
            }
            

            yield return new WaitForSeconds(reactiontime);

        }
    }
}
