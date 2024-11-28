using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverPanel : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject hud;

    public void SetWin(bool win)
    {
        TextMeshProUGUI text = gameObject.GetComponentInChildren<TextMeshProUGUI>();

        if(win)
        {
            text.text = "You Win";
        }
        else
        {
            text.text = "You Lose";
        }
        hud.SetActive(false);
        gameObject.SetActive(true);
        PauseControl.PauseGame(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
