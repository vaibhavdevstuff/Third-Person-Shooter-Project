using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioHandler : MonoBehaviour
{
    public AudioClip[] footstepSound;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponentInChildren<AudioSource>();
    }

    public void PlayFootstepSound()
    {
        if (!_audioSource.isPlaying)
        {
            _audioSource.clip = footstepSound[Random.Range(0, footstepSound.Length)];
            _audioSource.Play();
        }
    }

    public void PlayGunSound(AudioClip gunSound)
    {
        _audioSource.clip = gunSound;
        _audioSource.Play();
    }











    
}//class
