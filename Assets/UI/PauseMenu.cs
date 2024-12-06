using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] Toggle toggle;

    void Start()
    {
        toggle.onValueChanged.AddListener((value) =>
        {
            Mute(value);
        });
    }
    

    public void Mute(bool value)
    {
        if(value)
        {
            AudioListener.volume = 0;
        }
        else 
        {
            AudioListener.volume = 1;
        }
    }

}
