using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageEffects : MonoBehaviour
{
   [ColorUsage(true, true)]
   [SerializeField] private Color flashColor = Color.white;
   [SerializeField] private float flashTime = 0.4f;

   public AnimationCurve animCurve;

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

   public void StartDamageFlinch()
   {
        StartCoroutine(DamageFlinch(0.2f));
   }

    IEnumerator DamageFlinch(float time)
    {
        // Vector2 scale = dir * 0.5f;
        // transform.localScale = new Vector2(0.7f, 0.7f);
        // yield return new WaitForSecondsRealtime(time);
        // transform.localScale = new Vector2(1,1);
        Transform spriteParent = transform.GetChild(0);
        float startTime = Time.time;

        while(Time.time - startTime <= time)
        {
            float ratio = (Time.time - startTime) / time;
            //float scale = Mathf.Lerp(1, 0.7f, ratio);
            float scale = animCurve.Evaluate(ratio);
            spriteParent.localScale = new Vector2(scale, scale);
            yield return null;
        }
        transform.localScale = new Vector2(1, 1);
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
