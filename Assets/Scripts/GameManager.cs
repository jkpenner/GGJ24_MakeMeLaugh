using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum GameState
{
    Initializing,
    GroupStarting,
    GroupActive,
    GroupDespawning,
    GroupComplete,
    GameOver,
    GameVictory
}

public class GameManager : MonoBehaviour
{
    [SerializeField] GameSettings settings;
    [SerializeField] KeySequenceManager sequence;
    [SerializeField] UserInterface ui;

    Keyboard current;
    Key[] keys;
    HashSet<Key> heldKeys = new HashSet<Key>();


    private GameState state = GameState.Initializing;
    private bool isGameOver = false;

    private int currentLifes;
    private float time;
    private int streak;
    private int score;

    public IEnumerator Start()
    {
        current = Keyboard.current;
        if (current is null)
        {
            Debug.LogWarning("No keyboard found");
        }

        keys = new Key[(int)Key.IMESelected - 1];
        for (int i = 1; i < (int)Key.IMESelected; i++)
        {
            keys[i - 1] = (Key)i;
        }

        sequence.GroupKeyIndexChanged += OnGroupKeyIndexChanged;
        sequence.GroupCompleted += OnGroupCompleted;
        sequence.GroupFailed += OnGroupFailed;
        sequence.KeyEventTriggered += OnKeyEvent;

        ui.Prompts.PromptsCleared += OnPromptsCleared;



        if (settings is null)
        {
            yield break;
        }

        yield return null;

        StartGameWithSettings(settings);
    }

    public void StartGameWithSettings(GameSettings settings)
    {
        this.settings = settings;

        currentLifes = settings.lifeCount;
        ui.SetLifes(currentLifes);
        ui.SetMultiplier(1);
        ui.SetScore(score);
        ui.SetTimer(0f);


        ui.SetGameSettings(settings);

        sequence.SetGameSettings(settings);
        sequence.StartNextGroup();
        SetGameState(GameState.GroupActive);

        time = 0f;
        ui.SetTimer(time);
    }

    private void Update()
    {
        if (current != null && current.escapeKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene(GameConsts.MainMenuBuildIndex);
        }

        if (state == GameState.GroupActive)
        {
            time += Time.deltaTime;
            ui.SetTimer(time);
        }

        if (state == GameState.GroupComplete && heldKeys.Count == 0)
        {
            sequence.StartNextGroup();

            if (sequence.IsComplete)
            {
                if (settings.mode == GameMode.Random && settings.infinite)
                {
                    sequence.Regenerate();
                    sequence.StartNextGroup();
                    SetGameState(GameState.GroupActive);
                }
                else
                {
                    SetGameState(GameState.GameVictory);
                }                
            }
            else
            {
                SetGameState(GameState.GroupActive);
            }
        }

        // Temp reload sceen after all keys are releasse when in a game over state
        if (state == GameState.GameOver || state == GameState.GameVictory)
        {
            if (current.spaceKey.wasPressedThisFrame)
            {
                SceneManager.LoadScene(GameConsts.MainMenuBuildIndex);
            }
        }

        ProcessKeyEvents();
    }

    private void ProcessKeyEvents()
    {
        foreach (Key key in keys)
        {
            try
            {
                if (current[key].wasPressedThisFrame)
                {
                    OnKeyPressEvent(key);
                }

                if (current[key].wasReleasedThisFrame)
                {
                    OnKeyReleaseEvent(key);
                }
            }
            catch
            {
                // Debug.LogError($"Key {key} received {e}");
            }
        }
    }

    private void SetGameState(GameState state)
    {
        if (this.state == state)
        {
            return;
        }

        this.state = state;
        switch (this.state)
        {
            case GameState.GroupDespawning:
                StartCoroutine(DespawnPromptsRoutine());
                break;
            case GameState.GameOver:
                OnGameOverEntered();
                break;
            case GameState.GameVictory:
                OnGameVictoryEntered();
                break;
        }
    }

    private IEnumerator DespawnPromptsRoutine()
    {
        // Wait for all prompts to stop moving before despawning them. This will
        // allow the player to see what keys were missed for a breif period.
        yield return new WaitUntil(() => !ui.Prompts.HasAnyMovingPrompts());
        yield return new WaitForSeconds(0.5f);

        if (ui.Prompts.AnyActivePrompts())
        {
            ui.Prompts.DespawnAll();
        }
        else
        {
            OnPromptsCleared();
        }

        ui.Keyboard.ResetAllKeys();
    }

