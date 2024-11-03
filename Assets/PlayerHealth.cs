using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth: MonoBehaviour
{
    private int health = 100;
    public void TakeDamage(int damageTaken)
    {
        health -= damageTaken;
        //Debug.Log(health);
    }
    
}
