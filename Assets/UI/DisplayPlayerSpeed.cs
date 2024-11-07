using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayPlayerSpeed : MonoBehaviour
{
    [SerializeField] Rigidbody2D playerRb;

    [SerializeField] TextMeshProUGUI speedTextBox;
    void Start()
    {
        //speedTextBox = gameObject.GetComponentInChildren<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        speedTextBox.text = Math.Floor(playerRb.velocity.magnitude).ToString();
    }
}
