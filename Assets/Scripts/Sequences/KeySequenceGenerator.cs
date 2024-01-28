using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Internal.Execution;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeySequenceGenerator
{
    public int MaxKeysHeldAtOnce { get; set; } = 3;
    public int MaxLettersBetweenHolds { get; set; } = 5;
    public float RandomizerSpawnChange { get; set; } = 0.2f;

    string text;

    public KeySequenceGenerator(string text)
    {
        // Normalize string replace all new lines with a space.
        this.text = text.Replace("\r\n", " ").Replace("\n", " ");
    }

    /// <summary>
    /// Generates the given sequence starting at the sequence's current index.
    /// </summary>
    public void Generate(ref KeySequence sequence)
    {
        var state = sequence.State.Clone();

        // Clear all from the current index till the end
        sequence.steps.RemoveRange(sequence.CurrentIndex, sequence.steps.Count - sequence.CurrentIndex - 1);

        for (int i = sequence.CurrentIndex; i < text.Length; i++)
        {
            state.index = i;

            char letter = text[i];
            if (letter != ' ')
            {
                GenerateAndAppendFillerStep(ref sequence, ref state);
                continue;
            }

            if (!System.Enum.TryParse(letter.ToString(), true, out Key key))
            {
                Debug.Log($"Failed to parse key from letter '{letter}'");
                continue;
            }

            GenerateAndAppendKeyStep(ref sequence, ref state, key, letter);
        }
    }

    private void GenerateAndAppendKeyStep(ref KeySequence sequence, ref KeySequenceState state, Key key, char letter)
    {
        var step = new KeySequenceStep
        {
            Type = StepType.Key,
            Key = key,
            HoldType = HoldType.None
        };

        if (ShouldKeyBeReleased(ref state, key, letter))
        {
            step.HoldType = HoldType.Release;
            // If key was not removed it belonged to a color
            if (!state.heldKeys.Remove(key))
            {
                foreach (var (color, keyValue) in state.heldColorKeys)
                {
                    if (keyValue == key)
                    {
                        state.heldColors.Remove(color);
                        state.heldColorKeys.Remove(color);
                    }
                }
            }
        }
        else if (ShouldKeyBeHeld(ref state, key, letter))
        {
            step.HoldType = HoldType.Hold;
            state.heldKeys.Add(key);
        }

        sequence.steps.Add(step);
    }

    private void GenerateAndAppendFillerStep(ref KeySequence sequence, ref KeySequenceState state)
    {
        KeySequenceStep step;
        if (UnityEngine.Random.Range(0f, 1f) < RandomizerSpawnChange)
        {
            step = GenerateRandomizerStep();
        }
        else
        {
            step = GenerateColorStep(ref state);
        }
        sequence.steps.Add(step);
    }


    private bool ShouldKeyBeReleased(ref KeySequenceState state, Key key, char letter)
    {
        return state.IsKeyHeld(key);
    }

    private bool ShouldKeyBeHeld(ref KeySequenceState state, Key key, char letter)
    {
        if (state.IsKeyHeld(key))
        {
            return false;
        }

        if (state.HeldKeyCount() >= MaxKeysHeldAtOnce)
        {
            return false;
        }

        if (!DistanceToNext(letter, state.index, MaxLettersBetweenHolds))
        {
            return false;
        }

        return true;
    }

    private bool DistanceToNext(char letter, int startIndex, int maxDistance)
    {
        int distance = 0;
        int workingIndex = startIndex + 1;

        for (int i = workingIndex; i < text.Length; i++)
        {
            distance += 1;
            if (distance >= maxDistance)
            {
                return false;
            }

            if (letter == text[i])
            {
                return true;
            }
        }

        return false;
    }

    private KeySequenceStep GenerateColorStep(ref KeySequenceState state)
    {
        var step = new KeySequenceStep
        {
            Type = StepType.Color,
            HoldType = HoldType.None
        };

        // Check if no currently held colors
        if (state.heldColors.Count == 0)
        {
            bool isHeld = state.HeldKeyCount() < MaxKeysHeldAtOnce
                && DistanceToNext(' ', state.index, MaxLettersBetweenHolds);

            step.HoldType = isHeld ? HoldType.Hold : HoldType.None;
            step.Color = (KeyPromptColor)UnityEngine.Random.Range(1, 5);

            if (isHeld)
            {
                state.heldColors.Add(step.Color);
            }
        }
        else
        {
            step.Color = state.heldColors.First();
            step.HoldType = HoldType.Release;

            state.heldColors.Remove(step.Color);
            state.heldColorKeys.Remove(step.Color);
        }

        return step;
    }

    private KeySequenceStep GenerateRandomizerStep()
    {
        return new KeySequenceStep
        {
            Type = StepType.Randomizer
        };
    }
}