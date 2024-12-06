using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySounds : MonoBehaviour
{
    GameObject currAttackAudioSource; // the audio parent gameobject of the current action the enemy is taking
    //[SerializeField] AudioSource attackSounds;
    // void Start()
    // {
    //     attackAudioSources = transform.parent.GetComponentsInChildren<AudioSource>();
    // }
    // Start is called before the first frame update

    // this function gets called by animation events at specific frames of attack animations
    public void PlayAttackSound()
    {
        AudioSource[] attackAudioSources = currAttackAudioSource.GetComponents<AudioSource>();
        int index = Random.Range(0, attackAudioSources.Length);
        attackAudioSources[index].Play();
        //transform.parent.GetComponentInChildren<AudioSource>().Play();
    }

    public void SetCurrAttackAudio(GameObject audio)
    {
        currAttackAudioSource = audio;
    }
}
