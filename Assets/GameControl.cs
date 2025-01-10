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
    // Start is called before the first frame update
    void Start()
    {
        GameStats.completionTargets = targetTimes.timesInSeconds;
        GameStats.levelName = SceneManager.GetActiveScene().name;
        PlayMusic();
        
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
        }
        
    }

    public void SetWin(bool win)
    {
        //pauseControl.PauseGame(true);
        GameStats.gameWon = win;
        GameStats.completionTime = gameTime;

        //int rating = 0;
        if(win)
        {
            GameStats.rating = CalulateRating();

            if(GameStats.completionTime < prevHighScore || prevHighScore == 0)
            {
                currHighScore = GameStats.completionTime;
                bestRating = GameStats.rating;
            }

           
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

    private int CalulateRating()
    {
        int rating = 1;

        foreach(float time in targetTimes.timesInSeconds)
        {
            if(GameStats.completionTime < time)
            {
                rating += 1;
            }
        }

        return rating;
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
        if(GameStats.gameDifficulty == GameStats.GameDifficulty.normal)
        {
            prevHighScore = data.highScores[SceneManager.GetActiveScene().name];
            currHighScore = prevHighScore;
            bestRating = data.ratings[SceneManager.GetActiveScene().name];
        }
        else if(GameStats.gameDifficulty == GameStats.GameDifficulty.hard)
        {
            prevHighScore = data.highScoresHard[SceneManager.GetActiveScene().name];
            currHighScore = prevHighScore;
            bestRating = data.ratingsHard[SceneManager.GetActiveScene().name];
        }
        
    }

    public void SaveData(ref GameData data)
    {
        Debug.Log("saved");
        if(GameStats.gameDifficulty == GameStats.GameDifficulty.normal)
        {
            data.highScores[SceneManager.GetActiveScene().name] = currHighScore;
            data.ratings[SceneManager.GetActiveScene().name] = bestRating;
        }
        else if(GameStats.gameDifficulty == GameStats.GameDifficulty.hard)
        {
            data.highScoresHard[SceneManager.GetActiveScene().name] = currHighScore;
            data.ratingsHard[SceneManager.GetActiveScene().name] = bestRating;
        }
    }
}
