using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayEssence : MonoBehaviour
{
    [SerializeField] PlayerResourceManager rm;

    [SerializeField] TextMeshProUGUI essenceTextBox;
    void Start()
    {
        //speedTextBox = gameObject.GetComponentInChildren<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        essenceTextBox.text = rm.Essence.ToString();
    }
}
