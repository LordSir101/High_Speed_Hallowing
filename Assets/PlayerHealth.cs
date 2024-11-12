using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth: MonoBehaviour
{
    public GameOverPanel gameOverPanel;
    public HealthBar healthBar;
    private int health = 100;
    public int MaxHealth { get; set; } = 100;
    public void TakeDamage(int damageTaken)
    {
        health -= damageTaken;
        healthBar.SetHealth(health);

        if(health <= 0)
        {
            gameOverPanel.SetWin(false);
        }
    }
    
}
