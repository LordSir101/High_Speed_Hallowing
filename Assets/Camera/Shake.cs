using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour
{
    //public float duration;
    public AnimationCurve curve;
    public bool shaking = false;
    private GameObject player;

    private float zPos; // keep zPos constant to avoid the black screen bug. If camera z > -1, cant see the sprites on screen

    Coroutine shake;
    // Start is called before the first frame update
    void Start()
    {
         player = GameObject.FindGameObjectWithTag("Player");
         zPos = transform.position.z;
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

            // I think this is needed to stop the screen from blacking out?
            transform.position = new Vector3(player.transform.position.x, player.transform.position.y, zPos);
            shaking = false;
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
            Vector2 playerPos = new Vector2(posX, posY);
            Vector2 shake = playerPos + Random.insideUnitCircle * strength;
            transform.position = new Vector3(shake.x, shake.y, zPos);
            yield return null;
        }

        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, zPos);
        shaking = false;
    }
}
