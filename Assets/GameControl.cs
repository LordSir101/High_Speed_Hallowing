using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControl : MonoBehaviour
{
    private float gameTime = 0;
    private bool gameEnded = false;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
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
        PauseControl.PauseGame(true);
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
}
