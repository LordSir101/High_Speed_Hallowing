using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceEnemyConeAttack : MonoBehaviour
{
   
    private EnemyTelegraphAttack attackStats;
    private TH_Cone attackScript;
    private IceEnemyBehaviour behaviourScript;
    GameObject attackObj;

    [SerializeField]
    private GameObject attackType;
    GameObject player;

    public bool attacking = false;
    private bool animationComplete = false;
    [SerializeField] private GameObject snowConePrefab;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        attackStats = gameObject.GetComponent<EnemyTelegraphAttack>();

        attackObj = Instantiate(attackType);
        attackObj.transform.position = transform.position;
        //attackObj.transform.parent = gameObject.transform;
        attackObj.transform.parent = transform; // make the attack a child of enemy 

        attackScript = attackObj.GetComponent<TH_Cone>();
        behaviourScript = gameObject.GetComponent<IceEnemyBehaviour>();

        // the telegraph starts at 70% of the total hitbox since the hitbox is a ring.
        attackScript.Init(attackStats.windupTime, attackStats.activeTime, attackStats.cooldownTime, attackStats.Size, attackStats.Damage, attackStats.StartingTelegaphPercentSize);

        float delay = UnityEngine.Random.Range(0,2);
        StartCoroutine(AttackDelay(delay));

    }

    void Update()
    {
        // if the attack is ready, that means the previous attack is complete
        if(attackScript.attackEnded)
        {
            //StartCoroutine(attackScript.StartCooldown(ToggleAttackReady));
            //attackScript.attackEnded = false;
            attacking = false;
            animationComplete = false;
        }

        if(attackScript.attackReady && behaviourScript.playerInRange)
        {
            // rotate cone
            Vector2 dir =  player.transform.position - transform.position;
            float radvalue = Mathf.Atan2(-dir.y, -dir.x);
            float angle= radvalue * (180/Mathf.PI);
            attackObj.transform.localRotation = Quaternion.Euler(0,0,angle -90);

            attacking = true;
            attackScript.StartAttack();
            //ToggleAttackReady(false);
        }

        if(attackScript.windupProgress >= attackStats.animationStartPercent && !animationComplete)
        {
            // windupTimer += Time.deltaTime;

            // if(windupTimer / attackStats.windupTime >= 0.8)
            // {
                GameObject attack = Instantiate(snowConePrefab, gameObject.transform.position, attackObj.transform.localRotation);
                attack.transform.parent = transform;
                animationComplete = true;
                //attackScript.startAnimation = false;
                //windupTimer = 0;
                //attackScript.startAnimation = false;
            //}
        }
            
        
    }

    IEnumerator AttackDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(attackScript.StartCooldown());
    }

    // void ToggleAttackReady(bool isReady)
    // {
    //     attackStats.attackReady = isReady;
    // }
}
