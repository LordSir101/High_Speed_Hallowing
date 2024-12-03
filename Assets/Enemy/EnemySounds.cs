using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySounds : MonoBehaviour
{
    // Start is called before the first frame update
    public void PlayAttackSound()
    {
        transform.parent.GetComponentInChildren<AudioSource>().Play();
    }
}
