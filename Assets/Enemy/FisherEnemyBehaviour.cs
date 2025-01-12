using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FisherEnemyBehaviour : MonoBehaviour
{
    private float speed;
    [SerializeField] private float minSpeed, maxSpeed;
    [SerializeField] private LayerMask enemyLayer;

    [SerializeField]
    Rigidbody2D rb;

    [SerializeField]
    LayerMask playerLayer;

    [SerializeField] EnemyInfo enemyInfo;

    private EnemySounds audioController;

    [Header("Fishing Hook")]
    [SerializeField] Sprite hookSprite;
    [SerializeField] float attackRange, projSpeed, hookCooldown;
    [SerializeField] int hookDamage;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] GameObject hookAudioParent;
    [SerializeField] Transform hookStartingPos;
    [SerializeField] SpriteRenderer hookRenderer;
    [SerializeField] SpriteRenderer fishLineRenderer;
    [SerializeField] Animator animator;
    GameObject currHook;
    private float hookTimer;
    private float hookDamageMod;


    EnemyTelegraphAttack attackInfo;
    private FisherEnemyAttack coneAttack;

    GameObject player;

    private float maxConeAttackRange;
    //private float minConeAttackRange;
    

    private float reactiontime = 0.5f;

    public bool playerInRange = false;
    private bool isFishing = false;

    List<Vector3> possibleDirections;
    Vector2 targetDir;
    private float maxDis;
    private float minDis;

    void Start()
    {
        hookDamageMod = GetComponent<EnemyInfo>().damageMod;
        speed = UnityEngine.Random.Range(minSpeed, maxSpeed);

        player = GameObject.FindGameObjectWithTag("Player");

        attackInfo = gameObject.GetComponent<EnemyTelegraphAttack>();
        coneAttack = gameObject.GetComponent<FisherEnemyAttack>();
        //audioController = gameObject.GetComponentInChildren<EnemySounds>();

        // Size is the local scale
        maxConeAttackRange = attackInfo.Size;
        //minConeAttackRange = maxConeAttackRange * attackInfo.StartingTelegaphPercentSize;

        minDis = 0.5f;//maxConeAttackRange;//UnityEngine.Random.Range(1, maxConeAttackRange /2);
        maxDis = 4;//maxConeAttackRange;//UnityEngine.Random.Range(attackRange+1, attackRange + 3);
        
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
        Collider2D playerObjInRange = Physics2D.OverlapCircle(transform.position, maxConeAttackRange - 1, playerLayer);

        if (playerObjInRange != null)
        {
            playerInRange = true;
        }
        else
        {
            playerInRange = false;
        }

        hookTimer += Time.deltaTime;
        if(hookTimer >= hookCooldown)
        {
            if(!coneAttack.attacking)
            {
                AnimateCast();
                SpawnHook();
                hookTimer = 0;
            }
            // SpawnHook();
            // hookTimer = 0;
        }

    }

    void FixedUpdate()
    {
        if(!isFishing)
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

    private void AnimateCast()
    {
        animator.SetTrigger("Cast");
    }
    private void SpawnHook()
    {
        isFishing = true;
        //Transform throwingHand;
        Vector3 distanceToPlayer =  player.transform.position - hookStartingPos.position;
        // if(distanceToPlayer.x > 0)
        // {
        //     // right hand
        //     throwingHand = transform.GetChild(0).GetChild(1).GetChild(1);
        // }
        // else{
        //     // left hand
        //     throwingHand = transform.GetChild(0).GetChild(1).GetChild(0);
        // }

        currHook = Instantiate(projectilePrefab, hookStartingPos.position, transform.rotation);
        currHook.GetComponent<Projectile>().Init(hookDamage, hookDamageMod, hookSprite, 0.7f, 1f, 1, ReelHook);
        currHook.GetComponent<Rigidbody2D>().velocity = distanceToPlayer.normalized * projSpeed;
        currHook.GetComponent<TrailRenderer>().enabled = false;
        hookStartingPos.GetComponent<LineRenderer>().enabled = true;

        StartCoroutine(AnimateReel());

        //hookAudioParent.GetComponent<AudioSource>().Play();
    }

    private void ReelHook(Collision2D other)
    {

        // check if the hook hit something.
        if(other != null)
        {
            if(other.gameObject.tag == "Player")
            {
                Rigidbody2D playerRb = other.gameObject.GetComponent<Rigidbody2D>();

                // cancel grapple and movement
                playerRb.GetComponent<PlayerGrapple>().EndGrapple();
                player.GetComponent<PlayerMovement>().CanMove = false;
                player.GetComponentInChildren<PlayerAttack>().enabled = false;

                StartCoroutine(Reel());
            }
            else
            {
                isFishing = false;
            }
        }
        else
        {
            isFishing = false;
        }
       
    }

    private IEnumerator Reel()
    {
        // Vector3 posDiff = player.transform.position - transform.position;
        // Vector3 target = transform.position + posDiff.normalized;

        // float frames = 0;
        // float totalFrames = 45f;

        //Vector3 start = gameObject.transform.position;
        Collider2D touchingPlayer = Physics2D.OverlapCircle(transform.position, 0.05f, playerLayer);

        while(touchingPlayer == null)
        {
            Vector3 diff = transform.position - player.transform.position;
            player.transform.position += diff.normalized * 0.1f;
            yield return null;

            touchingPlayer = Physics2D.OverlapCircle(transform.position, 1f, playerLayer);
        }

        player.GetComponent<PlayerMovement>().CanMove = true;
        player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        isFishing = false;
        player.GetComponentInChildren<PlayerAttack>().enabled = true;
    }

    IEnumerator AnimateReel()
    {
        LineRenderer lineRenderer = hookStartingPos.GetComponent<LineRenderer>();
        hookRenderer.enabled = false;
        fishLineRenderer.enabled = false;

        while(isFishing)
        {
            // animate the grapple hook while actively grappling
            //Vector3 start = new Vector3(gameObject.transform.position.x,gameObject.transform.position.y,0);
            Vector3 target;
            //lineRenderer.enabled = true;
            if(currHook != null)
            {
                target = currHook.transform.position;
            }
            else
            {
                target = player.transform.position;
            }
            lineRenderer.SetPosition(0, hookStartingPos.position);
            lineRenderer.SetPosition(1, target);

            yield return null;

        }

        lineRenderer.enabled = false;
        hookRenderer.enabled = true;
        fishLineRenderer.enabled = true;
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
        float offset;

        if(enemyInfo.isFrenzy)
        {
            offset = (dirToPlayer.magnitude - minDis + 10) / (maxDis + 10 - minDis + 10);
            offset = Mathf.Clamp(offset, 0, 1);
        }
        else
        {
             offset = (dirToPlayer.magnitude - minDis) / (maxDis - minDis);
            offset = Mathf.Clamp(offset, 0, 1);
        }
       
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
                // RaycastHit2D hitTarget = Physics2D.Raycast(transform.position, dir, distance: 2f);

                // if(hitTarget)
                // {
                //     continue;
                // }

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
        // the weight towards enemies is higher for ice ghosts since they clump up alot
        return highest * 1.5f;
       
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
