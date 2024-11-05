using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    private int numInitialEmemies = 5;
    [SerializeField] private GameObject ringEnemyPrefab;
    void Start()
    {
        //SpawnEnemies(numInitialEmemies);
    }

    private void SpawnEnemies(int num)
    {
        for(int i = 0; i < num; i++)
        {
            float xPos = UnityEngine.Random.Range(0, 11);
            float yPos = UnityEngine.Random.Range(0, 11);

            Instantiate(ringEnemyPrefab, new Vector3(xPos, yPos, 0), transform.rotation);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
