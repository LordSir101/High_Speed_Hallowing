using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrineManager : MonoBehaviour
{
    private List<GameObject> shrines;
    private int shrinesActivated = 0;
    [SerializeField] private GameObject shrineParent;
    [SerializeField] private GameOverPanel gameOverPanel;
    // Start is called before the first frame update
    void Start()
    {
        shrines = new List<GameObject>();
        foreach (Transform childTransform in shrineParent.transform)
        {
            GameObject shrine = childTransform.gameObject;
            shrines.Add(shrine);
            Debug.Log("added");
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
    }
}
