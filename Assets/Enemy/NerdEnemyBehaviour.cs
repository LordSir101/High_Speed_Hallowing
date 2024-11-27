using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NerdEnemyBehaviour : MonoBehaviour
{
    private float speed;
    [SerializeField] private float minSpeed, maxSpeed, patrolTime;

    private float moveAngle;
    private float radius;
    private float reactionTime = 0.05f;

    Rigidbody2D rb;
    GameObject player;
    Vector2 startingPoint;
    private bool beginPatrol = false;
    Vector2 targetPoint;

    List<Vector3> possibleDirections;
    Vector2 targetDir;
    private float maxPatrolDis;
    private float minPatrolDis;
    private float minKiteDis; // the distancew the ghost will run straight away from the player
    private float maxKiteDis; // The distance where the ghost will ignore the player


    // Start is called before the first frame update
    void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        radius = 5;

        speed = UnityEngine.Random.Range(minSpeed, maxSpeed);

        minPatrolDis = 0;
        maxPatrolDis = UnityEngine.Random.Range(radius -1, radius + 1);

        minKiteDis = 1;
        maxKiteDis = 3;

        possibleDirections = new List<Vector3>();
        for(float angle = 0; angle <= 360; angle += 22.5f)
        {
            Vector2 start = new Vector2(1, 0);
            Quaternion quat = Quaternion.AngleAxis(angle, Vector3.forward);
            possibleDirections.Add(quat * start);
        }

        GetMovementCircle();
        StartCoroutine(ChangePatrol());
        StartCoroutine(Move());
    }

    // The nerd ghost moves in a circle around a point close to the player
    private void GetMovementCircle()
    {
        float range = 3f;
        float posX = UnityEngine.Random.Range(-range, range);
        float posY = UnityEngine.Random.Range(-range, range);

        targetPoint = new Vector2(player.transform.position.x + posX, player.transform.position.y + posY);

    }

    // Update is called once per frame
    void Update()
    {
        if((startingPoint - rb.position).magnitude < 0.5)
        {
            beginPatrol = true;
        }
        if(beginPatrol)
        {
            moveAngle += speed / radius * Mathf.PI * Time.deltaTime;
            //rb.transform.position = targetPoint  +  new Vector2(Mathf.Cos(moveAngle), Mathf.Sin(moveAngle)) * radius;
            Vector2 nextPos = targetPoint  +  new Vector2(Mathf.Cos(moveAngle), Mathf.Sin(moveAngle)) * radius;
            rb.velocity = (nextPos - rb.position) * speed;
            
        }
        
    }

    void FixedUpdate()
    {
        // Vector2 targetVel = targetDir * speed;
        // Vector2 diff = targetVel - rb.velocity;

        // float accelRate = Mathf.Abs(targetVel.magnitude) > 0.01f ? 5.5f : 5.5f;
        // float newSpeed = Mathf.Pow(diff.magnitude * accelRate, 0.9f);
        
        // rb.AddForce(targetDir * newSpeed);
        rb.velocity = targetDir * speed;
    }

    Vector2 GetBestDirection()
    {
        float bestWeight = -1;
        Vector3 bestDir = new Vector2(0,0);

        Vector3 dirToPlayer = player.transform.position - transform.position;
        Vector3 dirToTarget = new Vector3(targetPoint.x, targetPoint.y, 0) - transform.position;

        // offset for vector towards target
        // ghost will patrol around halfway between min and max
        float offset = (dirToTarget.magnitude - minPatrolDis) / (maxPatrolDis - minPatrolDis);
        offset = Mathf.Clamp(offset, 0, 1);


        // The further away the player is, the less weight we want for the vector that runs away from the player
        // Therefore, subtract a higher number from pweight the firther away we are.
        float playerOffset = (dirToPlayer.magnitude - minKiteDis) / (maxKiteDis - minKiteDis);
        playerOffset = Mathf.Clamp(playerOffset, 0, 1);
       
        foreach(Vector2 dir in possibleDirections)
        {
            // get dir's weight in relation to the player
            float pdot = Vector2.Dot(dirToPlayer.normalized, dir.normalized) * -1;
            float pnormalized = (pdot + 1) / 2; // normalize between 0 and 1
            float pweight = Mathf.Clamp(pnormalized - playerOffset, 0, 1);

            // get dir's weight in relation to the target
            float dot = Vector2.Dot(dirToTarget.normalized, dir.normalized);
            float normalized = (dot + 1) / 2; // normalize between 0 and 1
            float weight = 1 - Math.Abs(normalized - offset);
            
            // Favor moving in the general direction we are already moving over slightly better weight in the opposite direction.
            // This prevents jittery movement when the enemy has two equal options to move.
            float dotWithCurrDirection = Vector2.Dot(targetDir, dir);
            normalized = (dotWithCurrDirection + 1) / 2;
            float cweight = normalized * 0.5f;

            float totalWeight =  pweight + weight + cweight; //pweight +
            
            if(totalWeight > bestWeight)
            {
                //Check for obstruction
                RaycastHit2D hitTarget = Physics2D.Raycast(transform.position, dir, distance: 2f);

                if(hitTarget)
                {
                    continue;
                }

                bestWeight = totalWeight;
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
    //     Gizmos.DrawWireSphere(targetPoint, 1);
    //     Gizmos.color = Color.green;
    //     Gizmos.DrawWireSphere(player.transform.position, maxKiteDis);
    //     Gizmos.color = Color.yellow;
    //     Gizmos.DrawWireSphere(targetPoint, maxPatrolDis);
    // }

    IEnumerator Move()
    {
        while(true)
        {
            yield return new WaitForSeconds(reactionTime);
            targetDir = GetBestDirection();
        }
       
    }

    IEnumerator ChangePatrol()
    {       
        while(true)
        {
            yield return new WaitForSeconds(patrolTime);
            GetMovementCircle();
        }
    }
}
