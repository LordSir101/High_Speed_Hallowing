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
        if(GameStats.gameDifficulty == GameStats.GameDifficulty.normal)
        {
            highScores = data.highScores;
            ratings = data.ratings;
            unlocks = data.levelUnlocks;
        }
        else if(GameStats.gameDifficulty == GameStats.GameDifficulty.hard)
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

        foreach(Transform transform in levelsParent.transform)
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
    }
}
