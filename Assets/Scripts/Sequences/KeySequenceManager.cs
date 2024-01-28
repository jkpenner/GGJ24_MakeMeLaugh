using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeySequenceManager : MonoBehaviour
{
    [SerializeField] TextAsset wordSource;
    [SerializeField] int maxLettersBetweenHolds = 5;
    [SerializeField] int maxKeysHeldAtOnce = 3;

    private KeySequenceGenerator generator = null;
    private KeySequence sequence = null;
    private bool isDirty = true;

    public bool IsComplete => sequence?.IsComplete ?? false;
    public int StepCount => sequence?.StepCount ?? 0;
    public int CurrentIndex => sequence?.CurrentIndex ?? 0;

    public int MaxLettersBetweenHolds
    {
        get => maxLettersBetweenHolds;
        set
        {
            if (maxLettersBetweenHolds != value)
            {
                maxLettersBetweenHolds = value;
                isDirty = true;
            }
        }
    }

    public int MaxKeysHeldAtOnce
    {
        get => maxKeysHeldAtOnce;
        set
        {
            if (maxKeysHeldAtOnce != value)
            {
                maxKeysHeldAtOnce = value;
                isDirty = true;
            }
        }
    }

    public event Action<KeySequenceEventArgs> SequenceChanged;
    public event Action<KeySequenceEventArgs> SequenceCompleted;
    public event Action<KeySequenceStepEventArgs> StepCompleted;
    public event Action<KeySequenceStepEventArgs> CurrentStepChanged;

    private void Awake()
    {
        if (wordSource is not null)
        {
            SetWordSource(wordSource.text);
            Regenerate();
        }
    }

    public void SetWordSource(string source)
    {
        if (string.IsNullOrEmpty(source))
        {
            Debug.LogWarning($"KeySequence word source can not be set to an empty string.");
            return;
        }

        // Normalize string replace all new lines with a space.
        isDirty = true;
        source = source.Replace("\r\n", " ").Replace("\n", " ");
        generator = new KeySequenceGenerator(source);
    }

    public KeySequenceStep GetCurrentStep()
    {
        return sequence?.GetCurrentStep() ?? null;
    }

    public KeySequenceStep GetStep(int index)
    {
        return sequence?.GetStep(index) ?? null;
    }

    public void Regenerate()
    {
        if (!isDirty)
        {
            return;
        }

        isDirty = false;

        if (sequence is null)
        {
            sequence = new KeySequence();
        }

        Debug.Log("Regenerating sequence.");

        // Update generator values prior to update
        generator.MaxLettersBetweenHolds = maxLettersBetweenHolds;
        generator.MaxKeysHeldAtOnce = maxKeysHeldAtOnce;
        generator.RandomizerSpawnChange = 0f;
        generator.Generate(ref sequence);
    }

    public bool IsValidKeyInput(Key key)
    {
        if (sequence is null)
        {
            return false;
        }

        var step = sequence.GetCurrentStep();
        if (step is null)
        {
            return false;
        }

        if (step.Type == StepType.Key)
        {
            return step.Key == key;
        }
        else if (step.Type == StepType.Color)
        {
            // All keys are valid for colors, must check
            // on game side if key is valid color.
            return key != Key.None;
        }
        else
        {
            return false;
        }
    }

    public bool IsValidColorInput(KeyPromptColor color)
    {
        if (sequence is null)
        {
            return false;
        }

        var step = sequence.GetCurrentStep();
        if (step is null)
        {
            return false;
        }

        return step.Type == StepType.Color && step.Color == color;
    }

    public void HandlePressEvent(Key key, KeyPromptColor color)
    {
        if (sequence is null || sequence.IsComplete)
        {
            return;
        }

        var step = sequence.GetCurrentStep();
        if (step is null)
        {
            return;
        }

        if (step.Type == StepType.Key)
        {
            if (IsValidKeyInput(key) && step.HoldType != HoldType.Release)
            {
                Debug.Log($"Key ({step.Key}) hit successfully");
                step.MarkSucceeded();

                if (step.HoldType == HoldType.Hold)
                {
                    sequence.MarkKeyAsHeld(key);
                }
            }
            else
            {
                Debug.Log($"Hit invalided key (Hit {key} expected {step.Key})");

                step.MarkFailed();
                if (step.HoldType == HoldType.Hold)
                {
                    // Missed a key hold update sequence.
                    isDirty = true;
                }
            }
        }
        else if (step.Type == StepType.Color)
        {
            if (IsValidColorInput(color) && step.HoldType != HoldType.Release)
            {
                step.Key = key;
                step.MarkSucceeded();

                Debug.Log($"Color ({step.Color}) hit successfully");

                if (step.HoldType == HoldType.Hold)
                {
                    sequence.MarkColorAsHeld(color, key);
                    // An unknown key was pressed update sequence
                    isDirty = true;
                }
            }
            else
            {
                Debug.Log($"Hit invalided color (Hit {color} expected {step.Color})");
                step.MarkFailed();
                if (step.HoldType == HoldType.Hold)
                {
                    // Missed a color hold update sequence.
                    isDirty = true;
                }
            }
        }
        else
        {
            step.MarkSucceeded();
        }

        StepCompleted?.Invoke(new KeySequenceStepEventArgs(sequence.CurrentIndex, step));

        Regenerate(); // Only regens if dirty
        MoveToNextStep();
    }

    public void HandleReleaseEvent(Key key, KeyPromptColor color)
    {
        if (sequence is null || sequence.IsComplete)
        {
            return;
        }

        var step = sequence.GetCurrentStep();
        if (step is null)
        {
            return;
        }

        if (step.Type == StepType.Key)
        {
            if (IsValidKeyInput(key) && step.HoldType == HoldType.Release)
            {
                Debug.Log($"Successfully released {step.Key}");
                step.MarkSucceeded();

            }
            else if (sequence.State.IsKeyHeld(key))
            {
                isDirty = true;
                Debug.Log("Released a held key before it was time");
            }

            sequence.MarkKeyAsReleased(key);
        }
        else if (step.Type == StepType.Color)
        {
            if (IsValidColorInput(color))
            {
                step.MarkSucceeded();
                Debug.Log($"Successfully released {step.Color}");
            }
            else if (sequence.State.IsColorHeld(color))
            {
                isDirty = true;
                Debug.Log("Released a held color before it was time");
            }

            sequence.MarkColorAsReleased(color, key);
        }

        if (step.IsCompleted)
        {
            StepCompleted?.Invoke(new KeySequenceStepEventArgs(sequence.CurrentIndex, step));
        }

        Regenerate(); // Only regens if dirty
        if (step.IsCompleted)
        {
            MoveToNextStep();
        }
    }

    /// <summary>
    /// Marks the current step as completed in a failed state.
    /// </summary>
    public void MarkStepFailed()
    {
        if (sequence is null)
        {
            return;
        }

        var step = sequence.GetCurrentStep();
        if (step is null)
        {
            return;
        }

        step.MarkFailed();
        StepCompleted?.Invoke(new KeySequenceStepEventArgs(sequence.CurrentIndex, step));

        if (step.HoldType == HoldType.Hold)
        {
            isDirty = true;
            Regenerate();
        }

        MoveToNextStep();
    }

    public void MoveToNextStep()
    {
        if (sequence is null)
        {
            return;
        }

        sequence.MoveToNextStep();
        if (sequence.IsComplete)
        {
            SequenceCompleted?.Invoke(new KeySequenceEventArgs(sequence));
            return;
        }

        var step = sequence.GetCurrentStep();
        CurrentStepChanged?.Invoke(new KeySequenceStepEventArgs(sequence.CurrentIndex, step));
    }
}