using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Essence : MonoBehaviour
{
    private int value, minMultiplier = 1, maxMultiplier = 3, baseValue = 100;


    // Start is called before the first frame update
    void Start()
    {
        value = baseValue * Random.Range(minMultiplier, maxMultiplier + 1);
        StartCoroutine(ActivateCollider());
    }

    IEnumerator ActivateCollider()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.GetComponent<CircleCollider2D>().enabled = true;
    }

    // // Update is called once per frame
    // void Update()
    // {

    // }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerResourceManager>().Essence += value;
            StartCoroutine(MoveToPlayer(other.gameObject));
        }
    }

    IEnumerator MoveToPlayer(GameObject player)
    {
        float frames = 0;
        float totalFrames = 45f;

        Vector3 start = gameObject.transform.position;
        while(frames < totalFrames)
        {
            float interpolationRatio = frames / totalFrames;

            Vector3 interpolatedPos = Vector3.Lerp(start, player.transform.position, interpolationRatio);
            transform.position = interpolatedPos;
            frames++;
            yield return null;
        }
        Destroy(gameObject);
    }
}
