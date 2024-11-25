using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    private int numInitialEmemies = 5;

    // [Header("Possible Enemy Prefabs")]
    // [SerializeField] private List<GameObject> enemyPrefabs;
    [Header("Possible Waves")]
    [SerializeField] List<WaveInfo> waveInfos;
    private WaveInfo currWave;
    private float waveTimer = 0;
    private float waveTime;
    private bool lastWaveTypeSpawned = false;
    

    private float spawntimer = 0f;
    private float spawnTime = 1;

    private int maxEnemies = 6;
    private bool spawnEnemies = true;

    

    private List<GameObject> spawnPoints;
    void Start()
    {
        currWave = waveInfos[0];
        waveTime = 30;

        GameObject spawnPointParent = GameObject.FindGameObjectWithTag("SpawnPoint");
        spawnPoints = new List<GameObject>();

        foreach (Transform childTransform in spawnPointParent.transform)
        {
            GameObject spawnPoint = childTransform.gameObject;
            spawnPoints.Add(spawnPoint);
        }

    }

    void Update()
    {
        // update the wave information if there are more types of waves in waveinfos at certain increments.
        if(!lastWaveTypeSpawned )
        {
            waveTimer += Time.deltaTime;
            if(waveTimer >= waveTime)
            {
                int waveIndex = waveInfos.IndexOf(currWave) + 1;
                
                if(waveIndex > waveInfos.Count - 1)
                {
                    lastWaveTypeSpawned  = true;
                }
                else
                {
                    // the time between each wave gets longer
                    currWave = waveInfos[waveIndex];
                    waveTimer = 0;
                    waveTime *= 1.5f;

                }
            }
        }

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

    private void SpawnEnemies(int num)
    {
        List<GameObject> spawnPoints = GetValidSpawnPoints();

        for(int i = 0; i < num; i++)
        {
            float spawnRadius = 1f;

            GameObject spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count)];

            float posX = spawnPoint.transform.position.x + UnityEngine.Random.Range(-spawnRadius, spawnRadius);
            float posY = spawnPoint.transform.position.y + UnityEngine.Random.Range(-spawnRadius, spawnRadius);

            // get a list of enemies whose current percent of spawns are less that the max percent of spawns in the curr wave info.
            List<GameObject> enemiesThatCanBeSpawned = GetEnemiesThatCanBeSpawned(currWave);

            if(enemiesThatCanBeSpawned.Count != 0)
            {
                Instantiate(enemiesThatCanBeSpawned[UnityEngine.Random.Range(0, enemiesThatCanBeSpawned.Count)], new Vector3(posX, posY, 0), transform.rotation);
            }
            // if enemiesThatCanBeSpawned = 0, than the current enemies spawned follow the correct distribution already, so spawn a random enemy
            else
            {
                Instantiate(currWave.possibleEnemies[UnityEngine.Random.Range(0, currWave.possibleEnemies.Count)], new Vector3(posX, posY, 0), transform.rotation);
            }

        }

    }
    
    public void SpawnAroundPoint(Vector3 pos, int num)
    {
        for(int i = 0; i < num; i++)
        {
            float spawnRadius = 1f;
            float posX = pos.x + UnityEngine.Random.Range(-spawnRadius, spawnRadius);
            float posY = pos.y + UnityEngine.Random.Range(-spawnRadius, spawnRadius);

            List<GameObject> enemiesThatCanBeSpawned = GetEnemiesThatCanBeSpawned(currWave);

            
            if(enemiesThatCanBeSpawned.Count != 0)
            {
                Instantiate(enemiesThatCanBeSpawned[UnityEngine.Random.Range(0, enemiesThatCanBeSpawned.Count)], new Vector3(posX, posY, 0), transform.rotation);
            }
            // if enemiesThatCanBeSpawned = 0, than the current enemies spawned follow the correct distribution already, so spawn a random enemy
            else
            {
                Instantiate(currWave.possibleEnemies[UnityEngine.Random.Range(0, currWave.possibleEnemies.Count)], new Vector3(posX, posY, 0), transform.rotation);
            }

            
        }
    }

    private List<GameObject> GetEnemiesThatCanBeSpawned(WaveInfo wave)
    {
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        float totalEnemies = allEnemies.Length;

        // if no enemies exist or there is only 1 possible enemy, just return the possible enemies list
        if(totalEnemies == 0 || wave.possibleEnemies.Count == 1)
        {
            return wave.possibleEnemies;
        }

        int[] currDistribution = new int[wave.distribution.Count];

        List<GameObject> validEnemies = new List<GameObject>();

        foreach(GameObject enemy in allEnemies)
        {
            currDistribution[enemy.GetComponent<EnemyInfo>().id] += 1;
        }

        for(int index = 0; index < currDistribution.Length; index++)
        {
            if(currDistribution[index] / totalEnemies * 100 < wave.distribution[index])
            {
                validEnemies.Add(wave.possibleEnemies[index]);
            }
        }
        Debug.Log(validEnemies.Count);
        return validEnemies;
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
