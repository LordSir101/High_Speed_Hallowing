using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WaveInfo", order = 1)]
public class WaveInfo : ScriptableObject
{
    public List<GameObject> possibleEnemies;
    public List<float> distribution;
    public float time = 60;
}
