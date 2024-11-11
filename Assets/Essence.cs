using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Essence : MonoBehaviour
{
    private int value, minValue = 300, MaxValue = 500;


    // Start is called before the first frame update
    void Start()
    {
        value = Random.Range(minValue, MaxValue + 1);
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
            Destroy(gameObject);
        }
    }
}
