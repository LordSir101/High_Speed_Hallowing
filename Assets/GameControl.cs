using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour
{
    private float gameTime = 0;
    private bool gameEnded = false;
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

    //************************************************************************************************************************
    // TODO Make sure to get music that can be used for commercial purposes if selling the game. current version cannot be used
    //************************************************************************************************************************
    void PlayMusic()
    {
        backgroundMusic.Play();
    }
}
