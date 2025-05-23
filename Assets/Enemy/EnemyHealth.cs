using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    private int currHealth;
    

    [SerializeField] private float maxOuterLightRad, minOuterLightRad;
    [SerializeField] private Light2D healthLight;

    [SerializeField] private GameObject essencePrefab;
    [SerializeField] private GameObject damageText;

    EnemyDamageEffects enemyAnimator;
    EnemyInfo enemyInfo;

    // multiply damage by 10 since all enemy and player stats are scaled up by 10 to make upgrades feel more worth it to buy
    private int damageMod = 10; 

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = (int) Math.Ceiling(maxHealth * GetComponent<EnemyInfo>().healthMod);
        currHealth = maxHealth;

        maxOuterLightRad = healthLight.pointLightOuterRadius;

        enemyAnimator = GetComponent<EnemyDamageEffects>();
        enemyInfo = GetComponent<EnemyInfo>();
    }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

    public void DealDamage(Vector2 impact, float DamageBonusPercent)
    {
        // damage mod increases damage by factor of 10 so upgrades feel more impactful
        // multiply damage mod by 1.5 so the difference between attacks is more obvious
        // damage bonus percent is damage bonus from upgrades
        int damage = (int) Math.Floor(impact.magnitude * damageMod * 1.5f  * (1 + DamageBonusPercent));
        currHealth -= damage;

        ModifyLightHealthBar();

        enemyAnimator.StartDamageFlash();
        GameObject damageTextParent = Instantiate(damageText, new Vector2 (transform.position.x, transform.position.y + 1), Quaternion.identity);
        damageTextParent.GetComponentInChildren<TextMeshPro>().text = damage.ToString();
        enemyAnimator.StartDamageFlinch();

        if(currHealth <= 0)
        {
            enemyAnimator.StartDeathAnimation(impact);
            DropEssence(impact);

            if(GameStats.currGameMode == GameStats.GameMode.Survival)
            {
                HealPlayer();
            }

            Destroy(gameObject);   
        }

       
        
    }

    private void DropEssence(Vector3 impact)
    {
        //int maxDrops = 3;
        //int numToDrop = UnityEngine.Random.Range(1, maxDrops + 1);
        //float dropRad = 0.5f;
        
        // for(int i = 0; i < numToDrop; i++)
        // {
        //     float dropDistX = UnityEngine.Random.Range(-dropRad, dropRad);
        //     float dropDistY = UnityEngine.Random.Range(-dropRad, dropRad);
            //Instantiate(essencePrefab, new Vector3(transform.position.x + dropDistX, transform.position.y + dropDistY, 0), transform.rotation);
        //}
        GameObject drop = Instantiate(essencePrefab, new Vector3(transform.position.x, transform.position.y, 0), transform.rotation);
        drop.transform.GetChild(0).GetComponent<SpriteRenderer>().color = enemyInfo.soulColor;
        drop.transform.GetChild(0).GetComponentInChildren<Light2D>().color = enemyInfo.soulLightColor;

        drop.GetComponent<Rigidbody2D>().velocity = impact;


    }

    // private IEnumerator AnimateSoulDrop(GameObject soul,)
    // {
    //     float startTime = Time.time;

    //     // while (Time.time - startTime <= dashTime)
	// 	// {
	// 	// 	rb.velocity = force.normalized * dashSpeed;
    //     //     prevDashVelocity = rb.velocity;
	// 	// 	//Pauses the loop until the next frame, creating something of a Update loop. 
	// 	// 	//This is a cleaner implementation opposed to multiple timers and this coroutine approach is actually what is used in Celeste :D
	// 	// 	yield return null;
	// 	// }
    //     yield return new WaitForSeconds(startTime);

	// 	//Begins the "end" of our dash where we return some control to the player but still limit run acceleration (see Update() and Run())

	// 	rb.velocity = dashSpeed * 0.5f * force.normalized;
    // }

    private void ModifyLightHealthBar()
    {
        float healthRatio = (float) currHealth / (float) maxHealth;
        float diff = maxOuterLightRad - minOuterLightRad;

        // Decrease the light radius based on the missing health.
        healthLight.pointLightOuterRadius = healthRatio * diff + minOuterLightRad;
        healthLight.pointLightInnerRadius = healthLight.pointLightOuterRadius * 0.75f;
    }

    private void HealPlayer()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>().Heal(110);
    }
}
