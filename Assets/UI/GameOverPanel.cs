using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverPanel : MonoBehaviour
{
    // Start is called before the first frame update

    public void OpenPanel()
    {
        gameObject.SetActive(true);
        PauseControl.PauseGame(true);
    }
}
