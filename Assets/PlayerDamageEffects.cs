using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageEffects : MonoBehaviour
{
   [ColorUsage(true, true)]
   [SerializeField] private Color flashColor = Color.white;
   [SerializeField] private float flashTime = 0.25f;

   private SpriteRenderer[] spriteRenderers;
   private List<Material> materials;
   //[SerializeField] Material damageFlashMaterial;

   private void Awake()
   {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
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
