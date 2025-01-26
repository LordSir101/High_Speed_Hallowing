using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FisherEnemyAnimateSwing : MonoBehaviour
{
    [SerializeField] GameObject swingObject;

    public float animationTime = 0.5f;
    public float minAlpha = 0f;
    public float maxAlpha = 1f;
    public float animationTimer = 0f;
    Color tmp;
    // Start is called before the first frame update
    public void PlaySwing()
    {
        tmp = swingObject.GetComponent<SpriteRenderer>().color;
        tmp.a = 1f;
        swingObject.GetComponent<SpriteRenderer>().color = tmp;
        swingObject.SetActive(true);
    }

    public void EndSwing()
    {
        StartCoroutine(FadeSwing());
    }

    IEnumerator FadeSwing()
    {
        //tmp = swingObject.GetComponent<SpriteRenderer>().color;
        animationTimer = 0f;
        //tmp.a = 0f;
        while(animationTimer <= animationTime)
        {
            animationTimer += Time.deltaTime;

            float ratio = animationTimer / animationTime;
            float alpha = Mathf.Lerp(maxAlpha, minAlpha, ratio);
            tmp.a = alpha;

            swingObject.GetComponent<SpriteRenderer>().color = tmp;

           yield return null;
        }

        swingObject.SetActive(false);
        
    }
    
}
