using System.Collections;
using UnityEngine;

public class IceEnemyConeAttack : MonoBehaviour
{
   
    private EnemyTelegraphAttack attackStats;
    private TH_Cone attackScript;
    private IceEnemyBehaviour behaviourScript;
    private EnemySounds audioController;
    GameObject attackObj;

    [SerializeField]
    private GameObject attackType;
    GameObject player;

    public bool attacking = false;
    private bool animationComplete = false;
    [SerializeField] private GameObject snowConePrefab;
    [SerializeField] GameObject snowconeAudioParent;
    [SerializeField] Animator animator;
    GameObject currAttack;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        attackStats = gameObject.GetComponent<EnemyTelegraphAttack>();
        audioController = gameObject.GetComponentInChildren<EnemySounds>();

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
            currAttack.GetComponentInChildren<SpriteRenderer>().enabled = false;
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
            audioController.SetCurrAttackAudio(snowconeAudioParent);
            //ToggleAttackReady(false);
        }

        if(attackScript.windupProgress >= attackStats.animationStartPercent && !animationComplete)
        {
            // windupTimer += Time.deltaTime;

            // if(windupTimer / attackStats.windupTime >= 0.8)
            // {
                animator.SetTrigger("Attack");
                currAttack = Instantiate(snowConePrefab, gameObject.transform.position, attackObj.transform.localRotation);
                currAttack.transform.parent = transform;
                currAttack.GetComponent<AudioSource>().Play();
                animationComplete = true;

                // show the snow area sprite
                currAttack.GetComponentInChildren<SpriteRenderer>().enabled = true;
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