    private void OnKeyEvent(KeyEventArgs args)
    {
        if (state == GameState.GroupActive)
        {
            if (args.EventType == KeyEventType.WrongKeyPressed)
            {
                ui.Notification.ShowMessage(
                    "Streak Broken",
                    "Hit the wrong key",
                    GameConsts.Red
                );

                SFXPlayer.Instance.PlayRandomFailedSFX();
            }
            else if (args.EventType == KeyEventType.KeyReleased)
            {
                ui.Notification.ShowMessage(
                    "Streak Broken",
                    "Release a key too early",
                    GameConsts.Red
                );

                SFXPlayer.Instance.PlayRandomFailedSFX();
            }
            else if (args.EventType == KeyEventType.Success)
            {
                SFXPlayer.Instance.PlayRandomGoodSFX();
            }
        }


        if (args.EventType == KeyEventType.Success)
        {
            AddKeyPressScore();

            if (ui.Prompts.PromptIndexMap.TryGetValue(args.KeyIndex, out var prompt))
            {
                prompt.SetAsSuccess();
            }
        }
        else
        {
            ClearStreak();

            if (ui.Prompts.PromptIndexMap.TryGetValue(args.KeyIndex, out var prompt))
            {
                prompt.SetAsFailed();

                var key = args.Group.Keys[args.KeyIndex];
                if (ui.Keyboard.TryGetKeyVisual(key, out var keyVisual))
                {
                    keyVisual.SetErrorKey(true);
                }
            }
        }
    }

    private void OnGroupKeyIndexChanged(KeySequenceGroupEventArgs args)
    {
        if (!args.Group.IsCompleted)
        {
            ui.Prompts.SpawnKeyPrompt(args.Group, args.KeyIndex);

            var key = args.Group.Keys[args.KeyIndex];
            if (ui.Keyboard.TryGetKeyVisual(key, out var keyVisual))
            {
                keyVisual.SetActiveKey(true);
            }
        }
    }

    private void OnGroupCompleted(KeySequenceGroupEventArgs args)
    {
        ui.Notification.ShowMessage(
           "Sequence Complete",
           "Release keys to start next sequence",
           GameConsts.Green
       );

        AddGroupCompleteScore();

        SFXPlayer.Instance.PlayRandomGroupCompletedSFX();

        // Group was successfully completed
        Debug.Log("Successfully completed a group");
        SetGameState(GameState.GroupDespawning);
    }

    private void OnGroupFailed(KeySequenceGroupEventArgs args)
    {
        Debug.Log("Failed to completed a group");

        if (!isGameOver)
        {
            currentLifes -= 1;
            ui.SetLifes(currentLifes);

            if (currentLifes <= 0)
            {
                isGameOver = true;
            }
        }

        SetGameState(GameState.GroupDespawning);
    }

    private void OnPromptsCleared()
    {
        if (isGameOver)
        {
            SetGameState(GameState.GameOver);
        }
        else
        {
            SetGameState(GameState.GroupComplete);
        }
    }

    private void OnGameVictoryEntered()
    {
        ui.Score.Show("Completed Sequence", score, currentLifes, time, GameConsts.Green);
    }

    private void OnGameOverEntered()
    {
        ui.Score.Show("Failed Sequence", score, currentLifes, time, GameConsts.Red);
    }

    private void OnKeyPressEvent(Key key)
    {
        ui.Keyboard.SetKeyPressed(key, true);
        sequence.HandlePressEvent(key);
        heldKeys.Add(key);
    }

    private void OnKeyReleaseEvent(Key key)
    {
        ui.Keyboard.SetKeyPressed(key, false);
        sequence.HandleReleaseEvent(key);
        heldKeys.Remove(key);
    }

    private float GetMultiplier()
    {
        return Mathf.Min(1f + ((float)streak / settings.requireStreakPerRank), settings.maxMultiplier);
    }

    private void AddKeyPressScore()
    {
        streak += 1;
        score += Mathf.RoundToInt(settings.baseButtonPressScore * GetMultiplier());

        ui.SetScore(score);
        ui.SetMultiplier(Mathf.RoundToInt(GetMultiplier()));
    }

    private void AddGroupCompleteScore()
    {
        score += Mathf.RoundToInt(settings.baseSequenceCompleteScore * GetMultiplier());
        ui.SetScore(score);
    }

    private void ClearStreak()
    {
        streak = 0;
        ui.SetMultiplier(Mathf.RoundToInt(GetMultiplier()));
    }
}