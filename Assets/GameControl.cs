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
    [SerializeField] TargetTimes targetTimes;
    [SerializeField] List<LevelUnlockInfo> levelsToUnlock;
    [SerializeField] List<LevelUnlockInfo> levelsToUnlockHard;
    [SerializeField] GameStats.GameMode gameMode;

    [Header("SurvivalMode")]
    [SerializeField] FrenzyMode frenzyModeScript;
    [SerializeField] TargetTimes survivalTargetTimes;
    // Start is called before the first frame update
    void Start()
    {
        GameStats.completionTargets = targetTimes.timesInSeconds;
        GameStats.levelName = SceneManager.GetActiveScene().name;
        GameStats.currGameMode = gameMode;
        GameStats.ResetDefaults();
        PlayMusic();

        if(gameMode == GameStats.GameMode.Survival)
        {
            StartCoroutine(StartFrenzyMode());
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

            if(gameMode == GameStats.GameMode.Survival && gameTime >= survivalTargetTimes.timesInSeconds[0])
            {
                SetWin(true);
            }
        }
        
    }

    IEnumerator StartFrenzyMode()
    {
        yield return new WaitForSeconds(5);
        frenzyModeScript.StartFrenzyMode();

    }

    public void SetWin(bool win)
    {
        gameEnded = true;
        //pauseControl.PauseGame(true);
        GameStats.gameWon = win;
        GameStats.completionTime = gameTime;

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
