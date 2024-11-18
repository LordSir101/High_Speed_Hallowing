using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HealthShrine : Shrine
{
    PlayerHealth playerHealthScript;
    private int[] healthUpgradeValues;

    // Start is called before the first frame update
    void Start()
    {
        playerHealthScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();

        healthUpgradeValues = new int[] {50, 150, 300};
    }

    protected override void ShowUpgradeText()
    {
        interactText.enabled = true;
        interactText.text = $"Upgrade max health +{healthUpgradeValues[numUpgrades]} ({upgradeCosts[numUpgrades]})";
    }

    protected override void Upgrade(InputAction.CallbackContext context)
    {
        PlayerResourceManager rm = player.GetComponent<PlayerResourceManager>();

        if(rm.Essence >= upgradeCosts[numUpgrades])
        {
            rm.Essence -= upgradeCosts[numUpgrades];
            playerHealthScript.MaxHealth += healthUpgradeValues[numUpgrades];
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
