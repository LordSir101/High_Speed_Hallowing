using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Projectile : MonoBehaviour
{
    private int damage = 0;
    private Sprite sprite;

    public void Init(int damage, Sprite sprite)
    {
        this.sprite = sprite;
        this.damage = damage;
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
            other.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
            //Destroy(gameObject);
        }
        Destroy(gameObject);
    }
}
