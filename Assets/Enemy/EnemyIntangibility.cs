using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIntangibility : MonoBehaviour
{
    // TODO: consider making the enemy unable to be attacked if intangible?
    [SerializeField] List<SpriteRenderer> sprites;
    [SerializeField] GameObject raycastBuffer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    // void Update()
    // {
        
    // }

    private void OnTriggerEnter2D(Collider2D other)
    {
        foreach(SpriteRenderer sprite in sprites)
        {
            Color color = sprite.color;
            color.a = 0.3f;
            sprite.color = color;
        }

        raycastBuffer.SetActive(false);
        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        foreach(SpriteRenderer sprite in sprites)
        {
            Color color = sprite.color;
            color.a = 1f;
            sprite.color = color;
        }

        raycastBuffer.SetActive(true);
    }

    // keep ghost intangible when passing through overlaping colliders
    private void OnTriggerStay2D(Collider2D other)
    {
        if(raycastBuffer.activeInHierarchy)
        {
            foreach(SpriteRenderer sprite in sprites)
            {
                Color color = sprite.color;
                color.a = 0.3f;
                sprite.color = color;
            }

            raycastBuffer.SetActive(false);
        }
    }
}
