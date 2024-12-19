using System;
// using System.Collections;
// using System.Collections.Generic;
// using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    //TODO: save the map name that was just played to make restart button work correctly

    [SerializeField] TextMeshProUGUI gameOverText;
    [SerializeField] TextMeshProUGUI completionTimeText;
    [SerializeField] TextMeshProUGUI shrinesCleansedText;
    [SerializeField] TextMeshProUGUI difficultyText;
    [SerializeField] GameObject starParent;
    void Start()
    {
        SetWin(GameStats.gameWon);
        SetStats();
        SetStars();
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
        difficultyText.text = GameStats.gameDifficulty.ToString();
    }

    private void SetStars()
    {
        for (int i = 0; i < GameStats.rating; i++)
        {
            starParent.transform.GetChild(i).GetChild(0).gameObject.GetComponent<Image>().enabled = true;

            // show required time for 2 and 3 star rating
            
        }


        starParent.transform.GetChild(1).GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().enabled = true;
        starParent.transform.GetChild(1).GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = TimeSpan.FromSeconds(GameStats.completionTargets[0]).ToString(@"mm\:ss");

        starParent.transform.GetChild(2).GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().enabled = true;
        starParent.transform.GetChild(2).GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = TimeSpan.FromSeconds(GameStats.completionTargets[1]).ToString(@"mm\:ss");

    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(GameStats.levelName); 
    }
}
