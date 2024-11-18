using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArmorShrine : Shrine
{
    PlayerHealth playerHealthScript;
    private int[] armorUpgradeValues;

    // Start is called before the first frame update
    void Start()
    {
        playerHealthScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();

        armorUpgradeValues = new int[] {1, 2, 3};
    }

    protected override void ShowUpgradeText()
    {
        interactText.enabled = true;
        interactText.text = $"Upgrade Armor +{armorUpgradeValues[numUpgrades]} ({upgradeCosts[numUpgrades]})";
    }

    protected override void Upgrade(InputAction.CallbackContext context)
    {
        PlayerResourceManager rm = player.GetComponent<PlayerResourceManager>();

        if(rm.Essence >= upgradeCosts[numUpgrades])
        {
            rm.Essence -= upgradeCosts[numUpgrades];
            playerHealthScript.Armor += armorUpgradeValues[numUpgrades];
            numUpgrades++;

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
