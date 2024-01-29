using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SFXPlayer : MonoBehaviour {
    [SerializeField] private AudioClip[] goodSFXSounds;
    [SerializeField] private AudioClip[] badSFXSounds;
    [SerializeField] private AudioClip[] rowCompletedSounds;

    AudioSource audioSource;
    private static SFXPlayer _instance;
    public static SFXPlayer Instance => _instance;

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
        
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
        } else {
            _instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }
    
    public void PlayRandomGoodSFX() {
        Debug.Log("Playing random good SFX");
        audioSource.PlayOneShot(goodSFXSounds[Random.Range(0,goodSFXSounds.Length -1)]);
    }
    
    public void PlayRandomGroupCompletedSFX() {
        audioSource.PlayOneShot(rowCompletedSounds[Random.Range(0,rowCompletedSounds.Length -1)]);
    }
    
    public void PlayRandomFailedSFX() {
        audioSource.PlayOneShot(badSFXSounds[Random.Range(0,badSFXSounds.Length -1)]);
    }
}
