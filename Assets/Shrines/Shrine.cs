using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shrine : MonoBehaviour
{
    private InputActionReference interact;
    protected TextMeshProUGUI interactText;
    private ShrineManager shrineManager;
    protected GameObject player;

    public int Cost {get; set;}= 500;

    protected int[] upgradeCosts;
    protected int maxUpgrades = 3;
    protected int numUpgrades = 0;
    protected bool cleansed = false;

    // Start is called before the first frame update
    // void Start()
    // {
    //     // GameObject textObj = FindGameObjectInChildWithTag(gameObject., "InteractText");
    //     // interactText = textObj.GetComponent<TextMeshProUGUI>();
        
    // }

    public void Init(InputActionReference interact, ShrineManager shrineManager)
    {
        player = GameObject.FindGameObjectWithTag("Player");
        upgradeCosts = new int[] {500, 700, 900};
        interactText = Utils.FindGameObjectInChildWithTag(gameObject.GetComponentInChildren<Canvas>().gameObject, "InteractText").GetComponent<TextMeshProUGUI>();
        this.interact = interact;
        this.shrineManager = shrineManager;
    }

    private void Tribute(InputAction.CallbackContext context)
    {
        PlayerResourceManager rm = player.GetComponent<PlayerResourceManager>();

        if(rm.Essence >= Cost)
        {
            rm.Essence -= 500;
            //gameObject.GetComponent<CircleCollider2D>().enabled = false;
            //interactText.enabled = false;
            shrineManager.CleanseShrine();
            cleansed = true;

            // allows upgrade text to appear right away without moving away from shrine
            interact.action.performed -= Tribute;
            gameObject.GetComponent<CircleCollider2D>().enabled = false;
            gameObject.GetComponent<CircleCollider2D>().enabled = true;
        }   
    }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            if(cleansed)
            {
                ShowUpgradeText();
                interact.action.performed += Upgrade;
            }
            else
            {
                interactText.enabled = true;
                interactText.text = interactText.text + $" ({Cost})" ;
                interact.action.performed += Tribute;
            }
        }
        
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            if(cleansed)
            {
                interact.action.performed -= Upgrade;
            }
            else
            {
                interact.action.performed -= Tribute;
            }
            
            interactText.enabled = false;
        }
        
    }

    protected virtual void ShowUpgradeText()
    {
        return;
    }

    protected virtual void Upgrade(InputAction.CallbackContext context)
    {
        return;
    }

    protected void CloseShrine()
    {
        gameObject.GetComponent<CircleCollider2D>().enabled = false;
        interactText.enabled = false;
    }
}
