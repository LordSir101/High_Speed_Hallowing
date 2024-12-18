using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour
{
    //public float duration;
    public AnimationCurve curve;
    public bool shaking = false;
    private GameObject player;

    Coroutine shake;
    // Start is called before the first frame update
    void Start()
    {
         player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartShake(float duration)
    {
        shake = StartCoroutine(Shaking(duration));
    }

    public void StopShake()
    {
        if(shaking)
        {
            StopCoroutine(shake);
        }
        
    }

    // TODO: make this happen even when triggered multiple times really fast.
    public IEnumerator Shaking(float duration)
    {
        shaking = true;
        //Vector3 startPos = transform.position;
        
        float elapsedTime = 0f;

        while(elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float strength = curve.Evaluate(elapsedTime / duration);
            float posX = player.transform.position.x;
            float posY = player.transform.position.y;
            Vector3 playerPos = new Vector3(posX, posY, transform.position.z);
            transform.position = playerPos + Random.insideUnitSphere * strength;
            yield return null;
        }

        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);;
        shaking = false;
    }
}
