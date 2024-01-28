using UnityEngine;
using UnityEngine.InputSystem;

public enum StepType
{
    Key,
    Color,
    Randomizer,
}

public enum HoldType
{
    None,
    Hold,
    Release,
}

public enum CompletionState
{
    Pending,
    Succeeded,
    Failed,
}

[System.Serializable]
public class KeySequenceStep
{
    [SerializeField] StepType type = StepType.Key;
    [SerializeField] Key key = Key.None;
    [SerializeField] KeyPromptColor color = KeyPromptColor.None;
    [SerializeField] HoldType holdType = HoldType.None;

    public StepType Type { get => type; set => type = value; }
    public Key Key { get => key; set => key = value; }
    public KeyPromptColor Color { get => color; set => color = value; }
    public HoldType HoldType { get => holdType; set => holdType = value; }

    public CompletionState State { get; private set; }
    public bool IsCompleted => State != CompletionState.Pending;

    public void MarkSucceeded()
    {
        State = CompletionState.Succeeded;
    }

    public void MarkFailed()
    {
        State = CompletionState.Failed;
    }

    public void Reset()
    {
        State = CompletionState.Pending;
    }
}