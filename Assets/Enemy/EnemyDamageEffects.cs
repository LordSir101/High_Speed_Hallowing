using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageEffects : MonoBehaviour
{
   [ColorUsage(true, true)]
   [SerializeField] private Color flashColor = Color.white;
   [SerializeField] private float flashTime = 0.25f;

   private SpriteRenderer[] spriteRenderers;
   private List<Material> materials;

   [SerializeField] GameObject deathAnimator;
//    private ParticleSystem particleSystem;
   private EnemyInfo enemyInfo;
   //[SerializeField] Material damageFlashMaterial;

   private void Awake()
   {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        //particleSystem = deathAnimator.GetComponent<ParticleSystem>();
        enemyInfo = GetComponent<EnemyInfo>();
        Init();
   }

   private void Init()
   {
        materials = new List<Material>();

        for(int i = 0; i < spriteRenderers.Length; i++)
        {
            if(spriteRenderers[i].material.name == "DamageFlashMAT (Instance)")
            {
                Debug.Log(spriteRenderers[i]);
                materials.Add(spriteRenderers[i].material);
            }
            
        }
   }

   public void StartDeathAnimation(Vector3 impact)
   {    
        GameObject animation = Instantiate(deathAnimator,transform.position,transform.rotation);
        ParticleSystem particleSystem = animation.GetComponent<ParticleSystem>();

        ParticleSystem.MainModule settings = particleSystem.main;
        settings.startColor = enemyInfo.soulColor;

        Vector2 normalized = impact.normalized;
        float radValue = Mathf.Atan2(normalized.y, normalized.x);
        float angle= radValue * (180/Mathf.PI);
        animation.transform.rotation = Quaternion.Euler(0,0, angle -90);;

        particleSystem.Play();

   }

   public void StartDamageFlash()
   {
        StartCoroutine(DamageFlash());
   }

   private IEnumerator DamageFlash()
   {
        SetFlashColor();

        float currentFlashAmount = 0f;
        float elapsedTime = 0f;

        while(elapsedTime < flashTime)
        {
            elapsedTime += Time.deltaTime;

            currentFlashAmount = Mathf.Lerp(1f, 0, elapsedTime / flashTime);

            SetFlashAmount(currentFlashAmount);

            yield return null;
        }
   }

    private void SetFlashAmount(float amount)
    {
        for(int i = 0; i< materials.Count; i++)
        {
            materials[i].SetFloat("_FlashAmount", amount);
        }
    }

    private void SetFlashColor()
   {
        for(int i = 0; i < materials.Count; i++)
        {
            materials[i].SetColor("_FlashColor", flashColor);
        }
   }
}
