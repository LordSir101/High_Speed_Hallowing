using System.Collections;
using System.Collections.Generic;
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

    public void Sleep(float duration)
    {
		//Method used so we don't need to call StartCoroutine everywhere
		//nameof() notation means we don't need to input a string directly.
		//Removes chance of spelling mistakes and will improve error messages if any
		StartCoroutine(nameof(PerformSleep), duration);
    }

	private IEnumerator PerformSleep(float duration)
    {
		Time.timeScale = 0;
		yield return new WaitForSecondsRealtime(duration); //Must be Realtime since timeScale with be 0 
		Time.timeScale = 1;
	}
}
