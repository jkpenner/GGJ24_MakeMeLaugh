using System.Collections.Generic;
using UnityEngine.InputSystem;

public class KeySequenceState
{
    public int groupIndex;
    public int keyIndex;
    public HashSet<Key> heldKeys;

    public KeySequenceState()
    {
        groupIndex = -1;
        heldKeys = new HashSet<Key>();
    }

    public KeySequenceState(KeySequenceState other)
    {
        groupIndex = other.groupIndex;
        heldKeys = new HashSet<Key>(other.heldKeys);
    }

    public KeySequenceState Clone()
    {
        return new KeySequenceState(this);
    }

    public void Reset()
    {
        groupIndex = -1;
        heldKeys.Clear();
    }

    /// <summary>
    /// Gets the current number of keys being held down.
    /// </summary>
    public int HeldKeyCount()
    {
        return heldKeys.Count;
    }

    /// <summary>
    /// Check if a target key is currently held down.
    /// </summary>
    public bool IsKeyHeld(Key key)
    {
        return heldKeys.Contains(key);
    }
}