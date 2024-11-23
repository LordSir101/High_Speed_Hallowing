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

    //[SerializeField] Sprite angrySprite;
    Sprite cleanseSprite;
    

    public int Cost {get; set;}= 500;

    protected int[] upgradeCosts;
    protected int maxUpgrades = 3;
    protected int numUpgrades = 0;
    protected bool cleansed = false;

    protected virtual GameObject icon {get;set;}
    protected virtual Animator animator {get;set;}
    protected Sprite CleanseIcon {get;set;}
    protected Material CleanseIconMaterial {get;set;}
    protected List<GameObject> candles;

    // Start is called before the first frame update
    // void Start()
    // {
    //     // GameObject textObj = FindGameObjectInChildWithTag(gameObject., "InteractText");
    //     // interactText = textObj.GetComponent<TextMeshProUGUI>();
        
    // }

    public void Init(InputActionReference interact, ShrineManager shrineManager, Sprite cleanseSprite)
    {
        player = GameObject.FindGameObjectWithTag("Player");
        upgradeCosts = new int[] {500, 700, 900};
        interactText = Utils.FindGameObjectInChildWithTag(gameObject.GetComponentInChildren<Canvas>().gameObject, "InteractText").GetComponent<TextMeshProUGUI>();
        this.interact = interact;
        this.shrineManager = shrineManager;
        this.cleanseSprite = cleanseSprite;
        animator = GetComponentInChildren<Animator>();
        icon = Utils.FindGameObjectInChildWithTag(gameObject, "Icon");

        GameObject candleParent = Utils.FindGameObjectInChildWithTag(gameObject, "ShrineCandle");
        candles = new List<GameObject>();
        foreach(Transform candle in candleParent.transform)
        {
            candles.Add(candle.gameObject);
        }
    }

    private void Tribute(InputAction.CallbackContext context)
    {
        PlayerResourceManager rm = player.GetComponent<PlayerResourceManager>();

        if(rm.Essence >= Cost)
        {
            GetComponent<ParticleSystem>().Play();
            rm.Essence -= Cost;
            //gameObject.GetComponent<CircleCollider2D>().enabled = false;
            //interactText.enabled = false;
            shrineManager.CleanseShrine();
            cleansed = true;

            // allows upgrade text to appear right away without moving away from shrine
            interact.action.performed -= Tribute;
            
            ShowUpgradeText();
            interact.action.performed += Upgrade;

            gameObject.GetComponent<SpriteRenderer>().sprite = cleanseSprite;

            ChangeIcon();

            shrineManager.SpawnEnemiesAtShrine(transform.position, 3);
        }   
    }

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
                interactText.text = $"Tribute to cleanse (E) ({Cost})" ;
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
     protected void ChangeIcon()
    {
        icon.GetComponent<SpriteRenderer>().sprite = CleanseIcon;
        icon.GetComponent<SpriteRenderer>().material = CleanseIconMaterial;
        animator.SetTrigger("activated");
    }
    protected virtual void ShowUpgradeText()
    {
        return;
    }
    // protected virtual string GetUpgradeText()
    // {
    //     return "";
    // }

    protected virtual void Upgrade(InputAction.CallbackContext context)
    {
        return;
    }

    protected void CloseShrine()
    {
        gameObject.GetComponent<CircleCollider2D>().enabled = false;
        interactText.enabled = false;
    }

    protected void LightCandle()
    {
        foreach(GameObject candle in candles)
        {
            if(!candle.transform.GetChild(0).gameObject.activeInHierarchy)
            {
                candle.transform.GetChild(0).gameObject.SetActive(true);
                break;
            }
        }
    }
}
