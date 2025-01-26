using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingEnemyAnimateSwing : MonoBehaviour
{
    [SerializeField] GameObject left;
    [SerializeField] GameObject right;


    public float animationTime = 0.1f;
    public float minAlpha = 0f;
    public float maxAlpha = 1f;
    public float animationTimer = 0f;
    Color tmp;
    // Start is called before the first frame update
    public void PlaySwing()
    {
        tmp = left.GetComponent<SpriteRenderer>().color;
        tmp.a = 1f;
        left.GetComponent<SpriteRenderer>().color = tmp;
        right.GetComponent<SpriteRenderer>().color = tmp;
    }

    public void EndSwing()
    {
        StartCoroutine(FadeSwing());
    }

    IEnumerator FadeSwing()
    {
        animationTimer = 0f;
        while(animationTimer <= animationTime)
        {
            animationTimer += Time.deltaTime;

            float ratio = animationTimer / animationTime;
            float alpha = Mathf.Lerp(maxAlpha, minAlpha, ratio);
            tmp.a = alpha;

            left.GetComponent<SpriteRenderer>().color = tmp;
            right.GetComponent<SpriteRenderer>().color = tmp;

           yield return null;
        }

        left.SetActive(false);
        right.SetActive(false);
        
    }
    
}
