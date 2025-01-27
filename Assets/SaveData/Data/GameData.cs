using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
   public Dictionary<string, float> highScores;
   public Dictionary<string, float> highScoresHard;
   public Dictionary<string, int> ratings;
   public Dictionary<string, int> ratingsHard;

   public Dictionary<string, bool> levelUnlocks;
   public Dictionary<string, bool> levelUnlocksHard;
   public int version = 1;

    public GameData()
    {
        highScores = new Dictionary<string, float>()
        {
            { "CastleMap1", 0 },
            { "CastleMap2", 0 },
            { "CastleMap3", 0 },
            { "CastleMap4", 0 },
            { "CastleMap5", 0 },
            { "CastleMap6", 0 }
        };

        ratings = new Dictionary<string, int>()
        {
            { "CastleMap1", 0 },
            { "CastleMap2", 0 },
            { "CastleMap3", 0 },
            { "CastleMap4", 0 },
            { "CastleMap5", 0 },
            { "CastleMap6", 0 }
        };

        levelUnlocks = new Dictionary<string, bool>()
        {
            { "CastleMap1", true },
            { "CastleMap2", false },
            { "CastleMap3", false },
            { "CastleMap4", false },
            { "CastleMap5", false },
            { "CastleMap6", false }
        };

        highScoresHard = new Dictionary<string, float>()
        {
            { "CastleMap1", 0 },
            { "CastleMap2", 0 },
            { "CastleMap3", 0 },
            { "CastleMap4", 0 },
            { "CastleMap5", 0 },
            { "CastleMap6", 0 }
        };

        ratingsHard = new Dictionary<string, int>()
        {
            { "CastleMap1", 0 },
            { "CastleMap2", 0 },
            { "CastleMap3", 0 },
            { "CastleMap4", 0 },
            { "CastleMap5", 0 },
            { "CastleMap6", 0 }
        };

        levelUnlocksHard = new Dictionary<string, bool>()
        {
            { "CastleMap1", false },
            { "CastleMap2", false },
            { "CastleMap3", false },
            { "CastleMap4", false },
            { "CastleMap5", false },
            { "CastleMap6", false }
        };
    }

}
