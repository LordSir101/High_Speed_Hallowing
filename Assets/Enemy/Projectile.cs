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
    private float scale;
    float radius;
    private int lifespan;
    private float lifespanTimer = 0;

    Action<Collision2D> callback;

    public void Init(int damage, float damageMod, Sprite sprite, float radius, float scale = 0.5f, int lifespan = Int32.MaxValue, Action<Collision2D> callback = null)
    {
        this.sprite = sprite;
        this.damage = damage;
        this.damageMod = damageMod;
        this.callback = callback;
        this.lifespan = lifespan;
        this.radius = radius;
        this.scale = scale;
    }
    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = sprite;
        GetComponent<CircleCollider2D>().radius = radius;
        transform.localScale = new Vector3(scale, scale, scale);
    }

    void Update()
    {
        // by default, a projectile will live until it hits something
        lifespanTimer += Time.deltaTime;

        if(lifespanTimer > lifespan)
        {
            if(callback != null)
            {
                callback(null);
            }
            Destroy(gameObject);
        }
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

        if(callback != null)
        {
            callback(other);
        }
        Destroy(gameObject);
    }
}
