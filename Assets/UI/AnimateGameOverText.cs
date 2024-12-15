using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnimateGameOverText : MonoBehaviour
{
    TextMeshProUGUI gameOverText;
    [SerializeField] AnimationCurve curve;
    // Start is called before the first frame update
    void Start()
    {
        gameOverText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    public void AnimateText(string text, Action callback)
    {
        StartCoroutine(StartAnimation(text, callback));
    }

    private IEnumerator StartAnimation(string text, Action callback)
    {
        Debug.Log("animation");
        float animationTime = 0.5f;
        float startTime = Time.time;

        float maxFontSize = gameOverText.fontSize;
        gameOverText.text = text;

        while(Time.time - startTime < animationTime)
        {
            float fontSizePercent = curve.Evaluate((Time.time - startTime) / animationTime);
            gameOverText.fontSize =  maxFontSize * fontSizePercent;

            // enable text here so that we can set max font size in the editor.
            // have to enable text after the curve begins so it doesn't flash at full size for a frame at the beginning
            gameOverText.enabled = true;
            yield return null;
        }

        yield return new WaitForSeconds(2);
        //SceneControl.LoadScene("CastleMap");
        callback();
    }
}
