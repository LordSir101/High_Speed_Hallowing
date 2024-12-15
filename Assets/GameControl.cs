using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameControl : MonoBehaviour
{
    private float gameTime = 0;
    private bool gameEnded = false;
    [SerializeField] AudioSource backgroundMusic;
    PauseControl pauseControl;
    [SerializeField] AnimateGameOverText gameOverTextAnimateScript;
    // Start is called before the first frame update
    void Start()
    {
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

        string text = win ? "Level Complete" : "Game Over";

        gameOverTextAnimateScript.AnimateText(text, LoadEndScreen);
        

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
