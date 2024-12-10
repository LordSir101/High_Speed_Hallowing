using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
    [SerializeField] GameObject startButtons;
    [SerializeField] GameObject difficultySlectButtons;
    // Start is called before the first frame update
   public void ShowDifficultySelect()
   {
        startButtons.SetActive(false);
        difficultySlectButtons.SetActive(true);
   }
}
