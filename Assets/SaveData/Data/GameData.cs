using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
   public Dictionary<string, float> highScores;

    public GameData()
    {
        highScores = new Dictionary<string, float>()
        {
            { "CastleMap1", 0 },
            { "CastleMap2", 0 },
            { "CastleMap3", 0 },
            { "CastleMap4", 0 }
        };
    }

}
