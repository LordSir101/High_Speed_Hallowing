using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnimateGameOverText : MonoBehaviour
{
    TextMeshProUGUI gameOverText;
    [SerializeField] AnimationCurve curve, starCurve;
    [SerializeField] GameObject starParent;
    [SerializeField] AudioSource levelCompleteSound, starSound;
    // Start is called before the first frame update
    void Start()
    {
        gameOverText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    public void AnimateText(string text, int rating, Action callback)
    {
        StartCoroutine(StartAnimation(text, rating, callback));
    }

    private IEnumerator StartAnimation(string text, int rating, Action callback)
    {
        levelCompleteSound.Play();
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

        starParent.SetActive(true);

        yield return new WaitForSeconds(1);

        float pitch = starSound.pitch;
        //animate stars
        for(int i = 0; i < rating; i++)
        {
            RectTransform rectT = starParent.transform.GetChild(i).GetChild(0).GetComponent<RectTransform>();
            Image fillImage = rectT.gameObject.GetComponent<Image>();

            float targetScaleX = rectT.localScale.x;
            float targetScaleY = rectT.localScale.y;

            animationTime = 1f;
            startTime = Time.time;

            starSound.pitch = pitch + i * 0.05f;
            starSound.Play();

            while(Time.time - startTime < animationTime)
            {
                float scale = starCurve.Evaluate((Time.time - startTime) / animationTime);
                float x =  targetScaleX * scale;
                float y =  targetScaleY * scale;

                rectT.localScale = new Vector2(x, y);

                // enable text here so that we can set max font size in the editor.
                // have to enable text after the curve begins so it doesn't flash at full size for a frame at the beginning
                fillImage.enabled = true;
                yield return null;
            }

        }

       yield return new WaitForSeconds(2);
        //SceneControl.LoadScene("CastleMap");
        callback();
    }
}
