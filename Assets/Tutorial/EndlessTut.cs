using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndlessTut : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI notificationText;
    [SerializeField] AnimationCurve curve;
    private float maxTextFontSize;
    // Start is called before the first frame update
    void Start()
    {
        if(GameStats.gameDifficulty == GameStats.GameDifficulty.Normal)
        {
            notificationText.text = "In endless mode, ghosts keep \ngrowing stonger over time";

            maxTextFontSize = notificationText.fontSize;

            StartCoroutine(AnimateText(notificationText));
            StartCoroutine(SwitchText());
        }
        
    }

    IEnumerator SwitchText()
    {
        yield return new WaitForSeconds(10);
        notificationText.text = "Cleanse the King shrine to reset the \ngrey shrines to buy more upgrades";
        StartCoroutine(AnimateText(notificationText));
        yield return new WaitForSeconds(10);
        notificationText.enabled = false;
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

    }

}
