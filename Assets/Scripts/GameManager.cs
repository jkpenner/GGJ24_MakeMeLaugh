using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
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
    [SerializeField] KeyPromptContainer prompts;
    [SerializeField] KeyboardVisual visual;

    Keyboard current;
    Key[] keys;
    HashSet<Key> heldKeys = new HashSet<Key>();


    private GameState state = GameState.Initializing;
    private bool isGameOver = false;

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

        sequence.SetGameSettings(settings);
        sequence.GroupKeyIndexChanged += OnGroupKeyIndexChanged;
        sequence.GroupCompleted += OnGroupCompleted;
        sequence.GroupFailed += OnGroupFailed;
        sequence.KeyEventTriggered += OnKeyEvent;

        prompts.PromptsCleared += OnPromptsCleared;

        yield return null;

        sequence.StartNextGroup();
        SetGameState(GameState.GroupActive);
    }

    

    private void Update()
    {
        if (state == GameState.GroupComplete && heldKeys.Count == 0)
        {
            sequence.StartNextGroup();

            if (sequence.IsComplete)
            {
                SetGameState(GameState.GameVictory);
            }
            else
            {
                SetGameState(GameState.GroupActive);
            }
        }

        // Temp reload sceen after all keys are releasse when in a game over state
        if ((state == GameState.GameOver || state == GameState.GameVictory) && heldKeys.Count == 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
                OnGroupDespawningEntered();
                break;
            case GameState.GameOver:
                OnGameOverEntered();
                break;
            case GameState.GameVictory:
                OnGameVictoryEntered();
                break;
        }
    }

    private void OnKeyEvent(KeyEventArgs args)
    {
        if (args.EventType == KeyEventType.Success)
        {
            if (prompts.PromptIndexMap.TryGetValue(args.KeyIndex, out var prompt))
            {
                prompt.SetAsSuccess();
            }
        }
        else
        {
            if (prompts.PromptIndexMap.TryGetValue(args.KeyIndex, out var prompt))
            {
                prompt.SetAsFailed();
            }
        }
    }

    private void OnGroupKeyIndexChanged(KeySequenceGroupEventArgs args)
    {
        if (!args.Group.IsCompleted)
        {
            prompts.SpawnKeyPrompt(args.Group, args.KeyIndex);
        }
    }

    private void OnGroupCompleted(KeySequenceGroupEventArgs args)
    {
        // Group was successfully completed
        Debug.Log("Successfully completed a group");
        SetGameState(GameState.GroupDespawning);
    }

    private void OnGroupFailed(KeySequenceGroupEventArgs args)
    {
        Debug.Log("Failed to completed a group");
        isGameOver = true;
        SetGameState(GameState.GroupDespawning);
    }

    private void OnGroupDespawningEntered()
    {
        if (prompts.AnyActivePrompts())
        {
            prompts.DespawnAll();
        }
        else
        {
            OnPromptsCleared();
        }
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
        Debug.Log("Game Won!");
    }

    private void OnGameOverEntered()
    {
        Debug.Log("Game Over");
    }

    private void OnKeyPressEvent(Key key)
    {
        visual.SetKeyPressed(key, true);
        sequence.HandlePressEvent(key);
        heldKeys.Add(key);
    }

    private void OnKeyReleaseEvent(Key key)
    {
        visual.SetKeyPressed(key, false);
        sequence.HandleReleaseEvent(key);
        heldKeys.Remove(key);
    }
}