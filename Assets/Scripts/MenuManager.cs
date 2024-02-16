using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {
    [SerializeField] private Button easyButton;
    [SerializeField] GameObject easyHighScore;
    [SerializeField] TMP_Text easyHighScoreLabel;
    [SerializeField] private Button mediumButton;
    [SerializeField] GameObject mediumHighScore;
    [SerializeField] TMP_Text mediumHighScoreLabel;
    [SerializeField] private Button hardButton;
    [SerializeField] GameObject hardHighScore;
    [SerializeField] TMP_Text hardHighScoreLabel;
    [SerializeField] private Button endlessButton;
    [SerializeField] GameObject endlessHighScore;
    [SerializeField] TMP_Text endlessHighScoreLabel;

    [SerializeField] private GameSettings easySettings;
    [SerializeField] private GameSettings mediumSettings;
    [SerializeField] private GameSettings hardSettings;
    [SerializeField] private GameSettings endlessSettings;
    
    private void Start() {
        var easyHighScoreValue = PlayerPrefs.GetInt(easySettings.name, 0);
        easyHighScore.SetActive(easyHighScoreValue != 0);
        easyHighScoreLabel.text = easyHighScoreValue.ToString();

        var mediumHighScoreValue = PlayerPrefs.GetInt(mediumSettings.name, 0);
        mediumHighScore.SetActive(mediumHighScoreValue != 0);
        mediumHighScoreLabel.text = mediumHighScoreValue.ToString();

        var hardHighScoreValue = PlayerPrefs.GetInt(hardSettings.name, 0);
        hardHighScore.SetActive(hardHighScoreValue != 0);
        hardHighScoreLabel.text = hardHighScoreValue.ToString();

        var endlessHighScoreValue = PlayerPrefs.GetInt(endlessSettings.name, 0);
        endlessHighScore.SetActive(endlessHighScoreValue != 0);
        endlessHighScoreLabel.text = endlessHighScoreValue.ToString();


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

        endlessButton.onClick.AddListener(() => {
            SFXPlayer.Instance.PlayRandomGoodSFX();
            LoadGamePlayWithSettings(endlessSettings);
        });
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Application.Quit();
        }
    }

    private void LoadGamePlayWithSettings(GameSettings settings)
    {
        StartCoroutine(LoadGamePlayWithSettingsRoutine(settings));
    }

    private IEnumerator LoadGamePlayWithSettingsRoutine(GameSettings settings)
    {
        yield return SceneManager.LoadSceneAsync(GameConsts.GamePlayBuildIndex, LoadSceneMode.Additive);
        
        var gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager == null)
        {
            Debug.Log("Failed to find GameManager in loaded scene");
            yield break;
        }

        settings.maxKeysHeldAtOnce = PlayerPrefs.GetInt(GameConsts.MaxSequenceLengthKey, GameConsts.DefaultMaxKeySequence);

        gameManager.StartGameWithSettings(settings);

        var activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(1));
        yield return null;
        yield return SceneManager.UnloadSceneAsync(activeSceneIndex);
    }
}
