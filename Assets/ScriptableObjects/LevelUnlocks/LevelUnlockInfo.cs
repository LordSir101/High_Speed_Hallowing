using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/LevelUnlockInfo", order = 1)]
public class LevelUnlockInfo : ScriptableObject
{
    
    // Start is called before the first frame update
    [SerializeField] public string levelName;
    [SerializeField] public GameStats.GameDifficulty difficulty;

}
