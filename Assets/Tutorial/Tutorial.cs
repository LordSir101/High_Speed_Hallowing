using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    GameObject player;
    [SerializeField] TutorialText tutText;
    [SerializeField] GameObject tutorialTextParent;
    [SerializeField] TextMeshProUGUI currTextDisplay;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] InputActionReference toggleText;
    [SerializeField] TextMeshProUGUI toggleReminderText;
    [SerializeField] GameObject buttons;
    int tutTextIndex = 0;
    // Start is called before the first frame update

    // TODO: remove if this gets set by a menu later
    void Awake()
    {
        GameStats.gameDifficulty = GameStats.GameDifficulty.tutorial;
    }

    private void OnEnable()
    {
        toggleText.action.performed += ToggleTutorialText;
    }

    private void OnDisable()
    {
        toggleText.action.performed -= ToggleTutorialText;
    }

    void Start()
    {

        playerInput.actions.FindActionMap("Tutorial").Enable();
        playerInput.actions.FindActionMap("PlayerInput").Disable();

        player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerResourceManager>().Essence = 400;

        currTextDisplay.text = tutText.text[tutTextIndex];

        // disable prev button at the start
        buttons.transform.GetChild(1).gameObject.SetActive(false);
    }

    private void ToggleTutorialText(InputAction.CallbackContext context)
    {
        if(tutorialTextParent.activeInHierarchy)
        {
            tutorialTextParent.SetActive(false);
            playerInput.actions.FindActionMap("PlayerInput").Enable();
            toggleReminderText.text = "Press T to show tutorial";
        }
        else
        {
            tutorialTextParent.SetActive(true);
            playerInput.actions.FindActionMap("PlayerInput").Disable();
            toggleReminderText.text = "Press T to practice";
        }
    }

    public void NextText()
    {
        // if(tutTextIndex < tutText.text.Count - 1)
        // {
        //     tutTextIndex += 1;
        //     currTextDisplay.text = tutText.text[tutTextIndex];
        // }

        if(tutTextIndex == tutText.text.Count - 1)
        {
            // disable next button when at last instruction
            buttons.transform.GetChild(0).gameObject.SetActive(false);
        }
        
        // enable the "prev" button whenever we hit the "next" button
        buttons.transform.GetChild(1).gameObject.SetActive(true);
        
        
    }

    public void PrevText()
    {
        // if(tutTextIndex > 0)
        // {
        //     tutTextIndex -= 1;
        //     currTextDisplay.text = tutText.text[tutTextIndex];
        // }

        if(tutTextIndex == 0)
        {
            // disable next button when at last instruction
            buttons.transform.GetChild(1).gameObject.SetActive(false);
        }
      
        // enable "next" button when "prev" button is pressed
        buttons.transform.GetChild(0).gameObject.SetActive(true);
        
    }


}
