using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] TextAsset wordsAsset;
    [SerializeField] KeySequenceManager sequence;
    [SerializeField] KeyPromptController prompts;
    [SerializeField] KeyboardVisual visual;
    [SerializeField] KeyColorLayoutsScriptableObject keyColorLayouts;

    Keyboard current;
    Key[] keys;

    [SerializeField] int maxActivePrompts = 1;

    private void Awake()
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
    }

    private void Start()
    {
        SetKeyboardColors(keyColorLayouts.redKeys, Color.red);
        SetKeyboardColors(keyColorLayouts.greenKeys, Color.green);
        SetKeyboardColors(keyColorLayouts.blueKeys, Color.blue);
        SetKeyboardColors(keyColorLayouts.yellowKeys, Color.yellow);
    }

    private void Update()
    {
        if (!sequence.IsComplete && prompts.PromptCount < maxActivePrompts)
        {
            SpawnPromptFromSequence();
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

    private void OnKeyPressEvent(Key key)
    {
        var keyVisual = visual.GetKeyVisual(key);
        if (keyVisual != null)
        {
            keyVisual.SetPressed(true);
        }
        
        var step = sequence.GetCurrentStep();
        if (step is not null)
        {
            if (step.HoldType != HoldType.Release)
            {
                sequence.MarkStepCompleted(key, keyVisual != null ? keyVisual.PromptColor : KeyPromptColor.None);
            }
            else
            {
                // Fail the release step if we press a button.
                sequence.MarkStepFailed();
            }
        }
    }

    private void OnKeyReleaseEvent(Key key)
    {
        var keyVisual = visual.GetKeyVisual(key);
        if (keyVisual != null)
        {
            keyVisual.SetPressed(true);
        }
        
        var step = sequence.GetCurrentStep();
        if (step is not null)
        {
            if (step.HoldType == HoldType.Release)
            {
                sequence.MarkStepCompleted(key, keyVisual != null ? keyVisual.PromptColor : KeyPromptColor.None);
            }
            else
            {
                // Releasing a key early does not fail the current press step.
            }
        }
    }

    private void SpawnPromptFromSequence()
    {
        var stepIndex = sequence.CurrentIndex + prompts.PromptCount;
        var step = sequence.GetStep(stepIndex);

        if (step is not null)
        {
            prompts.SpawnKeyPrompt().Setup(step, stepIndex);
        }
    }

    void SetKeyboardColors(Key[] keys, Color color)
    {
        foreach (var key in keys)
        {
            visual.GetKeyVisual(key).SetColor(color);
        }
    }
}