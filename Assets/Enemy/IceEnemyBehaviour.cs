using System;
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

    private EnemySounds audioController;

    [Header("snowball")]
    [SerializeField] Sprite snowballSprite;
    [SerializeField] float attackRange, projSpeed, kiteDistance, snowballCooldown;
    [SerializeField] int snowballDamage;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] GameObject snowballAudioParent;
    private float snowballTimer;


    EnemyTelegraphAttack attackInfo;
    private IceEnemyConeAttack coneAttack;

    GameObject player;

    private float maxConeAttackRange;
    //private float minConeAttackRange;
    

    private float reactiontime = 0.5f;

    public bool playerInRange = false;

    List<Vector3> possibleDirections;
    Vector2 targetDir;
    private float maxDis;
    private float minDis;

    void Start()
    {
        speed = UnityEngine.Random.Range(minSpeed, maxSpeed);

        player = GameObject.FindGameObjectWithTag("Player");

        attackInfo = gameObject.GetComponent<EnemyTelegraphAttack>();
        coneAttack = gameObject.GetComponent<IceEnemyConeAttack>();
        audioController = gameObject.GetComponentInChildren<EnemySounds>();

        // Size is the local scale
        maxConeAttackRange = attackInfo.Size;
        //minConeAttackRange = maxConeAttackRange * attackInfo.StartingTelegaphPercentSize;

        minDis = maxConeAttackRange;//UnityEngine.Random.Range(1, maxConeAttackRange /2);
        maxDis = UnityEngine.Random.Range(attackRange+1, attackRange + 3);
        
        // get all directions around the enemy at 22.5 degree intervals
        possibleDirections = new List<Vector3>();
        for(float angle = 0; angle <= 360; angle += 22.5f)
        {
            Vector2 start = new Vector2(1, 0);
            Quaternion quat = Quaternion.AngleAxis(angle, Vector3.forward);
            possibleDirections.Add(quat * start);
        }

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

    void FixedUpdate()
    {
        if(!coneAttack.attacking)
        {
            Vector2 targetVel = targetDir * speed;
            Vector2 diff = targetVel - rb.velocity;

            float accelRate = Mathf.Abs(targetVel.magnitude) > 0.01f ? 5.5f : 5.5f;
            float newSpeed = Mathf.Pow(diff.magnitude * accelRate, 0.9f);
            
            rb.AddForce(targetDir * newSpeed);
        }
        else
        {
            rb.velocity = Vector3.zero;
        }

    }

    private void SpawnSnowball()
    {
        Transform throwingHand;
        Vector3 distanceToPlayer =  player.transform.position - transform.position;
        if(distanceToPlayer.x > 0)
        {
            // right hand
            throwingHand = transform.GetChild(0).GetChild(1).GetChild(1);
        }
        else{
            // left hand
            throwingHand = transform.GetChild(0).GetChild(1).GetChild(0);
        }

        GameObject proj = Instantiate(projectilePrefab, throwingHand.transform.position, transform.rotation);
        proj.GetComponent<Projectile>().Init(snowballDamage, snowballSprite);
        proj.GetComponent<Rigidbody2D>().velocity = distanceToPlayer.normalized * projSpeed;

        snowballAudioParent.GetComponent<AudioSource>().Play();
    }

    Vector2 GetBestDirection()
    {
        float bestWeight = -1;
        Vector3 bestDir = new Vector2(0,0);
        //float closestToCurrDirection = -1;

        Vector3 dirToPlayer = player.transform.position - transform.position;
        //Debug.Log(dirToPlayer.magnitude);

        // Change the preferred direction based on how far away enemy is
        // The further away the enemy is, the more weight a dot product of 1 will have and vice versa
        float offset = (dirToPlayer.magnitude - minDis) / (maxDis - minDis);
        offset = Mathf.Clamp(offset, 0, 1);
       
        foreach(Vector2 dir in possibleDirections)
        {
            // subract offset from normalized dot product so enemies start moving to the side the close they get to the player
            // dot products closer to offset will be weighted higher.
            float dot = Vector2.Dot(dirToPlayer.normalized, dir.normalized);
            float normalized = (dot + 1) / 2; // normalize between 0 and 1
            float weight = 1 - Math.Abs(normalized - offset);
            
            
            // Favor moving in the general direction we are already moving over slightly better weight in the opposite direction.
            // This prevents jittery movement when the enemy has two equal options to move.
            float dotWithCurrDirection = Vector2.Dot(targetDir, dir);
            normalized = (dotWithCurrDirection + 1) / 2;
            weight += normalized * 0.5f;
            
            if(weight > bestWeight)
            {
                //Check for obstruction
                RaycastHit2D hitTarget = Physics2D.Raycast(transform.position, dir, distance: 2f);

                if(hitTarget)
                {
                    continue;
                }

                bestWeight = weight;
                bestDir = dir;
            }
        
        }
        return bestDir;
    }

    // void OnDrawGizmos()
    // {
    //     foreach(Vector2 pos in possibleDirections)
    //     {
    //         Gizmos.color = Color.blue;
    //         if(pos == targetDir)
    //         {
    //             Gizmos.color = Color.green;
    //         }
            
    //         Gizmos.DrawLine(transform.position, new Vector3(pos.x, pos.y, 0) + transform.position);
    //     }

    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireSphere(player.transform.position, minDis);
    //     Gizmos.color = Color.green;
    //     Gizmos.DrawWireSphere(player.transform.position, maxDis);
    // }

    // move towards player until within ring hitbox
    IEnumerator Move()
    {
        while (true)
        {
            targetDir = GetBestDirection();
            
            yield return new WaitForSeconds(reactiontime);
        }
    }
}
