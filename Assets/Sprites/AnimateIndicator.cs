using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateIndicator : MonoBehaviour
{
    // Start is called before the first frame update
    public float animationTime = 0.2f;
    public float minScale = 0.1f;
    public float maxScale = 2.5f;
    public float animationTimer = 0f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        animationTimer += Time.deltaTime;

        float ratio = animationTimer / animationTime;
        float scale = Mathf.Lerp(minScale, maxScale, ratio);
        gameObject.transform.localScale = new Vector3(scale, scale, 0);

        transform.position = transform.parent.position;

        if(animationTimer >= animationTime)
        {
            Destroy(gameObject);
        }
    }
}
