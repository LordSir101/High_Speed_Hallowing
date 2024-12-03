using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class RingEnemyBehaviour : MonoBehaviour
{
    
    [SerializeField] private float minSpeed, maxSpeed;
    [SerializeField] private LayerMask enemyLayer;

    private float speed;

    [SerializeField]
    Rigidbody2D enemyRb;

    EnemyTelegraphAttack ringAttackInfo;

    GameObject player;

    private float maxAttackRange;
    private float minAttackRange;

    private float reactiontime = 0.1f;

    List<Vector3> possibleDirections;
    Vector2 targetDir;
    float maxDis = 3;
    float minDis = 1;
    // distance enemy will strafe at when attack is ready
    float minAttackDis;
    float maxAttackDis;
    // distance from attack range enemy will strafe at when attack is on cooldown (enemy will stay further back)
    float minKiteRange = 1;
    float maxKiteRange = 1;
    //Vector2 patrolPoint;

    void Start()
    {
        
        speed = UnityEngine.Random.Range(minSpeed, maxSpeed);

        player = GameObject.FindGameObjectWithTag("Player");

        ringAttackInfo = gameObject.GetComponent<EnemyTelegraphAttack>();
        // Size is the local scale of ring attack. divide by 2 for radius
        maxAttackRange = ringAttackInfo.Size / 2;
        minAttackRange = maxAttackRange * ringAttackInfo.StartingTelegaphPercentSize;

        // Enemy starts running away from player when they get into this range
        minAttackDis = UnityEngine.Random.Range(1, minAttackRange);

        float disToMinAttack = minAttackRange - minDis;
        float disToMaxAttack = maxAttackRange - minDis;

        // Enemy will strafe in a circle when it is halfway between min and max distance
        float lowestMaxDis = minAttackRange * 2 - disToMinAttack; // lowest max that will make enemy strafe within its attack range
        float highestMaxDis = maxAttackRange * 2 - disToMaxAttack; // highest max that will make enemy strafe within its attack range

        // some enemies will strafe just outside/inside thier attack range. subtract and add a small buffer to max dis
        maxAttackDis = UnityEngine.Random.Range(lowestMaxDis - 0.5f, highestMaxDis + 0.5f);
        

        // get all directions around the enemy at 22.5 degree intervals
        possibleDirections = new List<Vector3>();
        for(float angle = 0; angle <= 360; angle += 22.5f)
        {
            Vector2 start = new Vector2(1, 0);
            Quaternion quat = Quaternion.AngleAxis(angle, Vector3.forward);
            possibleDirections.Add(quat * start);
        }

        // set inital target straight towards player
        targetDir = player.transform.position - transform.position;
        StartCoroutine(Move());
    }


    void FixedUpdate()
    {
        Vector2 targetVel = targetDir * speed;

        // get difference betwen current velocity and velocity we want to accelerate to
        //targetVel = Vector2.Lerp(rb.velocity, targetVel, 0.5f);
        Vector2 diff = targetVel - enemyRb.velocity;
        // rate of change in speed. set accel and deccel = to base move speed for instant movement
        float accelRate = Mathf.Abs(targetVel.magnitude) > 0.01f ? 5.5f : 5.5f;

        float newSpeed = Mathf.Pow(diff.magnitude * accelRate, 0.9f);

        // add force gives us slightly laggy movement
        enemyRb.AddForce(targetDir * newSpeed);
        //enemyRb.velocity = targetDir * speed;
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

        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(transform.position, 2f, enemyLayer);
       
        foreach(Vector2 dir in possibleDirections)
        {
            // subract offset from normalized dot product so enemies start moving to the side the close they get to the player
            // dot products closer to offset will be weighted higher.
            float dot = Vector2.Dot(dirToPlayer.normalized, dir.normalized);
            float normalized = (dot + 1) / 2; // normalize between 0 and 1
            float weight = 1 - Math.Abs(normalized - offset);
            
            float weightTowardsEnemies = GetHighestWeightOfVectorTowardsEnemies(dir, nearbyEnemies);
            // if a dir brings us closer to an enemy, weigh it less to avoid clumping of enemies
            weight -= weightTowardsEnemies;
            
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

    float GetHighestWeightOfVectorTowardsEnemies(Vector2 dir, Collider2D[] nearbyEnemies)
    {
        float highest = -2f;
        foreach(Collider2D col in nearbyEnemies)
        {
            Vector3 dirToEnemy = col.transform.position - transform.position;
            // float offset = (dirToEnemy.magnitude - minDis) / (maxDis - minDis);
            // offset = Mathf.Clamp(offset, 0, 1);
            // offset = 1-offset; // 0 when close 1 when far

            float dot = Vector2.Dot(dirToEnemy.normalized, dir.normalized);
            float normalized = (dot + 1) / 2; // normalize between 0 and 1
            float weight = normalized;

            if(weight > highest)
            {
                highest = weight;
            }

        }

        // the highest weight tells us the closest this vector will move us towards another enemy
        // we only care about the highest weight for each direction
        return highest;
       
    }

    public void ChangeState(string state)
    {
        if(state == "attacking")
        {
            minDis = minAttackDis;
            maxDis = maxAttackDis;
        }
        // enemy moves back when ability on cooldown
        else if(state == "kiting")
        {
            minDis = minAttackDis + minKiteRange;
            maxDis = maxAttackDis + maxKiteRange;
        }
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

    //     // Gizmos.color = Color.red;
    //     // Gizmos.DrawWireSphere(player.transform.position, minDis);
    //     // Gizmos.color = Color.green;
    //     // Gizmos.DrawWireSphere(player.transform.position, maxDis);
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
