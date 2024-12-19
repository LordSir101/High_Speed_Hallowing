using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/TargetTimes", order = 1)]
public class TargetTimes : ScriptableObject
{
    public List<float> timesInSeconds;
}
