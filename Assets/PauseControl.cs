using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseControl: MonoBehaviour
{
    public static bool gameIsPaused = false;
    [SerializeField] GameObject pauseMenu;
    public InputActionReference pause;
    //[SerializeField] InputActionMap playerActions;
    [SerializeField] private PlayerInput playerInput;

    void Start()
    {
        playerInput.actions.FindActionMap("PauseMenu").Enable();
    }

    private void OnEnable()
    {
        pause.action.performed += TogglePauseMenu;
    }
    private void OnDisable()
    {
        pause.action.performed -= TogglePauseMenu;
    }

    public void PauseGame(bool pause)
    {
        if(pause)
        {
            Time.timeScale = 0f;
            gameIsPaused = true;
            //playerActions.Disable();
            playerInput.actions.FindActionMap("PlayerInput").Disable();
        }
        else 
        {
            Time.timeScale = 1;
            gameIsPaused = false;
            playerInput.actions.FindActionMap("PlayerInput").Enable();
        }

        
    }

    // private void DisablePlayerMovement()
    // {

    // }

    public void ActionPause(float duration, Action callBack)
    {
		//Method used so we don't need to call StartCoroutine everywhere
		//nameof() notation means we don't need to input a string directly.
		//Removes chance of spelling mistakes and will improve error messages if any
		StartCoroutine(PerformSleepCallback(duration, callBack));
    }

     public void HitPause(float duration)
    {
		//Method used so we don't need to call StartCoroutine everywhere
		//nameof() notation means we don't need to input a string directly.
		//Removes chance of spelling mistakes and will improve error messages if any
		StartCoroutine(PerformSleep(duration));
    }

    private IEnumerator PerformSleep(float duration)
    {
		Time.timeScale = 0;
		yield return new WaitForSecondsRealtime(duration); //Must be Realtime since timeScale with be 0 
		Time.timeScale = 1;
	}

	private IEnumerator PerformSleepCallback(float duration, Action callback)
    {
		Time.timeScale = 0;
		yield return new WaitForSecondsRealtime(duration); //Must be Realtime since timeScale with be 0 
		Time.timeScale = 1;
        callback();
	}

    private void TogglePauseMenu(InputAction.CallbackContext context)
    {
        if(gameIsPaused)
        {
            pauseMenu.SetActive(false);
            PauseGame(false);
        }
        else
        {
            pauseMenu.SetActive(true);
            PauseGame(true);
        }
    }
}
