using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HealingShrine : Shrine
{
    PlayerHealth playerHealthScript;
    [SerializeField] Sprite cleanseIcon;
    [SerializeField] Material cleanseIconMaterial;
    void Start()
    {
        playerHealthScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        CleanseIcon = cleanseIcon;
        CleanseIconMaterial = cleanseIconMaterial;
    }

    protected override void ShowUpgradeText()
    {
        interactText.enabled = true;
        interactText.text = $"Heal 30% health ({upgradeCosts[numUpgrades]})";
    }

    protected override void Upgrade(InputAction.CallbackContext context)
    {
        PlayerResourceManager rm = player.GetComponent<PlayerResourceManager>();

        if(rm.Essence >= upgradeCosts[numUpgrades])
        {
            rm.Essence -= upgradeCosts[numUpgrades];
            playerHealthScript.HealPercentHealthOverTime(0.3f, 10);
            numUpgrades++;

            LightCandle();
        }
       
        if(numUpgrades == maxUpgrades)
        {
            CloseShrine();
        }
        else
        {
            ShowUpgradeText();
        }
    }
}
