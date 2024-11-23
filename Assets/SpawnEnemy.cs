using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    private int numInitialEmemies = 5;

    [Header("Possible Enemy Prefabs")]
    [SerializeField] private List<GameObject> enemyPrefabs;

    private float spawntimer = 0f;
    private float spawnTime = 1;

    private int maxEnemies = 6;
    private bool spawnEnemies = true;

    private List<GameObject> spawnPoints;
    void Start()
    {
        
        GameObject spawnPointParent = GameObject.FindGameObjectWithTag("SpawnPoint");
        spawnPoints = new List<GameObject>();

        foreach (Transform childTransform in spawnPointParent.transform)
        {
            GameObject spawnPoint = childTransform.gameObject;
            spawnPoints.Add(spawnPoint);
        }

    }

    private void SpawnEnemies(int num)
    {
        List<GameObject> spawnPoints = GetValidSpawnPoints();

        for(int i = 0; i < num; i++)
        {
            float spawnRadius = 1f;

            GameObject spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count)];

            float posX = spawnPoint.transform.position.x + UnityEngine.Random.Range(-spawnRadius, spawnRadius);
            float posY = spawnPoint.transform.position.y + UnityEngine.Random.Range(-spawnRadius, spawnRadius);

            Instantiate(enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Count)], new Vector3(posX, posY, 0), transform.rotation);
        }

    }

    public void SpawnAroundPoint(Vector3 pos, int num)
    {
        for(int i = 0; i < num; i++)
        {
            float spawnRadius = 1f;
            float posX = pos.x + UnityEngine.Random.Range(-spawnRadius, spawnRadius);
            float posY = pos.y + UnityEngine.Random.Range(-spawnRadius, spawnRadius);

            Instantiate(enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Count)], new Vector3(posX, posY, 0), transform.rotation);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(spawnEnemies)
        {
            spawntimer += Time.deltaTime;

            if(spawntimer >= spawnTime)
            {
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

                if(enemies.Length < maxEnemies)
                {
                    //SpawnEnemies(1);
                    // List<GameObject> validSpawnPoints = GetValidSpawnPoints();

                    int numEnemiesAvailableToSpawn = maxEnemies - enemies.Length;
                    SpawnEnemies(Mathf.Clamp(numEnemiesAvailableToSpawn, 0, 5));
                    
                }
                spawntimer = 0;
            
            }
        }
        
    }

    private List<GameObject> GetValidSpawnPoints()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        List<GameObject> validPoints = new List<GameObject>();

        float left = Camera.main.transform.position.x - planes[0].distance;
        float right = Camera.main.transform.position.x + planes[1].distance;
        float down = Camera.main.transform.position.y - planes[2].distance;
        float up = Camera.main.transform.position.y + planes[3].distance;

        foreach(GameObject spawnPoint in spawnPoints)
        {
            // check if spawn point is out of view of camera
            Vector2 pos = spawnPoint.transform.position;
            if(pos.x < left || pos.x > right || pos.y < down || pos.y > up)
            {
                validPoints.Add(spawnPoint);
            }
        }

        // foreach(GameObject point in validPoints)
        // {
        //     Debug.Log(point);
        // }

        return validPoints;
    }

    public void SpawnFrenzyWave()
    {
        spawnEnemies = false;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        int numEnemiesAvailableToSpawn = maxEnemies - enemies.Length;
        SpawnEnemies(Mathf.Clamp(numEnemiesAvailableToSpawn, 0, maxEnemies));
    }
}
