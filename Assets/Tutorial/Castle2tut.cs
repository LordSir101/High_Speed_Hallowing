using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Castle2Tut : MonoBehaviour
{
    GameObject player;
    [SerializeField] TutorialText tutText;
    [SerializeField] TextMeshProUGUI currTextDisplay;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] AnimationCurve curve;
    [SerializeField] GameObject cooldownUIParent;

    [SerializeField] ParticleSystem cooldownRefreshEffect;


    int tutTextIndex = 0;
    public bool firstEnemyAttacked = false;
    private bool firstGrapple = false;
    private int tutWaveNum = 0;
    float maxTextFontSize;

    PlayerReflectDash playerReflectDash;


    // TODO: remove if this gets set by a menu later
    void Awake()
    {
        Time.timeScale = 1;
    }

    void Start()
    {
        //playerInput.actions.FindActionMap("Tutorial").Enable();
        if(GameStats.gameDifficulty == GameStats.GameDifficulty.normal)
        {
            playerInput.actions["Grapple"].Disable();

            player = GameObject.FindGameObjectWithTag("Player");
            //player.GetComponent<PlayerResourceManager>().Essence = 400;

            currTextDisplay.text = tutText.text[tutTextIndex];

            //playerMovement = player.GetComponent<PlayerMovement>();
            playerReflectDash = player.GetComponent<PlayerReflectDash>();
            //playerResources = player.GetComponent<PlayerResourceManager>();
            //playerImpact = player.GetComponent<PlayerImpact>();

            cooldownUIParent.transform.GetChild(1).gameObject.SetActive(false);

            currTextDisplay.enabled = true;

            maxTextFontSize = currTextDisplay.fontSize;

            StartCoroutine(CheckForReflectDash());

        }

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

        // wait a second to give the next text time to pop up incase they do a snap dash refresh right away
        yield return new WaitForSeconds(1);

        StartCoroutine(CheckForSnapDashRefresh());

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
        playerInput.actions["Grapple"].Enable();
        playerInput.actions["Grapple"].performed += FirstGrapple;
        cooldownUIParent.transform.GetChild(1).gameObject.SetActive(true);
    }

    private void FirstGrapple(InputAction.CallbackContext context)
    {
        if(!firstGrapple)
        {
            firstGrapple = true;
            NextText();
            playerInput.actions["Grapple"].performed -= FirstGrapple;
        }
    }

    public void NextText()
    {       
        currTextDisplay.enabled = false;
        tutTextIndex += 1;
        currTextDisplay.text = tutText.text[tutTextIndex];
        StartCoroutine(AnimateText(currTextDisplay));
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

}
