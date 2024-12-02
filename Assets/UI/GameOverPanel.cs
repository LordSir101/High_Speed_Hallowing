using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverPanel : MonoBehaviour
{
    //TODO: save the map name that was just played to make restart button work correctly

    [SerializeField] TextMeshProUGUI gameOverText;
    [SerializeField] TextMeshProUGUI completionTimeText;
    [SerializeField] TextMeshProUGUI shrinesCleansedText;
    void Start()
    {
        SetWin(GameStats.gameWon);
        SetStats();
    }

    private void SetWin(bool win)
    {
        //TextMeshProUGUI text = gameObject.GetComponentInChildren<TextMeshProUGUI>();

        if(win)
        {
            gameOverText.text = "You Win";
        }
        else
        {
            gameOverText.text = "You Lose";
        }
        //hud.SetActive(false);
        //gameObject.SetActive(true);
        //PauseControl.PauseGame(true);
    }

    private void SetStats()
    {
        completionTimeText.text = TimeSpan.FromSeconds(GameStats.completionTime).ToString(@"mm\:ss");
        shrinesCleansedText.text = $"{GameStats.shrinesCleansed}/{GameStats.totalShrines}";
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
    }
}
