using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            int damage = (int) Math.Floor(rb.velocity.magnitude);
            other.gameObject.GetComponent<EnemyHealth>().DealDamage(damage);
            //Debug.Log(rb.velocity.magnitude);
        }
    }
}
