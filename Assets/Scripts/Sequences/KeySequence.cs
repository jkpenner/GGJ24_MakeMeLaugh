using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class KeySequence
{
    private List<KeySequenceGroup> groups;
    private KeySequenceState state;

    public List<KeySequenceGroup> Groups => groups;
    public KeySequenceState State => state;

    public int GroupIndex => state.groupIndex;
    public int KeyIndex => state.keyIndex;    

    public int SequenceGroupCount => groups.Count;
    public bool IsComplete => GroupIndex == SequenceGroupCount;

    public KeySequence()
    {
        this.state = new KeySequenceState();
        this.groups = new List<KeySequenceGroup>();
    }

    public bool IsActiveKey(Key key)
    {
        if (IsComplete)
        {
            return false;
        }

        if (GroupIndex < 0 || GroupIndex >= groups.Count)
        {
            return false;
        }

        var group = groups[GroupIndex];
        if (KeyIndex < 0 || KeyIndex >= group.Keys.Count)
        {
            return false;
        }

        return group.Keys[KeyIndex] == key;
    }

    public void Reset()
    {
        state.Reset();
    }

    public void MoveToNextKey()
    {
        if (IsComplete || GroupIndex >= groups.Count)
        {
            return;
        }

        state.keyIndex = Mathf.Min(state.keyIndex + 1, groups[GroupIndex].Keys.Count);
    }

    public void MoveToNextGroup()
    {
        if (IsComplete)
        {
            return;
        }

        state.groupIndex = Mathf.Min(state.groupIndex + 1, SequenceGroupCount);
        state.keyIndex = 0;
    }

    public bool IsCurrentGroupComplete()
    {
        if (IsComplete || GroupIndex < 0 || GroupIndex >= groups.Count)
        {
            return false;
        }

        return groups[GroupIndex].Keys.Count <= state.keyIndex;
    }

    public KeySequenceGroup GetCurrentGroup()
    {
        if (IsComplete || GroupIndex < 0 || GroupIndex >= groups.Count)
        {
            return null;
        }

        return groups[GroupIndex];
    }
}