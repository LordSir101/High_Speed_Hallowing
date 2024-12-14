using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayEssence : MonoBehaviour
{
    PlayerResourceManager rm;

    [SerializeField] TextMeshProUGUI essenceTextBox;
    void Start()
    {
        //speedTextBox = gameObject.GetComponentInChildren<TextMeshPro>();
        rm = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerResourceManager>();
    }

    // Update is called once per frame
    void Update()
    {
        essenceTextBox.text = rm.Essence.ToString();
    }
}
