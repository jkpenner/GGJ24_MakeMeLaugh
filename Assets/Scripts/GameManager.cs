using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] TextAsset wordsAsset;
    [SerializeField] KeyPromptController prompts;
    [SerializeField] KeyboardVisual visual;
    [SerializeField] KeyColorLayoutsScriptableObject keyColorLayouts;

    private KeySequenceGenerator generator;
    public KeySequence sequence;

    Keyboard current;
    Key[] keys;

    HashSet<Key> heldKeys = new HashSet<Key>();
    Key heldColorKey = Key.None;
    KeyPromptColor heldColor = KeyPromptColor.None;

    [SerializeField] int maxActivePrompts = 1;
    [SerializeField] int maxKeysHeldAtOnce = 3;
    [SerializeField] int maxLettersBetweenHolds = 6;

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

        if (wordsAsset is not null)
        {
            var words = wordsAsset.text.Split("\n");
            generator = new KeySequenceGenerator(words);
            generator.MaxKeysHeldAtOnce = maxKeysHeldAtOnce;
            generator.MaxLettersBetweenHolds = maxLettersBetweenHolds;
            sequence = generator.Generate();
        }
        else
        {
            Debug.LogError("No Word Asset assigned.");
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
        if (visual.TryGetKeyVisual(key, out KeyVisual keyVisual))
        {
            keyVisual.SetPressed(true);
        }

        var prompt = prompts.CurrentPrompt;
        if (prompt is null)
        {
            return;
        }

        var step = sequence.GetStep(prompt.StepIndex);
        if (step is null)
        {
            return;
        }

        if (step.Type == StepType.Key)
        {
            ProcessKeyStepPressEvent(key, step, prompt);
        }
        else if (step.Type == StepType.Color)
        {
            ProcessColorStepPressEvent(key, step, prompt);
        }
    }

    private void ProcessKeyStepPressEvent(Key key, KeySequenceStep step, KeyPrompt prompt)
    {
        if (step.Type == StepType.Key && step.Key == key)
        {
            if (step.HoldType == HoldType.Hold)
            {
                heldKeys.Add(key);
            }

            // Todo: Play successful hit animation.
        }
        else
        {
            Debug.Log("Hit the wrong key for the prompt");
            // Todo: Play failed hit animation.
        }

        prompts.DespawnKeyPrompt();
        sequence.MoveToNextStep();
    }

    private void ProcessColorStepPressEvent(Key key, KeySequenceStep step, KeyPrompt prompt)
    {
        var keyVisual = visual.GetKeyVisual(key);
        if (keyVisual is null)
        {
            Debug.LogError($"Failed to find key visual for {key}");
            prompts.DespawnKeyPrompt();
            sequence.MoveToNextStep();
            return;
        }

        if (step.Type == StepType.Color && step.Color == keyVisual.PromptColor)
        {
            if (step.HoldType == HoldType.Hold)
            {
                heldColor = step.Color;
                heldColorKey = key;
            }
            // Todo: Play successful hit animation.
        }
        else
        {
            Debug.Log("Hit the wrong color for the prompt");
            // Todo: Play failed hit animation.

        }

        prompts.DespawnKeyPrompt();
        sequence.MoveToNextStep();
    }

    private void OnKeyReleaseEvent(Key key)
    {
        if (visual.TryGetKeyVisual(key, out KeyVisual keyVisual))
        {
            keyVisual.SetPressed(false);
        }

        if (!heldKeys.Contains(key) && heldColorKey != key)
        {
            return;
        }

        // if not removed from held its it is the held color key.
        if (!heldKeys.Remove(key))
        {
            heldColor = KeyPromptColor.None;
            heldColorKey = Key.None;
        }

        var prompt = prompts.CurrentPrompt;
        if (prompt is null)
        {
            return;
        }

        var step = sequence.GetStep(prompt.StepIndex);
        if (step is null || step.HoldType != HoldType.Release)
        {
            return;
        }

        if (step.Type == StepType.Key)
        {
            ProcessKeyStepReleaseEvent(key, step, prompt);
        }
        else if (step.Type == StepType.Color)
        {
            ProcessColorStepReleaseEvent(key, step, prompt);
        }
    }

    private void ProcessKeyStepReleaseEvent(Key key, KeySequenceStep step, KeyPrompt prompt)
    {
        if (step.Key == key && step.HoldType == HoldType.Release)
        {
            // Todo: Play successful hit animation.

            prompts.DespawnKeyPrompt();
            sequence.MoveToNextStep();
        }
        else
        {
            Debug.Log("Release key at the wrong time");
            // Todo: Play failed hit animation on the correct prompt.
        }
    }

    private void ProcessColorStepReleaseEvent(Key key, KeySequenceStep step, KeyPrompt prompt)
    {
        if (step.Color == heldColor && step.HoldType == HoldType.Release)
        {
            // Todo: Play successful hit animation.

            prompts.DespawnKeyPrompt();
            sequence.MoveToNextStep();
        }
        else
        {
            Debug.Log("Release color key at the wrong time");
            // Todo: Play failed hit animation on the correct prompt.
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