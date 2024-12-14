using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShrineManager : MonoBehaviour
{
    private List<GameObject> shrines;
    private int shrinesActivated = 0;
    [SerializeField] private GameObject shrineParent;
    //[SerializeField] private GameOverPanel gameOverPanel;
    [SerializeField] private InputActionReference inputActionRef;
    [SerializeField] private FrenzyMode frenzyModeScript;
    [SerializeField] private Sprite happySprite;
    [SerializeField] private SpawnEnemy enemySpawner;
    [SerializeField] private AudioSource shrineCleanseSound, shrineUpgradePurchaseSound;
    [SerializeField] public GameObject paymentText;
    private GameObject bigShrine;
    // Start is called before the first frame update
    void Start()
    {
        
        bigShrine = GameObject.FindGameObjectWithTag("EndGoal");

        shrines = new List<GameObject>();
        foreach (Transform childTransform in shrineParent.transform)
        {
            GameObject shrine = childTransform.gameObject;
            if(shrine.tag != "EndGoal")
            {
                shrine.GetComponent<Shrine>().Init(inputActionRef, this, happySprite);
                shrines.Add(shrine);
            }
        }
        GameStats.totalShrines = shrines.Count + 1;
    }

    public void CleanseShrine()
    {
        shrinesActivated += 1;
        GameStats.IncreaseShrinesCleansed();
        
        if(GameStats.gameDifficulty == GameStats.GameDifficulty.tutorial)
        {
            bigShrine.GetComponent<TutEndShrine>().TurnOnGem();
        }
        else
        {
            bigShrine.GetComponent<EndShrine>().TurnOnGem();
        }
        
        IncreaseShrineCleanseCost();

        PlayCleanseSound();
        
    }

    // public function so big shrine can use it too
    public void PlayCleanseSound()
    {
        StartCoroutine(StartCleanseSound());
    }

    public void PlayUpgradeSound()
    {
        //StartCoroutine(StartUpgradeSound());
        shrineUpgradePurchaseSound.Play();
    }

    // IEnumerator StartUpgradeSound()
    // {
    //     shrineUpgradePurchaseSound.Play();
    //     shrineUpgradePurchaseSound.pitch = 2.5f;

    //     while(shrineUpgradePurchaseSound.isPlaying)
    //     {
    //         shrineUpgradePurchaseSound.pitch += 0.1f;
    //         yield return new WaitForSecondsRealtime(0.2f);
    //     }
    // }

    IEnumerator StartCleanseSound()
    {
        shrineCleanseSound.Play();
        shrineCleanseSound.pitch = 1;

        while(shrineCleanseSound.isPlaying)
        {
            shrineCleanseSound.pitch += 0.1f;
            yield return new WaitForSecondsRealtime(0.2f);
        }
    }

    private void IncreaseShrineCleanseCost()
    {
        foreach (GameObject shrine in shrines)
        {
            shrine.GetComponent<Shrine>().Cost *= 2;
        }
    }

    public void SpawnEnemiesAtShrine(Vector3 pos, int num)
    {
        enemySpawner.SpawnAroundPoint(pos, num);
    }

    public void StartFrenzyMode()
    {
        frenzyModeScript.StartFrenzyMode();
    }
}
