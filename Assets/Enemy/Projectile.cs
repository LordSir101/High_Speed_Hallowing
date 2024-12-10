using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Projectile : MonoBehaviour
{
    private int damage = 0;
    private float damageMod = 1;
    private Sprite sprite;

    public void Init(int damage, float damageMod, Sprite sprite)
    {
        this.sprite = sprite;
        this.damage = damage;
        this.damageMod = damageMod;
    }
    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = sprite;
    }
    
    // Start is called before the first frame update
    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     if(other.gameObject.tag == "Player")
    //     {
    //         other.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
    //         Destroy(gameObject);
    //     }
    // }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            // increase damage based on difficulty
            int newDamage = (int) Math.Ceiling(damage * damageMod);
            other.gameObject.GetComponent<PlayerHealth>().TakeDamage(newDamage);
            //Destroy(gameObject);
        }
        Destroy(gameObject);
    }
}
