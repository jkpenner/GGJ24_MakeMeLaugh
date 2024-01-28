using System;
using UnityEngine;

public class KeyPromptManager : MonoBehaviour
{
    [SerializeField] KeyPromptContainer container;
    [SerializeField] int maxActivePrompts = 1;

    private KeySequenceManager sequence;

    private void Awake()
    {
        sequence = FindFirstObjectByType<KeySequenceManager>();
        if (sequence != null)
        {
            sequence.StepCompleted += OnStepCompleted;
            sequence.CurrentStepChanged += OnCurrentStepChanged;
            sequence.SequenceCompleted += OnSequenceCompleted;
            sequence.SequenceChanged += OnSequenceChanged;
        }
    }

    private void Update()
    {
        if (!sequence.IsComplete && container.PromptCount < maxActivePrompts)
        {
            SpawnPromptFromSequence();
        }
    }

    private void SpawnPromptFromSequence()
    {
        var stepIndex = sequence.CurrentIndex + container.PromptCount;
        var step = sequence.GetStep(stepIndex);

        if (step is not null)
        {
            container.SpawnKeyPrompt(stepIndex, step);
        }
    }

    private void OnStepCompleted(KeySequenceStepEventArgs args)
    {
        container.DespawnKeyPrompt(args.StepIndex);
    }

    private void OnCurrentStepChanged(KeySequenceStepEventArgs args)
    {
        for (int i = container.PromptCount; i < maxActivePrompts; i++)
        {
            int stepIndex = args.StepIndex + i;
            var step = sequence.GetStep(stepIndex);
            if (step is not null)
            {
                container.SpawnKeyPrompt(stepIndex, step);
            }
        }
    }

    private void OnSequenceCompleted(KeySequenceEventArgs args)
    {
        foreach(var (stepIndex, _) in container.Prompts)
        {
            container.DespawnKeyPrompt(stepIndex);
        }
    }

    private void OnSequenceChanged(KeySequenceEventArgs args)
    {
        foreach(var (stepIndex, prompt) in container.Prompts)
        {
            var step = sequence.GetStep(stepIndex);
            if (step is not null)
            {
                prompt.Setup(step, stepIndex);
            }
            else
            {
                container.DespawnKeyPrompt(stepIndex);
            }
        }
    }
}