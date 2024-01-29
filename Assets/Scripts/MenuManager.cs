using System;
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
    
    private void Start() {
        easyButton.onClick.AddListener(() => {
            SFXPlayer.Instance.PlayRandomGoodSFX();
            LoadGamePlayWithSettings(easySettings);
        });
        mediumButton.onClick.AddListener(() => {
            SFXPlayer.Instance.PlayRandomGoodSFX();
            LoadGamePlayWithSettings(mediumSettings);
        });
        hardButton.onClick.AddListener(() => {
            SFXPlayer.Instance.PlayRandomGoodSFX();
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
