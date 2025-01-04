using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
   public Dictionary<string, float> highScores;
   public Dictionary<string, float> highScoresHard;
   public Dictionary<string, int> ratings;
   public Dictionary<string, int> ratingsHard;

    public GameData()
    {
        highScores = new Dictionary<string, float>()
        {
            { "CastleMap1", 0 },
            { "CastleMap2", 0 },
            { "CastleMap3", 0 },
            { "CastleMap4", 0 }
        };

        ratings = new Dictionary<string, int>()
        {
            { "CastleMap1", 0 },
            { "CastleMap2", 0 },
            { "CastleMap3", 0 },
            { "CastleMap4", 0 }
        };

        highScoresHard = new Dictionary<string, float>()
        {
            { "CastleMap1", 0 },
            { "CastleMap2", 0 },
            { "CastleMap3", 0 },
            { "CastleMap4", 0 }
        };

        ratingsHard = new Dictionary<string, int>()
        {
            { "CastleMap1", 0 },
            { "CastleMap2", 0 },
            { "CastleMap3", 0 },
            { "CastleMap4", 0 }
        };
    }

}
