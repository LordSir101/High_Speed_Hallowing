using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControl : MonoBehaviour
{
    private float gameTime = 0;
    private bool gameEnded = false;
    [SerializeField] AudioSource backgroundMusic;
    [SerializeField] PauseControl pauseControl;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        PlayMusic();
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
        pauseControl.PauseGame(true);
        GameStats.gameWon = win;
        GameStats.completionTime = gameTime;
        SceneControl.LoadScene("GameOverMenu");

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

    //************************************************************************************************************************
    // Make sure to get music that can be used for commercial purposes if selling the game. current version cannot be used
    //************************************************************************************************************************
    void PlayMusic()
    {
        backgroundMusic.Play();
    }
}
