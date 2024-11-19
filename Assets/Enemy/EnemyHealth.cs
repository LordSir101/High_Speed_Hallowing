using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    private int currHealth;

    [SerializeField] private float maxOuterLightRad, minOuterLightRad;
    [SerializeField] private Light2D healthLight;

    [SerializeField] private GameObject essencePrefab;

    // Start is called before the first frame update
    void Start()
    {
        currHealth = maxHealth;

        maxOuterLightRad = healthLight.pointLightOuterRadius;
    }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

    public void DealDamage(int damage)
    {
        // currHealth -= damage;

        // ModifyLightHealthBar();

        // if(currHealth <= 0)
        // {
        //     Destroy(gameObject);
        //     DropEssence();
        // }
    }

    private void DropEssence()
    {
        int maxDrops = 3;
        int numToDrop = UnityEngine.Random.Range(1, maxDrops + 1);
        float dropRad = 0.5f;
        
        for(int i = 0; i < numToDrop; i++)
        {
            float dropDistX = UnityEngine.Random.Range(-dropRad, dropRad);
            float dropDistY = UnityEngine.Random.Range(-dropRad, dropRad);
            Instantiate(essencePrefab, new Vector3(transform.position.x + dropDistX, transform.position.y + dropDistY, 0), transform.rotation);
        }
    }

    private void ModifyLightHealthBar()
    {
        float healthRatio = (float) currHealth / (float) maxHealth;
        float diff = maxOuterLightRad - minOuterLightRad;

        // Decrease the light radius based on the missing health.
        healthLight.pointLightOuterRadius = healthRatio * diff + minOuterLightRad;
        healthLight.pointLightInnerRadius = healthLight.pointLightOuterRadius * 0.75f;
    }
}
