using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardAttack : MonoBehaviour
{
    private EnemyTelegraphAttack attackStats;
    private SlowingCircle attackScript;
    [SerializeField] private float range;

    [SerializeField]
    private GameObject attackType;
    GameObject attack;
    Vector2 attackPos;
    // Start is called before the first frame update
    void Start()
    {
        attackStats = gameObject.GetComponent<EnemyTelegraphAttack>();
        
        //animator = gameObject.GetComponent<Animator>();

        attackPos = GetAttackPosition();

        attack = Instantiate(attackType);
        
        attack.transform.position  = attackPos;
        //attack.transform.parent = gameObject.transform;
        //attack.transform.parent = transform; // make the attack a child of enemy 

        attackScript = attack.GetComponent<SlowingCircle>();

        attackScript.Init(attackStats.windupTime, attackStats.activeTime, attackStats.cooldownTime, attackStats.Size, attackStats.Damage, attackStats.StartingTelegaphPercentSize, attackStats.animationStartPercent);

        attack.transform.parent = transform;
        attack.GetComponent<SpriteRenderer>().sortingLayerID = 0;

        float delay = UnityEngine.Random.Range(0,2);
        StartCoroutine(AttackDelay(delay));
        
    }

    private Vector2 GetAttackPosition()
    {
        // float posX = UnityEngine.Random.Range(-range, range);
        // float posY = UnityEngine.Random.Range(-range, range);


        //return new Vector2(transform.position.x + posX, transform.position.y + posY);
        return new Vector2(transform.position.x, transform.position.y);
    }

    // Update is called once per frame
    void Update()
    {
        if(attackScript.attackReady)
        {
            // Vector2 attackPos = GetAttackPosition();

            // GameObject attack = Instantiate(attackType);
            
            // attack.transform.position  = attackPos;
            // //attack.transform.parent = gameObject.transform;
            // //attack.transform.parent = transform; // make the attack a child of enemy 

            // attackScript = attack.GetComponent<SlowingCircle>();

            // attackScript.Init(attackStats.windupTime, attackStats.activeTime, attackStats.cooldownTime, attackStats.Size, attackStats.Damage, attackStats.StartingTelegaphPercentSize, attackStats.animationStartPercent);
            attackScript.StartAttack();

            //attackScript.StartCooldown();
            
            //ToggleAttackReady(false);
            //StartCoroutine(attackScript.StartCooldown(ToggleAttackReady));
        }
        if(attackScript.attackEnded)
        {
            attackScript.transform.GetChild(1).GetComponent<SpriteRenderer>().enabled = false;
            attackPos = GetAttackPosition();
            attack.transform.position  = attackPos;
        }

        if(attackScript.windupProgress >= 1)
        {
            attackScript.transform.GetChild(1).GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    IEnumerator AttackDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(attackScript.StartCooldown());
    }
}
