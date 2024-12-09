using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DamageShrine : Shrine
{
    PlayerAttack playerAttackScript;
    private float[] dmgUpgradeValues;
    [SerializeField] Sprite cleanseIcon;
    [SerializeField] Material cleanseIconMaterial;

    // Start is called before the first frame update
    void Start()
    {
        playerAttackScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAttack>();
        CleanseIcon = cleanseIcon;
        CleanseIconMaterial = cleanseIconMaterial;

        dmgUpgradeValues = new float[] {0.10f, 0.10f, 0.15f};
    }

    protected override void ShowUpgradeText()
    {
        interactText.enabled = true;
        interactText.text = $"Increase damage dealt +{dmgUpgradeValues[numUpgrades]} ({upgradeCosts[numUpgrades]})";
    }

    protected override void Upgrade(InputAction.CallbackContext context)
    {
        PlayerResourceManager rm = player.GetComponent<PlayerResourceManager>();

        if(rm.Essence >= upgradeCosts[numUpgrades])
        {
            rm.Essence -= upgradeCosts[numUpgrades];
            playerAttackScript.DamageBonusPercent += dmgUpgradeValues[numUpgrades];
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
