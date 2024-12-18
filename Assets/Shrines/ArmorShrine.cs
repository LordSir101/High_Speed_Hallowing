using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArmorShrine : Shrine
{
    PlayerHealth playerHealthScript;
    private int[] armorUpgradeValues;

    [SerializeField] Sprite cleanseIcon;
    [SerializeField] Material cleanseIconMaterial;


    // Start is called before the first frame update
    void Start()
    {
        playerHealthScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        CleanseIcon = cleanseIcon;
        CleanseIconMaterial = cleanseIconMaterial;

        armorUpgradeValues = new int[] {10, 15, 15};
    }

    protected override void ShowUpgradeText()
    {
        interactText.enabled = true;
        interactText.text = $"Light Candle: +{armorUpgradeValues[numUpgrades]}  Armor ({upgradeCosts[numUpgrades]})";
    }

    protected override void Upgrade(InputAction.CallbackContext context)
    {
        PlayerResourceManager rm = player.GetComponent<PlayerResourceManager>();

        if(rm.Essence >= upgradeCosts[numUpgrades])
        {
            rm.Essence -= upgradeCosts[numUpgrades];
            ShowUpgradeCostText(upgradeCosts[numUpgrades]);
            
            playerHealthScript.Armor += armorUpgradeValues[numUpgrades];
            numUpgrades++;
            LightCandle();
            PlayUpgradeSound();
           
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
