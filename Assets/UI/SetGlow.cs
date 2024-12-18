using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetGlow : MonoBehaviour
{
    [ColorUsage(true, true)]
    [SerializeField] private Color flashColor;
    // Start is called before the first frame update
    void Start()
    {
        if(GetComponent<SpriteRenderer>().material.name == "DamageFlashMAT (Instance)")
        {
            GetComponent<SpriteRenderer>().material.SetColor("_FlashColor", flashColor);
            GetComponent<SpriteRenderer>().material.SetFloat("_FlashAmount", 1);
        }
    }
}
