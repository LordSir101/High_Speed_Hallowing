using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WaveInfo", order = 1)]
public class WaveInfo : ScriptableObject
{
    public List<GameObject> possibleEnemies;

    // an enemy's id is thier index in the distribution array
    // put 0 in distribution if an enemy should not appear in a level
    public List<float> distribution;
    public float time = 60;
    public int maxEnemies;
}
