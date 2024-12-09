using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HealthShrine : Shrine
{
    PlayerHealth playerHealthScript;
    private int[] healthUpgradeValues;

    //[SerializeField] Sprite uncleanseIcon;
    [SerializeField] Sprite cleanseIcon;
    [SerializeField] Material cleanseIconMaterial;
    // Animator animator;
    // GameObject icon;

    // Start is called before the first frame update
    void Start()
    {
        playerHealthScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        CleanseIcon = cleanseIcon;
        CleanseIconMaterial = cleanseIconMaterial;
        // animator = GetComponentInChildren<Animator>();
        // icon = Utils.FindGameObjectInChildWithTag(gameObject, "Icon");

        healthUpgradeValues = new int[] {200, 300, 400};
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
            playerHealthScript.Heal(healthUpgradeValues[numUpgrades]);
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

    // protected override void ChangeIcon()
    // {
    //     icon.GetComponent<SpriteRenderer>().sprite = cleanseIcon;
    //     icon.GetComponent<SpriteRenderer>().material = cleanseIconMaterial;
    //     animator.SetTrigger("activated");
    // }
}
