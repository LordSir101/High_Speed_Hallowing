using System;
using System.Collections;
using UnityEngine;

public class PauseControl: MonoBehaviour
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

    public void ActionPause(float duration, Action callBack)
    {
		//Method used so we don't need to call StartCoroutine everywhere
		//nameof() notation means we don't need to input a string directly.
		//Removes chance of spelling mistakes and will improve error messages if any
		StartCoroutine(PerformSleep(duration, callBack));
    }

	private IEnumerator PerformSleep(float duration, Action callback)
    {
		Time.timeScale = 0;
		yield return new WaitForSecondsRealtime(duration); //Must be Realtime since timeScale with be 0 
		Time.timeScale = 1;
        callback();
	}
}
