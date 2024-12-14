using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] AudioSource[] attackAudios;
    [SerializeField] AudioSource dashAudio;
    [SerializeField] AudioSource jumpAudio;
    [SerializeField] AudioSource grappleShootAudio;
    [SerializeField] AudioSource grappleAudio;
    [SerializeField] AudioSource cooldownRefreshAudio;
    [SerializeField] AudioSource reflectDashAudio;
    void Start()
    {
        //attackAudios = transform.GetChild(0).GetComponents<AudioSource>();
    }

    // Update is called once per frame
    public void PlayAttackSound()
    {
        //attackAudios.pitch = Random.Range(0.9f, 1.2f);
        foreach(AudioSource audio in attackAudios)
        {
            audio.Play();
        }
    }

    public void PlayDashSound()
    {
        // modify the pitch in increments of 0.05
        dashAudio.pitch = 0.95f + 0.05f * Random.Range(0f,2f);
        dashAudio.Play();
    }

    public void PlayJumpSound()
    {
        jumpAudio.pitch = 0.95f + 0.05f * Random.Range(0f,2f);
        jumpAudio.Play();
    }

    public void PlayGrappleShootSound()
    {
        grappleShootAudio.Play();
    }
    public void PlayGrappleSound()
    {
        grappleAudio.Play();
    }

    public void PlayCooldownRefreshAudio()
    {
        cooldownRefreshAudio.pitch = 1.9f + 0.05f * Random.Range(0f,2f);
        cooldownRefreshAudio.Play();
    }

    public void PlayReflectDashAudio()
    {
        reflectDashAudio.Play();
    }
}
