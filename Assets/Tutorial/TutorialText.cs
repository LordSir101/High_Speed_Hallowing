using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/TutorialText", order = 1)]
public class TutorialText : ScriptableObject
{
    public List<string> text;
}

