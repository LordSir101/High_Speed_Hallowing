using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectInfo : MonoBehaviour
{
    [SerializeField] public string levelName;
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] GameObject starParent;
    [SerializeField] GameObject lockParent;
    [SerializeField] TargetTimes targetTimes;

    public void DisplayHighScore(float time)
    {
        timeText.text = TimeSpan.FromSeconds(time).ToString(@"mm\:ss");
    }

    public void DisplayRating(int rating)
    {
        // clear previous rating in case the difficulty was changed
        for (int i = 0; i < 3; i++)
        {
            starParent.transform.GetChild(i).GetChild(0).gameObject.GetComponent<Image>().enabled = false;
        }

        for (int i = 0; i < rating; i++)
        {
            starParent.transform.GetChild(i).GetChild(0).gameObject.GetComponent<Image>().enabled = true;
        }

        // hide time required reminder text for the 1 star rating by default
        starParent.transform.GetChild(0).GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().enabled = false;

        if(targetTimes.timesInSeconds.Count == 3)
        {
            starParent.transform.GetChild(0).GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().enabled = true;
            starParent.transform.GetChild(0).GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = TimeSpan.FromSeconds(targetTimes.timesInSeconds[2]).ToString(@"mm\:ss");
        }
       

        starParent.transform.GetChild(1).GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().enabled = true;
        starParent.transform.GetChild(1).GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = TimeSpan.FromSeconds(targetTimes.timesInSeconds[1]).ToString(@"mm\:ss");

        starParent.transform.GetChild(2).GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().enabled = true;
        starParent.transform.GetChild(2).GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = TimeSpan.FromSeconds(targetTimes.timesInSeconds[0]).ToString(@"mm\:ss");
    }

    public void DisplayUnlockStatus(bool unlocked)
    {
        // clear previous unlock status
        lockParent.SetActive(false);
        gameObject.GetComponent<Button>().enabled = true;

        if(!unlocked)
        {
            gameObject.GetComponent<Button>().enabled = false;
            lockParent.SetActive(true);
        }
    }
}
