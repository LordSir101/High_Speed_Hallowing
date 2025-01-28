using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CooldownUI : MonoBehaviour
{
    [SerializeField] GameObject grappleCooldownIcon;
    [SerializeField] GameObject dashCooldownIcon;


    [SerializeField] TextMeshProUGUI dashKeybindReminder;
    [SerializeField] TextMeshProUGUI grappleKeybindReminder;
    PlayerInput playerInput;
    InputAction dashInput;
    InputAction grappleInput;

    Image dashIcon;
    RectTransform dashTimerIcon;
    Image grappleIcon;
    RectTransform grappleTimerIcon;

    float origionalCooldownUIHeight;
    float origionalCooldownUIWidth;
    // Start is called before the first frame update
    void Start()
    {
        dashIcon = dashCooldownIcon.transform.GetChild(1).GetComponent<Image>();
        dashTimerIcon = dashCooldownIcon.transform.GetChild(0).GetComponent<RectTransform>();
        grappleIcon = grappleCooldownIcon.transform.GetChild(1).GetComponent<Image>();
        grappleTimerIcon = grappleCooldownIcon.transform.GetChild(0).GetComponent<RectTransform>();

        origionalCooldownUIHeight = dashTimerIcon.sizeDelta.y;
        origionalCooldownUIWidth = dashTimerIcon.sizeDelta.x;

        playerInput = GameObject.FindGameObjectWithTag("Input").GetComponent<PlayerInput>();
        dashInput = playerInput.currentActionMap.FindAction("Dash");
        grappleInput = playerInput.currentActionMap.FindAction("Grapple");
    }

    // Update is called once per frame
    void Update()
    {
        if(playerInput.currentControlScheme == "Controller")
        {
            dashKeybindReminder.text = dashInput.GetBindingDisplayString(InputBinding.MaskByGroup("Controller"));
            grappleKeybindReminder.text = grappleInput.GetBindingDisplayString(InputBinding.MaskByGroup("Controller"));
        }
        else
        {
            dashKeybindReminder.text = dashInput.GetBindingDisplayString(InputBinding.MaskByGroup("M_Keyboard"));
            grappleKeybindReminder.text = grappleInput.GetBindingDisplayString(InputBinding.MaskByGroup("M_Keyboard"));
        }
        
    }

    public void StartDashCooldown(float time)
    {
        StartCoroutine(DashCooldown(time));
    }
    
    IEnumerator DashCooldown(float time)
    {
        float startTime = Time.time;

        dashIcon.color = new Color(dashIcon.color.r, dashIcon.color.g, dashIcon.color.b, 0.1f);

        while(Time.time - startTime < time)
        {
            float ratio = (Time.time - startTime)/time;
            float currheight = Mathf.Lerp(0, origionalCooldownUIHeight, ratio);
            dashTimerIcon.sizeDelta = new Vector2(origionalCooldownUIWidth, currheight);
            yield return null;
        }

        dashIcon.color = new Color(dashIcon.color.r, dashIcon.color.g, dashIcon.color.b, 1f);
    }

    public void StartGrappleCooldown(float time)
    {
        StartCoroutine(GrappleCooldown(time));
    }
    
    IEnumerator GrappleCooldown(float time)
    {
        float startTime = Time.time;

        grappleIcon.color = new Color(grappleIcon.color.r, grappleIcon.color.g, grappleIcon.color.b, 0.1f);

        while(Time.time - startTime < time)
        {
            float ratio = (Time.time - startTime)/time;
            float currheight = Mathf.Lerp(0, origionalCooldownUIHeight, ratio);
            grappleTimerIcon.sizeDelta = new Vector2(origionalCooldownUIWidth, currheight);
            yield return null;
        }

        grappleIcon.color = new Color(grappleIcon.color.r, grappleIcon.color.g, grappleIcon.color.b, 1f);
    }

    public void EndAllCooldowns()
    {
        StopAllCoroutines();
        dashIcon.color = new Color(dashIcon.color.r, dashIcon.color.g, dashIcon.color.b, 1f);
        grappleIcon.color = new Color(grappleIcon.color.r, grappleIcon.color.g, grappleIcon.color.b, 1f);
        //dashTimerIcon.sizeDelta = new Vector2()
    }
}
