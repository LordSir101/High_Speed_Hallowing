using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayPlayerSpeed : MonoBehaviour
{
    [SerializeField] Rigidbody2D playerRb;

    [SerializeField] TextMeshProUGUI speedTextBox;
    PlayerMovement playerMovement;
    void Start()
    {
        //speedTextBox = gameObject.GetComponentInChildren<TextMeshPro>();
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        speedTextBox.text = Math.Floor(playerMovement.currSpeed).ToString();
    }
}
