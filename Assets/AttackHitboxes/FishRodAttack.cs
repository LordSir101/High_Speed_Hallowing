using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishRodAttack : TelegraphedHitbox
{
    // // Start is called before the first frame update
    // void Start()
    // {
        
    // }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }
    private float damageMod;
    void Start()
    {
        damageMod = transform.parent.GetComponent<EnemyInfo>().damageMod;
        transform.localScale = new Vector3(Size + 1, Size, Size);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            //Debug.Log(transform.parent.GetComponent<EnemyInfo>().damageMod);
            // increase damage based on difficulty
            int damage = (int) Math.Ceiling(Damage * damageMod);
            other.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
            SetAllCollidersStatus(false);
        }
    }
}
