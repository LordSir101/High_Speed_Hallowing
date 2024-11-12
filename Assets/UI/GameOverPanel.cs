using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverPanel : MonoBehaviour
{
    // Start is called before the first frame update

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
        gameObject.SetActive(true);
        PauseControl.PauseGame(true);
    }
}
