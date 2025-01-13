using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class FisherEnemyAttack : MonoBehaviour
{
    private EnemyTelegraphAttack attackStats;
    private FisherEnemyBehaviour behaviourScript;
    private FishRodAttack attackScript;

    [SerializeField]
    private GameObject attackType;

    [SerializeField] Animator animator;
    [SerializeField] Animator bodyAnimator;
    private EnemySounds audioController;

    [Header("attack Sounds")]
    [SerializeField] GameObject attackAudioParent;

    public float windupTimer = 0;

    private bool animationComplete = false;
    public bool playerInRange = false;
    public bool attacking = false;
    GameObject attack;
    GameObject player;
    [SerializeField] GameObject hands;

    

    void Start()
    {
        attackStats = gameObject.GetComponent<EnemyTelegraphAttack>();
        //animator = gameObject.GetComponentInChildren<Animator>();
        behaviourScript = gameObject.GetComponent<FisherEnemyBehaviour>();
        audioController = gameObject.GetComponentInChildren<EnemySounds>();

        player = GameObject.FindGameObjectWithTag("Player");

        attack = Instantiate(attackType);
        
        attack.transform.position = transform.position;
        //attack.transform.parent = gameObject.transform;
        attack.transform.parent = transform; // make the attack a child of enemy 

        attackScript = attack.GetComponent<FishRodAttack>();

        // the telegraph starts at 70% of the total hitbox since the hitbox is a ring.
        attackScript.Init(attackStats.windupTime, attackStats.activeTime, attackStats.cooldownTime, attackStats.Size, attackStats.Damage, attackStats.StartingTelegaphPercentSize, attackStats.animationStartPercent);

        float delay = UnityEngine.Random.Range(0,2);

        //behaviourScript.ChangeState("kiting");
        StartCoroutine(AttackDelay(delay));

    }

    void Update()
    {
        
         if(attackScript.attackReady && behaviourScript.playerInRange)
        {
            // rotate cone
            Vector2 dir = player.transform.position - transform.position;
            float radvalue = Mathf.Atan2(-dir.y, -dir.x);
            float angle= radvalue * (180/Mathf.PI);
            attack.transform.localRotation = Quaternion.Euler(0,0,angle -90);

            
            attackScript.StartAttack();
            
            //ToggleAttackReady(false);
        }

        // start animation slightly before active frames
        if(attackScript.windupProgress >= attackStats.animationStartPercent && !animationComplete)
        {
            attacking = true;
            audioController.SetCurrAttackAudio(attackAudioParent);
            // windupTimer += Time.deltaTime;

            // if(windupTimer / attackStats.windupTime >= 0.8)
            // {
                // set chain attack audio as the audio to be played
                // the audio will be played later by an animation event
                //audioController.SetCurrAttackAudio(attackAudioParent);
            

            bodyAnimator.enabled = false;

            Vector2 dir = player.transform.position - transform.position;
            float radvalue = Mathf.Atan2(dir.y, dir.x);
            float angle= radvalue * (180/Mathf.PI);
            hands.transform.localRotation = Quaternion.Euler(0,0,angle -90);

            animator.SetTrigger("Swing");
            //animator.Play("RodSwing2");
            animationComplete = true;
                //attackScript.startAnimation = false;
                //windupTimer = 0;
                //attackScript.startAnimation = false;
            //}
            
        }
        if(attackScript.attackEnded)
        {
            //behaviourScript.ChangeState("kiting");
            animationComplete = false;
            hands.transform.localRotation = transform.localRotation;
            bodyAnimator.enabled = true;
            attacking = false;
            //StartCoroutine(attackScript.StartCooldown());
            //attackScript.attackEnded = false;
        }
    }

    IEnumerator AttackDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(attackScript.StartCooldown());
    }

    // public void PlayAttackSound()
    // {
    //     GetComponentInChildren<AudioSource>().Play();
    // }

    // public void AnimateActiveFrames()
    // {
    //     animator.SetTrigger("Attack");
    // }

    // void ToggleAttackReady(bool isReady)
    // {
    //     attackStats.attackReady = isReady;
    // }
}
