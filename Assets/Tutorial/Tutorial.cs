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
    // [SerializeField] GameObject tutorialTextParent;
    [SerializeField] TextMeshProUGUI currTextDisplay;
    [SerializeField] private PlayerInput playerInput;
    // [SerializeField] InputActionReference toggleText;
    // [SerializeField] TextMeshProUGUI toggleReminderText;
    // [SerializeField] TextMeshProUGUI tutorialCompleteText;
    // [SerializeField] GameObject buttons;
    [SerializeField] AnimationCurve curve;
    [SerializeField] SpawnEnemy enemySpawner;
    [SerializeField] GameObject cooldownUIParent;
    [SerializeField] ShrineManager shrineManager;
    [SerializeField] Shrine shrine;
    [SerializeField] ParticleSystem cooldownRefreshEffect;

    //[SerializeField] 
    int tutTextIndex = 0;
    public bool firstEnemyAttacked = false;
    private bool firstGrapple = false;
    private int tutWaveNum = 0;
    float maxTextFontSize;

    PlayerMovement playerMovement;
    PlayerReflectDash playerReflectDash;
    PlayerResourceManager playerResources;
    PlayerImpact playerImpact;
    GameObject startingEnemy;
    // Start is called before the first frame update

    // TODO: remove if this gets set by a menu later
    void Awake()
    {
        GameStats.gameDifficulty = GameStats.GameDifficulty.Tutorial;
        Time.timeScale = 1;
        startingEnemy = GameObject.FindGameObjectWithTag("Enemy");
    }

    // private void OnEnable()
    // {
    //     //toggleText.action.performed += ToggleTutorialText;
    // }

    // private void OnDisable()
    // {
    //     toggleText.action.performed -= ToggleTutorialText;
    // }

    void Start()
    {
        //playerInput.actions.FindActionMap("Tutorial").Enable();
        playerInput.actions.FindActionMap("PlayerInput").Disable();
        playerInput.actions["Movement"].Enable();
        playerInput.actions["PointerPosition"].Enable();

        player = GameObject.FindGameObjectWithTag("Player");
        //player.GetComponent<PlayerResourceManager>().Essence = 400;

        currTextDisplay.text = tutText.text[tutTextIndex];

        playerMovement = player.GetComponent<PlayerMovement>();
        playerReflectDash = player.GetComponent<PlayerReflectDash>();
        playerResources = player.GetComponent<PlayerResourceManager>();
        playerImpact = player.GetComponent<PlayerImpact>();

        shrine.numEnemiesToSpawn = 2;

        maxTextFontSize = currTextDisplay.fontSize;

        //playerInput.actions["ReflectDash"].Enable();


        // disable prev button at the start
        //buttons.transform.GetChild(1).gameObject.SetActive(false);
    }

    public void CheckFirstEnemyAttacked()
    {
        if(!firstEnemyAttacked)
        {
            firstEnemyAttacked = true;
            NextText();
            StartCoroutine(CheckFirstEnemyKilled());
        }
    }

    IEnumerator CheckFirstEnemyKilled()
    {
        bool enemyKilled = false;

        while(!enemyKilled)
        {
            if(startingEnemy == null)
            {
                enemyKilled = true;
            }

            yield return null;
        }

        UnlockDash();
    }

    private void UnlockDash()
    {
        NextWave();
        NextText();
        playerInput.actions["Dash"].Enable();
        playerInput.actions["Dash"].performed += DashComplete;
        cooldownUIParent.transform.GetChild(0).gameObject.SetActive(true);
    }

    private void DashComplete(InputAction.CallbackContext context)
    {
        NextText();
        playerInput.actions["Dash"].performed -= DashComplete;
        StartCoroutine(CheckForWallDash());
    }

    IEnumerator CheckForWallDash()
    {
        bool wallJumpPerformed = false;

        while(!wallJumpPerformed)
        {
            if(playerMovement.GetWallJumpCombo() > 0)
            {
                wallJumpPerformed = true;
            }

            yield return null;
        }

        NextText();
        StartCoroutine(CheckForWallDashRefresh());
        //playerInput.actions["Dash"].performed += CheckForWallDashRefresh;
        //StartCoroutine(WaitForTime(10, UnlockSnapDash));

    }

    IEnumerator CheckForWallDashRefresh()
    {
        bool refreshPerformed = false;
        bool cooldownReset = false;

        while(!refreshPerformed)
        {
            if(!cooldownRefreshEffect.isEmitting)
            {
                cooldownReset = true;
            }
            
            if(playerMovement.GetWallJumpCombo() > 0 && cooldownRefreshEffect.isEmitting && cooldownReset)
            {
                // if dash is on cooldown and the action window is active when wall jump is pressed, they successfuly performed the combo
                refreshPerformed = true;
            }
            

            yield return null;
        }
        //NextText();
        UnlockSnapDash();

        // if(playerMovement.GetWallJumpCombo() > 0 && cooldownRefreshEffect.isEmitting)
        // {
        //     // if dash is on cooldown and the action window is active when wall jump is pressed, they successfuly performed the combo
            
        //     //refreshPerformed = true;
            
        //     playerInput.actions["Dash"].performed -= CheckForWallDashRefresh;
            
            
        // }
        
    }

    private void UnlockSnapDash()
    {
        NextWave();
        NextText();
        playerInput.actions["ReflectDash"].Enable();
        StartCoroutine(CheckForReflectDash());
    }

    IEnumerator CheckForReflectDash()
    {
        bool dashPerformed = false;

        while(!dashPerformed)
        {
            if(playerReflectDash.reflectDashing)
            {
                dashPerformed = true;
            }

            yield return null;
        }

        NextText();
        NextWave();
        StartCoroutine(CheckForSnapDashRefresh());
        //StartCoroutine(WaitForTime(5, UnlockShrines));

    }

    IEnumerator CheckForSnapDashRefresh()
    {
        bool refreshPerformed = false;
        bool cooldownReset = false;

        while(!refreshPerformed)
        {
            if(!cooldownRefreshEffect.isEmitting)
            {
                cooldownReset = true;
            }
            
            if(playerReflectDash.reflectDashing && cooldownRefreshEffect.isEmitting && cooldownReset)
            {
                // if dash is on cooldown and the action window is active when wall jump is pressed, they successfuly performed the combo
                refreshPerformed = true;
            }
            

            yield return null;
        }
        NextText();
        UnlockShrines();
    }
        

    private void UnlockShrines()
    {
        playerInput.actions["Interact"].Enable();
        //StartCoroutine(CheckCanCleanseShrine());
        StartCoroutine(CheckShrineCleansed());
    }

    // IEnumerator CheckCanCleanseShrine()
    // {
    //     bool enoughResources = false;

    //     while(!enoughResources)
    //     {
    //         if(playerResources.Essence > 500)
    //         {
    //             enoughResources = true;
    //         }

    //         yield return null;
    //     }

    //     NextText();
    //     StartCoroutine(CheckShrineCleansed());

    // }

    IEnumerator CheckShrineCleansed()
    {
        bool shrineCleansed = false;

        while(!shrineCleansed)
        {
            if(shrineManager.shrinesActivated > 0)
            {
                shrineCleansed = true;
            }

            yield return null;
        }

        NextText();
        StartCoroutine(CheckUpgradeMade());
    }

    IEnumerator CheckUpgradeMade()
    {
        bool upgraded = false;

        while(!upgraded)
        {
            if(shrine.GetNumUpgrades() > 0)
            {
                upgraded = true;
            }

            yield return null;
        }

        NextText();
        NextWave();
        playerInput.actions["Grapple"].Enable();
        playerInput.actions["Grapple"].performed += FirstGrapple;
        cooldownUIParent.transform.GetChild(1).gameObject.SetActive(true);
        //StartCoroutine(CheckUpgradeMade());
    }

    private void FirstGrapple(InputAction.CallbackContext context)
    {
        if(!firstGrapple)
        {
            firstGrapple = true;
            NextText();
            playerInput.actions["Grapple"].performed -= FirstGrapple;
            //StartCoroutine(WaitForTime(10, UnlockDash));
        }
    }

    public void NextText()
    {       
        currTextDisplay.enabled = false;
        tutTextIndex += 1;
        currTextDisplay.text = tutText.text[tutTextIndex];
        StartCoroutine(AnimateText(currTextDisplay));
    }

    private void NextWave()
    {
        tutWaveNum += 1;
        enemySpawner.SetWave(tutWaveNum);
    }

     private IEnumerator AnimateText(TextMeshProUGUI text)
    {
        float animationTime = 0.3f;
        float startTime = Time.time;

        //float maxFontSize = text.fontSize;

        while(Time.time - startTime < animationTime)
        {
            float fontSizePercent = curve.Evaluate((Time.time - startTime) / animationTime);
            text.fontSize =  maxTextFontSize * fontSizePercent;

            // enable text here so that we can set max font size in the editor.
            // have to enable text after the curve begins so it doesn't flash at full size for a frame at the beginning
            text.enabled = true;
            yield return null;
        }

        // Time.timeScale = 0;

        // yield return new WaitForSecondsRealtime(2);

        // Time.timeScale = 1;
    }

    IEnumerator WaitForTime(float duration, Action callback)
    {
        yield return new WaitForSeconds(duration);
        callback();
    }

    // private void ToggleTutorialText(InputAction.CallbackContext context)
    // {
    //     if(tutorialTextParent.activeInHierarchy)
    //     {
    //         tutorialTextParent.SetActive(false);
    //         playerInput.actions.FindActionMap("PlayerInput").Enable();
    //         toggleReminderText.text = "Press T to show tutorial";
    //     }
    //     else
    //     {
    //         tutorialTextParent.SetActive(true);
    //         playerInput.actions.FindActionMap("PlayerInput").Disable();
    //         toggleReminderText.text = "Press T to practice";
    //     }
    // }

    // public void NextText()
    // {
    //     // if(tutTextIndex < tutText.text.Count - 1)
    //     // {
    //     //     tutTextIndex += 1;
    //     //     currTextDisplay.text = tutText.text[tutTextIndex];
    //     // }
    //     tutTextIndex += 1;
    //     currTextDisplay.text = tutText.text[tutTextIndex];

    //     if(tutTextIndex == tutText.text.Count - 1)
    //     {
    //         // disable next button when at last instruction
    //         buttons.transform.GetChild(0).gameObject.SetActive(false);
    //     }
        
    //     // enable the "prev" button whenever we hit the "next" button
    //     buttons.transform.GetChild(1).gameObject.SetActive(true);
        
        
    // }

    // public void PrevText()
    // {
    //     // if(tutTextIndex > 0)
    //     // {
    //     //     tutTextIndex -= 1;
    //     //     currTextDisplay.text = tutText.text[tutTextIndex];
    //     // }
    //     tutTextIndex -= 1;
    //     currTextDisplay.text = tutText.text[tutTextIndex];
    //     if(tutTextIndex == 0)
    //     {
    //         // disable next button when at last instruction
    //         buttons.transform.GetChild(1).gameObject.SetActive(false);
    //     }
      
    //     // enable "next" button when "prev" button is pressed
    //     buttons.transform.GetChild(0).gameObject.SetActive(true);
        
    // }

    // public void FinishTutorial()
    // {
    //     currTextDisplay.gameObject.SetActive(false);
    //     toggleReminderText.gameObject.SetActive(false);

    //     playerInput.actions.FindActionMap("Tutorial").Disable();

    //     StartCoroutine(AnimateTutorialComplete());
    // }

    // private IEnumerator AnimateTutorialComplete()
    // {
    //     float animationTime = 0.5f;
    //     float startTime = Time.time;

    //     float maxFontSize = tutorialCompleteText.fontSize;

    //     while(Time.time - startTime < animationTime)
    //     {
    //         float fontSizePercent = curve.Evaluate((Time.time - startTime) / animationTime);
    //         tutorialCompleteText.fontSize =  maxFontSize * fontSizePercent;

    //         // enable text here so that we can set max font size in the editor.
    //         // have to enable text after the curve begins so it doesn't flash at full size for a frame at the beginning
    //         tutorialCompleteText.enabled = true;
    //         yield return null;
    //     }

    //     yield return new WaitForSeconds(2);
    //     SceneControl.LoadScene("CastleMap");
    // }


}
