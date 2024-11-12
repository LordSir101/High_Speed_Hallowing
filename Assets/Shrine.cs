using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shrine : MonoBehaviour
{
    [SerializeField] private InputActionReference interact;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private ShrineManager shrineManager;
    private GameObject player;

    private int cost = 500;

    // Start is called before the first frame update
    void Start()
    {
        // GameObject textObj = FindGameObjectInChildWithTag(gameObject., "InteractText");
        // interactText = textObj.GetComponent<TextMeshProUGUI>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Tribute(InputAction.CallbackContext context)
    {
        PlayerResourceManager rm = player.GetComponent<PlayerResourceManager>();

        if(rm.Essence >= cost)
        {
            rm.Essence -= 500;
            gameObject.GetComponent<CircleCollider2D>().enabled = false;
            interactText.enabled = false;
            shrineManager.CleanseShrine();
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
            interactText.enabled = true;
            interact.action.performed += Tribute;

        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            interactText.enabled = false;
            interact.action.performed -= Tribute;

        }
    }
}
