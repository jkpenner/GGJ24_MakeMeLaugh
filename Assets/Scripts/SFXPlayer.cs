using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SFXPlayer : MonoBehaviour {
    [SerializeField] private AudioClip[] goodSFXSounds;
    [SerializeField] private AudioClip[] badSFXSounds;
    [SerializeField] private AudioClip[] rowCompletedSounds;

    static AudioSource audioSource;

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
    }
    
    public void PlayRandomGoodSFX() {
        Debug.Log("Playing random good SFX");
        audioSource.PlayOneShot(goodSFXSounds[Random.Range(0,goodSFXSounds.Length -1)]);
    }
    
    public void PlayRandomGroupCompletedSFX() {
        audioSource.PlayOneShot(rowCompletedSounds[Random.Range(0,rowCompletedSounds.Length -1)]);
    }
    
    public void PlayRandomBadSFX() {
        audioSource.PlayOneShot(badSFXSounds[Random.Range(0,badSFXSounds.Length -1)]);
    }
}
