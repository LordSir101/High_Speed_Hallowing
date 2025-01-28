using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Castle5Tut : MonoBehaviour
{
 
    [SerializeField] TextMeshProUGUI currTextDisplay;
   
    void Start()
    {
        StartCoroutine(DisplayText());

    }

    IEnumerator DisplayText()
    {
        currTextDisplay.enabled = true;

        currTextDisplay.text = "Stay alive as long as possible!";

        yield return new WaitForSeconds(10);

        currTextDisplay.enabled = false;

    }

}
