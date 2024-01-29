using System;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    //Singleton
    private static MusicPlayer _instance;
    public static MusicPlayer Instance => _instance;

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
        } else {
            _instance = this;
        }
        
        DontDestroyOnLoad(gameObject);
    }
}