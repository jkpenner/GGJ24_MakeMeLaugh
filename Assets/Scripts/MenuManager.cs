using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {
    [SerializeField] private Button easyButton;
    [SerializeField] private Button mediumButton;
    [SerializeField] private Button hardButton;

    [SerializeField] private GameSettings easySettings;
    [SerializeField] private GameSettings mediumSettings;
    [SerializeField] private GameSettings hardSettings;
    
    public enum Difficulty {
        Easy = 0,
        Medium = 1,
        Hard = 2
        
    }
    
    private void Start() {
        easyButton.onClick.AddListener(() => {
            // PlayerPrefs.SetInt("Difficulty", (int)Difficulty.Easy);
            // SceneManager.LoadScene(1);
            LoadGamePlayWithSettings(easySettings);
        });
        mediumButton.onClick.AddListener(() => {
            // PlayerPrefs.SetInt("Difficulty", (int)Difficulty.Medium);
            // SceneManager.LoadScene(1);
            LoadGamePlayWithSettings(mediumSettings);
        });
        hardButton.onClick.AddListener(() => {
            // PlayerPrefs.SetInt("Difficulty", (int)Difficulty.Hard);
            // SceneManager.LoadScene(1);
            LoadGamePlayWithSettings(hardSettings);
        });
    }

    private void LoadGamePlayWithSettings(GameSettings settings)
    {
        StartCoroutine(LoadGamePlayWithSettingsRoutine(settings));
    }

    private IEnumerator LoadGamePlayWithSettingsRoutine(GameSettings settings)
    {
        yield return SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
        
        var gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager == null)
        {
            Debug.Log("Failed to find GameManager in loaded scene");
            yield break;
        }

        gameManager.StartGameWithSettings(settings);

        var activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(1));
        yield return null;
        yield return SceneManager.UnloadSceneAsync(activeSceneIndex);
    }
}
