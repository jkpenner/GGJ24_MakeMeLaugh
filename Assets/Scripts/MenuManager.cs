using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {
    [SerializeField] private Button easyButton;
    [SerializeField] private Button mediumButton;
    [SerializeField] private Button hardButton;
    
    public enum Difficulty {
        Easy = 0,
        Medium = 1,
        Hard = 2
        
    }
    
    private void Start() {
        easyButton.onClick.AddListener(() => {
            PlayerPrefs.SetInt("Difficulty", (int)Difficulty.Easy);
            SceneManager.LoadScene(1);
        });
        mediumButton.onClick.AddListener(() => {
            PlayerPrefs.SetInt("Difficulty", (int)Difficulty.Medium);
            SceneManager.LoadScene(1);
        });
        hardButton.onClick.AddListener(() => {
            PlayerPrefs.SetInt("Difficulty", (int)Difficulty.Hard);
            SceneManager.LoadScene(1);
        });
    }
}
