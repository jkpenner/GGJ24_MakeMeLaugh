using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class KeySequence
{
    public List<KeySequenceStep> steps;

    public int CurrentIndex;
    public int StepCount => steps.Count;
    public bool IsComplete => CurrentIndex == StepCount;

    public KeySequence(List<KeySequenceStep> steps)
    {
        this.steps = steps;
    }

    public void MoveToNextStep()
    {
        CurrentIndex += 1;
        CurrentIndex = Mathf.Min(CurrentIndex, StepCount);
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
}