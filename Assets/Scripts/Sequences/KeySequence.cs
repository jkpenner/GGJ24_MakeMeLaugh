using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class KeySequence
{
    public List<KeySequenceStep> steps;
    private KeySequenceState state;

    public int CurrentIndex => state.index;
    public int StepCount => steps.Count;
    public bool IsComplete => CurrentIndex == StepCount;
    public KeySequenceState State => state;

    public KeySequence()
    {
        this.state = new KeySequenceState();
        this.steps = new List<KeySequenceStep>();
    }

    /// <summary>
    /// Resets the sequence back to its starting position.
    /// </summary>
    public void Reset()
    {
        state.Reset();
    }

    public void MoveToNextStep()
    {
        state.index += 1;
        state.index = Mathf.Min(CurrentIndex, StepCount);
    }

    public KeySequenceStep GetCurrentStep()
    {
        return GetStep(CurrentIndex);
    }

    public KeySequenceStep GetStep(int index)
    {
        if (index >= 0 && index < StepCount)
        {
            return steps[index];
        }
        return null;
    }

    public void MarkKeyAsHeld(Key key)
    {
        state.heldKeys.Add(key);
    }

    public void MarkKeyAsReleased(Key key)
    {
        state.heldKeys.Remove(key);
    }

    public void MarkColorAsHeld(KeyPromptColor color, Key key)
    {
        state.heldColors.Add(color);
        state.heldColorKeys[color] = key;
    }

    public void MarkColorAsReleased(KeyPromptColor color, Key key)
    {
        state.heldColors.Remove(color);
        state.heldColorKeys.Remove(color);
    }
}