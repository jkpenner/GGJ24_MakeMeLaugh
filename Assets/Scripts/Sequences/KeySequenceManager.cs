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

    /// <summary>
    /// Sets the current step as completed, based on the given key and
    /// color combination it will mark the step as either completed 
    /// successfully or failed.
    /// </summary>
    public void MarkStepCompleted(Key key, KeyPromptColor color)
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
            if (IsValidKeyInput(key))
            {
                step.MarkSucceeded();
                if (step.HoldType == HoldType.Hold)
                {
                    sequence.MarkKeyAsHeld(key);
                }
            }
            else
            {
                step.MarkFailed();
                if (step.HoldType == HoldType.Hold)
                {
                    // Missed a key hold update sequence.
                    isDirty = true;
                }
            }

            if (step.HoldType == HoldType.Release)
            {
                sequence.MarkKeyAsReleased(key);
            }
        }
        else if (step.Type == StepType.Color)
        {
            if (IsValidColorInput(color))
            {
                step.Key = key;
                step.MarkSucceeded();

                if (step.HoldType == HoldType.Hold)
                {
                    sequence.MarkColorAsHeld(color, key);
                    // An unknown key was pressed update sequence
                    isDirty = true;
                }
            }
            else
            {
                step.MarkFailed();
                if (step.HoldType == HoldType.Hold)
                {
                    // Missed a color hold update sequence.
                    isDirty = true;
                }
            }

            if (step.HoldType == HoldType.Release)
            {
                sequence.MarkColorAsReleased(color, key);
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