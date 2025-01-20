using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour, IDataPersistence
{
    private float gameTime = 0;
    private bool gameEnded = false;

    // data persistence
    private float prevHighScore;
    private float currHighScore;
    private int bestRating;

    //************
    [SerializeField] AudioSource backgroundMusic;
    PauseControl pauseControl;
    [SerializeField] AnimateGameOverText gameOverTextAnimateScript;
    
    [SerializeField] List<LevelUnlockInfo> levelsToUnlock;
    [SerializeField] List<LevelUnlockInfo> levelsToUnlockHard;
    [SerializeField] GameStats.GameMode gameMode;
    
    

    [Header("Time Attack")]
    [SerializeField] TargetTimes targetTimesNormal;
    [SerializeField] TargetTimes targetTimesHard;
    TargetTimes targetTimes;
    

    [Header("SurvivalMode")]
    [SerializeField] FrenzyMode frenzyModeScript;
    [SerializeField] TargetTimes survivalTargetTimesNormal;
    [SerializeField] TargetTimes survivalTargetTimesHard;
    [SerializeField] SpawnEnemy enemySpawnerScript;
    [SerializeField] float rampUpTime = 40;
    TargetTimes survivalTargetTimes;
    
    // Start is called before the first frame update
    void Start()
    {
        GameStats.levelName = SceneManager.GetActiveScene().name;
        GameStats.currGameMode = gameMode;
        GameStats.ResetDefaults();
        PlayMusic();

        StartupBasedOnGamemode();
        
    }

    private void StartupBasedOnGamemode()
    {
        if(gameMode == GameStats.GameMode.Survival)
        {
            if(GameStats.gameDifficulty == GameStats.GameDifficulty.Normal)
            {
                survivalTargetTimes = survivalTargetTimesNormal;
            }
            else if(GameStats.gameDifficulty == GameStats.GameDifficulty.Hard)
            {
                survivalTargetTimes = survivalTargetTimesHard;
            }

            GameStats.completionTargets = survivalTargetTimes.timesInSeconds;
            StartCoroutine(StartFrenzyMode());
            StartCoroutine(RampUpDifficulty());
            StartCoroutine(DecreaseRampUpTimeOverTime());
        }
        else if(gameMode == GameStats.GameMode.TimeAttack)
        {
            if(GameStats.gameDifficulty == GameStats.GameDifficulty.Normal)
            {
                targetTimes = targetTimesNormal;
            }
            else if(GameStats.gameDifficulty == GameStats.GameDifficulty.Hard)
            {
                targetTimes = targetTimesHard;
            }

            GameStats.completionTargets = targetTimes.timesInSeconds;
        }
    }

    IEnumerator RampUpDifficulty()
    {
        while(!gameEnded)
        {
            yield return new WaitForSeconds(rampUpTime);
            frenzyModeScript.IncreaseFrenzyDamage(10);
            enemySpawnerScript.IncreaseGhostStats(0.1f);
            Debug.Log("stats increased");

        }
    }

    IEnumerator DecreaseRampUpTimeOverTime()
    {
        while(!gameEnded || rampUpTime > 10)
        {
            yield return new WaitForSeconds(90);
            Debug.Log("ramp up time decreased");
            rampUpTime -= 5;

        }
    }

    void Awake()
    {
        Time.timeScale = 1;
        pauseControl = GetComponent<PauseControl>();
        // un pause game in case game was restarted through pause menu
        pauseControl.PauseGame(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(!gameEnded)
        {
            gameTime += Time.deltaTime;

            // if(gameMode == GameStats.GameMode.Survival && gameTime >= survivalTargetTimes.timesInSeconds[0])
            // {
            //     SetWin(true);
            // }
        }
        
    }

    IEnumerator StartFrenzyMode()
    {
        yield return new WaitForSeconds(5);
        frenzyModeScript.StartFrenzyMode();

    }

    public void SetWin(bool win)
    {
        frenzyModeScript.StopFrenzyMode();
        gameEnded = true;
        //pauseControl.PauseGame(true);
        GameStats.completionTime = gameTime;

        // if the player survived for the minimum time in survival mode, change gamestae to win
        if(gameMode == GameStats.GameMode.Survival && gameTime >= survivalTargetTimes.timesInSeconds[2])
        {
            win = true;
        }

        GameStats.gameWon = win;

        //int rating = 0;
        if(win)
        {

            GameStats.rating = CalulateRating();
            CheckHighScores();

            
        }

        string text = win ? "Level Complete" : "Game Over";

        gameOverTextAnimateScript.AnimateText(text, GameStats.rating, LoadEndScreen);
        
        // TextMeshProUGUI text = gameObject.GetComponentInChildren<TextMeshProUGUI>();

        // if(win)
        // {
        //     text.text = "You Win";
        // }
        // else
        // {
        //     text.text = "You Lose";
        // }
        // hud.SetActive(false);
        // gameObject.SetActive(true);
        // PauseControl.PauseGame(true);
    }

    private void UnlockLevels(ref GameData data)
    {
        if(GameStats.gameDifficulty == GameStats.GameDifficulty.Normal)
        {
            foreach(LevelUnlockInfo levelToUnlock in levelsToUnlock)
            {
                if(levelToUnlock.difficulty == GameStats.GameDifficulty.Normal)
                {
                    data.levelUnlocks[levelToUnlock.levelName] = true;
                }
                else
                {
                    data.levelUnlocksHard[levelToUnlock.levelName] = true;
                }
            }
        }
        else if(GameStats.gameDifficulty == GameStats.GameDifficulty.Hard)
        {
            foreach(LevelUnlockInfo levelToUnlock in levelsToUnlockHard)
            {
                if(levelToUnlock.difficulty == GameStats.GameDifficulty.Normal)
                {
                    data.levelUnlocks[levelToUnlock.levelName] = true;
                }
                else
                {
                    data.levelUnlocksHard[levelToUnlock.levelName] = true;
                }
            }
        }
        
    }

    private int CalulateRating()
    {
        int rating = 0;
        if(gameMode == GameStats.GameMode.TimeAttack)
        {
            rating = 1;
            foreach(float time in targetTimes.timesInSeconds)
            {
                if(GameStats.completionTime < time)
                {
                    rating += 1;
                }
            }
        }
        else if(gameMode == GameStats.GameMode.Survival)
        {
            foreach(float time in survivalTargetTimes.timesInSeconds)
            {
                if(GameStats.completionTime >= time)
                {
                    rating += 1;
                }
            }
        }

        Debug.Log(rating);

        return rating;
    }

    private void CheckHighScores()
    {
        if(gameMode == GameStats.GameMode.TimeAttack)
        {
            if(GameStats.completionTime < prevHighScore || prevHighScore == 0)
            {
                currHighScore = GameStats.completionTime;
                bestRating = GameStats.rating;
            }
        }
        else if(gameMode == GameStats.GameMode.Survival)
        {
            float score =  GameStats.completionTime > survivalTargetTimes.timesInSeconds[0] ? survivalTargetTimes.timesInSeconds[0] : GameStats.completionTime;
            if(score > prevHighScore || prevHighScore == 0)
            {
                currHighScore = score;
                bestRating = GameStats.rating;
            }
        }

        Debug.Log(currHighScore);
        
    }

    private void LoadEndScreen()
    {
        SceneControl.LoadScene("GameOverMenu");
    }

    void PlayMusic()
    {
        backgroundMusic.Play();
    }

    // These get called when scene is loaded, by dataPersitenceManager
    public void LoadData(GameData data)
    {
        if(GameStats.gameDifficulty == GameStats.GameDifficulty.Normal)
        {
            prevHighScore = data.highScores[SceneManager.GetActiveScene().name];
            currHighScore = prevHighScore;
            bestRating = data.ratings[SceneManager.GetActiveScene().name];
        }
        else if(GameStats.gameDifficulty == GameStats.GameDifficulty.Hard)
        {
            prevHighScore = data.highScoresHard[SceneManager.GetActiveScene().name];
            currHighScore = prevHighScore;
            bestRating = data.ratingsHard[SceneManager.GetActiveScene().name];
        }
        
    }

    public void SaveData(ref GameData data)
    {
        if(GameStats.gameDifficulty == GameStats.GameDifficulty.Normal)
        {
            data.highScores[SceneManager.GetActiveScene().name] = currHighScore;
            data.ratings[SceneManager.GetActiveScene().name] = bestRating;
        }
        else if(GameStats.gameDifficulty == GameStats.GameDifficulty.Hard)
        {
            data.highScoresHard[SceneManager.GetActiveScene().name] = currHighScore;
            data.ratingsHard[SceneManager.GetActiveScene().name] = bestRating;
        }

        if(GameStats.gameWon)
        {
            UnlockLevels(ref data);
        }
        
    }
}
