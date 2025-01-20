using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class PlayerHealth: MonoBehaviour
{
    [SerializeField] ScriptableRendererFeature frenzyEffect;
    [SerializeField] GameObject damageText;
    [SerializeField] public GameControl gameController;
    [SerializeField] PauseControl pauseControl;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private ParticleSystem healingEffect;
    public HealthBar healthBar;
    private PlayerDamageEffects damageEffects;
    private PlayerAudio playerAudio;
    private int health;
    public int MaxHealth { get; set; } = 1000;//1000;
    public int Armor {get; set;} = 0;
    private bool isDead = false;

    void Start()
    {
        health = MaxHealth;
        damageEffects = GetComponent<PlayerDamageEffects>();
        playerAudio = GetComponentInChildren<PlayerAudio>();
    }
    public void TakeDamage(int damageTaken)
    {
        int damageDealt = damageTaken - Armor > 0 ? damageTaken - Armor : 0;
        health -= damageDealt;
        healthBar.SetHealth(health);

        if(health <= 0)
        {
            if(!isDead)
            {
                playerInput.actions.FindActionMap("PlayerInput").Disable();
                frenzyEffect.SetActive(false);
                gameController.SetWin(false);
                isDead = true;
            }
            
        }

        damageEffects.StartDamageFlash();
        damageEffects.StartCameraShake();
        playerAudio.PlayPlayerDamageSound();
        pauseControl.HitPause(0.01f);
        
        GameObject damageTextParent = Instantiate(damageText, new Vector2 (transform.position.x, transform.position.y + 1), Quaternion.identity);
        damageTextParent.GetComponentInChildren<TextMeshPro>().text = $"-{damageDealt}";
        damageTextParent.GetComponentInChildren<TextMeshPro>().color = Color.red;
    }

    public void Heal(int healing)
    {
        if(health + healing > MaxHealth)
        {
            health = MaxHealth;
        }
        else
        {
            health += healing;
        }
        
        healthBar.SetHealth(health);
        PlayHealingEffect(3);
        DisplayHealingNumbers(healing);
        
    }

    // public void HealToFull()
    // {
    //     health = MaxHealth;
    //     healthBar.SetHealth(health);
    // }
    public void HealPercentHealthOverTime(float percent, int time)
    {
        StartCoroutine(StartHOT(percent, time));
    }

    IEnumerator StartHOT(float percent, int time)
    {
        // ParticleSystem particleSystem = healingEffect;

        // ParticleSystem.MainModule settings = particleSystem.main;
        // settings.duration = time;
        // PlayHealingEffect(time);

        float startTime = Time.time;

        float totalhealing = MaxHealth *  percent;
        int healingIncrement = (int) Mathf.Ceil(totalhealing / time);

        while(Time.time - startTime <= time)
        {
            Heal(healingIncrement);
            yield return new WaitForSeconds(1f);
        }
    }

    void PlayHealingEffect(float time)
    {
        // ParticleSystem particleSystem = healingEffect;

        // ParticleSystem.MainModule settings = particleSystem.main;
        // settings.duration = time;

        healingEffect.Play();

    }

    void DisplayHealingNumbers(int amount)
    {
        GameObject healingTextParent = Instantiate(damageText, new Vector2 (transform.position.x, transform.position.y + 1), Quaternion.identity);
        healingTextParent.GetComponentInChildren<TextMeshPro>().text = $"+{amount}";
        healingTextParent.GetComponentInChildren<TextMeshPro>().color = Color.green;
    }
    
}
