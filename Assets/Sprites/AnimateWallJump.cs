using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateWallJump : MonoBehaviour
{
    // Update is called once per frame
    public float animationTime = 0.2f;
    public float minAlpha = 0f;
    public float maxAlpha = 1f;
    public float animationTimer = 0f;
    Color tmp;
    void Start()
    {
        tmp = GetComponent<SpriteRenderer>().color;
        tmp.a = 0f;
        //Thugger.GetComponent<SpriteRenderer>().color = tmp;
                
    }

    // Update is called once per frame
    void Update()
    {
        animationTimer += Time.deltaTime;

        float ratio = animationTimer / animationTime;
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, ratio);
        tmp.a = alpha;

        if(animationTimer >= animationTime)
        {
            Destroy(gameObject);
        }
    }
}
