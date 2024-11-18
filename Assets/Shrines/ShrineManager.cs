using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShrineManager : MonoBehaviour
{
    private List<GameObject> shrines;
    private int shrinesActivated = 0;
    [SerializeField] private GameObject shrineParent;
    [SerializeField] private GameOverPanel gameOverPanel;
    [SerializeField] private InputActionReference inputActionRef;
    // Start is called before the first frame update
    void Start()
    {
        shrines = new List<GameObject>();
        foreach (Transform childTransform in shrineParent.transform)
        {
            GameObject shrine = childTransform.gameObject;
            shrine.GetComponent<Shrine>().Init(inputActionRef, this);
            shrines.Add(shrine);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(shrinesActivated == shrines.Count)
        {
            gameOverPanel.SetWin(true);
        }
    }

    public void CleanseShrine()
    {
        shrinesActivated += 1;
        IncreaseShrineCleanseCost();
    }

    private void IncreaseShrineCleanseCost()
    {
        foreach (GameObject shrine in shrines)
        {
            shrine.GetComponent<Shrine>().Cost *= 2;
        }
    }
}
