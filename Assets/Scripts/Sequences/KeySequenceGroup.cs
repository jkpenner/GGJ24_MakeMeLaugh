using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum CompletionState
{
    Pending,
    Succeeded,
    Failed
}

[System.Serializable]
public class KeySequenceGroup
{
    [SerializeField] List<Key> keys;

    public List<Key> Keys => keys;
    public CompletionState State { get; private set; }
    public bool IsCompleted => State != CompletionState.Pending;

    public KeySequenceGroup(List<Key> keys)
    {
        this.keys = keys;
        this.State = CompletionState.Pending;
    }

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