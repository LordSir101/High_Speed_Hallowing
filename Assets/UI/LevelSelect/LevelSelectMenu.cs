using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectMenu : MonoBehaviour, IDataPersistence
{
    GameData data;
    Dictionary<string, float> highScores;
    Dictionary<string, int> ratings;
    Dictionary<string, bool> unlocks;
    [SerializeField] GameObject levelsParent;
    [SerializeField] GameObject levelSelectParent;
    [SerializeField] GameObject difficultySlectButtons;
    List<GameObject> pages;
    int pageNum = 0;
    [SerializeField] GameObject nextBtn, prevBtn;

    void Start()
    {
        pages = new List<GameObject>();

        foreach(Transform transform in levelsParent.transform)
        {
            pages.Add(transform.gameObject);   
        }
    }
    public void LoadData(GameData data)
    {
        this.data = data;
        
    }

    public void SaveData(ref GameData data)
    {
        return;
    }

    private void LoadDataBasedOnDifficulty()
    {
        if(GameStats.gameDifficulty == GameStats.GameDifficulty.Normal)
        {
            highScores = data.highScores;
            ratings = data.ratings;
            unlocks = data.levelUnlocks;
        }
        else if(GameStats.gameDifficulty == GameStats.GameDifficulty.Hard)
        {
            highScores = data.highScoresHard;
            ratings = data.ratingsHard;
            unlocks = data.levelUnlocksHard;
        }
    }
    public void ShowLevelSelect()
    {
        difficultySlectButtons.SetActive(false);
        levelSelectParent.SetActive(true);
        LoadDataBasedOnDifficulty();

        // pages = new List<GameObject>();

        // foreach(Transform transform in levelsParent.transform)
        // {
        //     pages.Add(transform.gameObject);   
        // }

        DisplayPage(0);
    }

    private void DisplayPage(int num)
    {
        pages[num].SetActive(true);

        foreach(Transform transform in pages[num].transform)
        {
            LevelSelectInfo info = transform.gameObject.GetComponent<LevelSelectInfo>();
                
            float time = highScores[info.levelName];
            int rating = ratings[info.levelName];
            bool unlocked = unlocks[info.levelName];

            info.DisplayHighScore(time);
            info.DisplayRating(rating);
            info.DisplayUnlockStatus(unlocked);
        }
    }

    public void ShowDifficultySelect()
    {
        difficultySlectButtons.SetActive(true);
        levelSelectParent.SetActive(false);

        SetDefaults();
        
    }

    public void NextPage()
    {
        // if(tutTextIndex < tutText.text.Count - 1)
        // {
        //     tutTextIndex += 1;
        //     currTextDisplay.text = tutText.text[tutTextIndex];
        // }
        pages[pageNum].SetActive(false);

        pageNum += 1;
        //currTextDisplay.text = tutText.text[tutTextIndex];

        if(pageNum == pages.Count - 1)
        {
            // disable next button when at last instruction
            nextBtn.SetActive(false);
        }
        
        // enable the "prev" button whenever we hit the "next" button
        prevBtn.SetActive(true);

        DisplayPage(pageNum);
        
        
    }

    public void PrevPage()
    {
        // if(tutTextIndex > 0)
        // {
        //     tutTextIndex -= 1;
        //     currTextDisplay.text = tutText.text[tutTextIndex];
        // }
        pages[pageNum].SetActive(false);

        pageNum -= 1;
        //currTextDisplay.text = tutText.text[tutTextIndex];
        if(pageNum == 0)
        {
            // disable next button when at last instruction
            prevBtn.SetActive(false);
        }
      
        // enable "next" button when "prev" button is pressed
        nextBtn.SetActive(true);

        DisplayPage(pageNum);
        
    }

    private void SetDefaults()
    {
        pages[pageNum].SetActive(false);
        pageNum = 0;
        prevBtn.SetActive(false);
        nextBtn.SetActive(true);
    }
}
