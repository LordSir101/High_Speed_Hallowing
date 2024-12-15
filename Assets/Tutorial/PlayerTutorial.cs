using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTutorial : MonoBehaviour
{
    [SerializeField] Tutorial tuorialScript;
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            tuorialScript.CheckFirstEnemyAttacked();   
        }
    }
}
