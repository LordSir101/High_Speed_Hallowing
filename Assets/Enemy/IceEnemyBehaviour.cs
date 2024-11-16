using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class IceEnemyBehaviour : MonoBehaviour
{
    private float speed;
    [SerializeField] private float minSpeed, maxSpeed;

    [SerializeField]
    Rigidbody2D rb;

    [SerializeField]
    LayerMask playerLayer;

    [Header("snowball")]
    [SerializeField] Sprite snowballSprite;
    [SerializeField] float attackRange, projSpeed, kiteDistance, snowballCooldown;
    [SerializeField] int snowballDamage;
    [SerializeField] GameObject projectilePrefab;
    private float snowballTimer;


    EnemyTelegraphAttack attackInfo;
    private IceEnemyConeAttack coneAttack;

    GameObject player;

    private float maxConeAttackRange;
    private float minConeAttackRange;
    

    private float reactiontime = 0.5f;

    public bool playerInRange = false;

    void Start()
    {
        speed = Random.Range(minSpeed, maxSpeed);

        player = GameObject.FindGameObjectWithTag("Player");

        attackInfo = gameObject.GetComponent<EnemyTelegraphAttack>();
        coneAttack = gameObject.GetComponent<IceEnemyConeAttack>();

        // Size is the local scale
        maxConeAttackRange = attackInfo.Size;
        minConeAttackRange = maxConeAttackRange * attackInfo.StartingTelegaphPercentSize;

        StartCoroutine(Move());
    }
    void Update()
    {
        Collider2D playerObjInRange = Physics2D.OverlapCircle(transform.position, maxConeAttackRange, playerLayer);

        if (playerObjInRange != null)
        {
            playerInRange = true;
        }
        else
        {
            playerInRange = false;
        }

        snowballTimer += Time.deltaTime;
        if(snowballTimer >= snowballCooldown)
        {
            if(!coneAttack.attacking)
            {
                SpawnSnowball();
            }
            snowballTimer = 0;
        }

    }

    private void SpawnSnowball()
    {
        Transform throwingHand;
        Vector3 distanceToPlayer =  player.transform.position - transform.position;
        if(distanceToPlayer.x > 0)
        {
            // right hand
            throwingHand = transform.GetChild(1).GetChild(1);
        }
        else{
            // left hand
            throwingHand = transform.GetChild(1).GetChild(0);
        }

        GameObject proj = Instantiate(projectilePrefab, throwingHand.transform.position, transform.rotation);
        proj.GetComponent<Projectile>().Init(snowballDamage, snowballSprite);
        proj.GetComponent<Rigidbody2D>().velocity = distanceToPlayer.normalized * projSpeed;
    }

    // move towards player until within ring hitbox
    IEnumerator Move()
    {
        while (true)
        {
            if(!coneAttack.attacking)
            {
                Vector3 distanceToPlayer =  player.transform.position - transform.position;

                // move until the player is 1 unit within attack range
                if(distanceToPlayer.magnitude > attackRange - 1)
                {
                    rb.velocity = distanceToPlayer.normalized * speed;
                }
                else if(distanceToPlayer.magnitude < attackRange - kiteDistance)
                {
                    rb.velocity = distanceToPlayer.normalized * speed * -1;
                }
                else
                {
                    rb.velocity = Vector3.zero;
                }

                // if(distanceToPlayer.magnitude > maxConeAttackRange)
                // {
                //     rb.velocity = distanceToPlayer.normalized * speed;
                // }
                // else if(distanceToPlayer.magnitude < minConeAttackRange)
                // {
                //     rb.velocity = distanceToPlayer.normalized * speed * -1;
                // }
                // else
                // {
                //     rb.velocity = Vector3.zero;
                // }
            }
            else{
                rb.velocity = Vector3.zero;
            }
            

            yield return new WaitForSeconds(reactiontime);

        }
    }
}
