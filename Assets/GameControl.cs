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
    [SerializeField] LevelUnlockInfo nextLevel;
    [SerializeField] List<LevelUnlockInfo> levelsToUnlockHard;
    [SerializeField] GameStats.GameMode gameMode;
    [SerializeField] FrenzyMode frenzyModeScript;
    [SerializeField] SpawnEnemy enemySpawnerScript;
    

    [Header("Time Attack")]
    [SerializeField] TargetTimes targetTimesNormal;
    [SerializeField] TargetTimes targetTimesHard;
    TargetTimes targetTimes;
    

    [Header("SurvivalMode")]
    [SerializeField] TargetTimes survivalTargetTimesNormal;
    [SerializeField] TargetTimes survivalTargetTimesHard;
    [SerializeField] float rampUpTime = 60;
    TargetTimes survivalTargetTimes;
<<<<<<< Updated upstream
=======

    [Header("EndlessMode")]
    [SerializeField] TargetTimes endlessTargetTimesNormal;
    [SerializeField] TargetTimes endlessTargetTimesHard;
    //[SerializeField] SpawnEnemy enemySpawnerScriptEndless;
    [SerializeField] ShrineManager shrineManager;
    TargetTimes endlessTargetTimes;
>>>>>>> Stashed changes
    
    // Start is called before the first frame update
    void Start()
    {
        PlayMusic();

        StartupBasedOnGamemode();
        
    }

    void OnEnable()
    {
        GameStats.levelName = SceneManager.GetActiveScene().name;
        GameStats.currGameMode = gameMode;
        GameStats.ResetDefaults();

        if(nextLevel != null)
        {
            GameStats.nextLevel = nextLevel.levelName;
        }
        
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
<<<<<<< Updated upstream
            StartCoroutine(RampUpDifficulty());
            StartCoroutine(DecreaseRampUpTimeOverTime());
=======
            StartCoroutine(RampUpDifficulty(rampUpTime, 10, 0.05f));
            StartCoroutine(DecreaseRampUpTimeOverTime(10, 120, 10));
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
            yield return new WaitForSeconds(rampUpTime);
            frenzyModeScript.IncreaseFrenzyDamage(10);
            enemySpawnerScript.IncreaseGhostStats(0.05f);
=======
            yield return new WaitForSeconds(rampTime);
            frenzyModeScript.IncreaseFrenzyDamage(frenzyDamageInc);
            enemySpawnerScript.IncreaseGhostStats(ghostStatsInc);
>>>>>>> Stashed changes

        }
    }

    IEnumerator DecreaseRampUpTimeOverTime()
    {
        while(!gameEnded || rampUpTime > 10)
        {
            yield return new WaitForSeconds(180);
            rampUpTime -= 15;

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

<<<<<<< Updated upstream
=======
    public void ResetMap()
    {
        frenzyModeScript.StopFrenzyMode();
        enemySpawnerScript.EnableSpawns();
        enemySpawnerScript.SetWave(0);
        shrineManager.ResetShrines();

        PlayerHealth playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        int healAmount = playerHealth.MaxHealth / 2;
        playerHealth.Heal(healAmount);
    }


>>>>>>> Stashed changes
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
            //float score =  GameStats.completionTime > survivalTargetTimes.timesInSeconds[0] ? survivalTargetTimes.timesInSeconds[0] : GameStats.completionTime;
            if(GameStats.completionTime > prevHighScore || prevHighScore == 0)
            {
                currHighScore = GameStats.completionTime;
                bestRating = GameStats.rating;
            }
        }
        
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
