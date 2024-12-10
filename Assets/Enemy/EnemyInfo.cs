using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInfo : MonoBehaviour
{
    [SerializeField] public int id;
    [SerializeField] public Color soulColor;
    [SerializeField] public Color soulLightColor;

    public float damageMod {get;set;}
    public float healthMod {get;set;}
}
