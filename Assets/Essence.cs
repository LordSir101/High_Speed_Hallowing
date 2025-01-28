using System.Collections;
using UnityEngine;

public class Essence : MonoBehaviour
{
    private int value, minMultiplier = 2, maxMultiplier = 4, baseValue = 50;
    bool collected = false;


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
        if(other.gameObject.tag == "Player" && !collected)
        {
            other.gameObject.GetComponent<PlayerResourceManager>().Essence += value;
            collected = true;
            StartCoroutine(MoveToPlayer(other.gameObject));
        }
        else
        {
            // stops the soul from going through walls
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
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
