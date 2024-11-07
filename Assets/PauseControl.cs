using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseControl : MonoBehaviour
{
    public static bool gameIsPaused = false;

    public static void PauseGame(bool pause)
    {
        if(pause)
        {
            Time.timeScale = 0f;
            gameIsPaused = true;
        }
        else 
        {
            Time.timeScale = 1;
            gameIsPaused = false;
        }
    }
}
