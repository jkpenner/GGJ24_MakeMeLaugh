using System.Collections.Generic;
using UnityEngine.InputSystem;

public class KeySequenceState
{
    public int index;
    public HashSet<Key> heldKeys;
    public HashSet<KeyPromptColor> heldColors;
    public Dictionary<KeyPromptColor, Key> heldColorKeys;

    public KeySequenceState()
    {
        index = 0;
        heldKeys = new HashSet<Key>();
        heldColors = new HashSet<KeyPromptColor>();
        heldColorKeys = new Dictionary<KeyPromptColor, Key>();
    }

    public KeySequenceState(KeySequenceState other)
    {
        index = other.index;
        heldKeys = new HashSet<Key>(other.heldKeys);
        heldColors = new HashSet<KeyPromptColor>(other.heldColors);
        heldColorKeys = new Dictionary<KeyPromptColor, Key>(other.heldColorKeys);
    }

    public KeySequenceState Clone()
    {
        return new KeySequenceState(this);
    }

    public void Reset()
    {
        index = 0;
        heldKeys.Clear();
        heldColors.Clear();
        heldColorKeys.Clear();
    }

    /// <summary>
    /// Gets the current number of keys being held down.
    /// </summary>
    public int HeldKeyCount()
    {
        return heldKeys.Count + heldColors.Count;
    }

    /// <summary>
    /// Check if a target key is currently held down.
    /// </summary>
    public bool IsKeyHeld(Key key)
    {
        return heldKeys.Contains(key) || heldColorKeys.ContainsValue(key);
    }

    /// <summary>
    /// Checks if the target color is currently held own.
    /// </summary>
    public bool IsColorHeld(KeyPromptColor color)
    {
        return heldColors.Contains(color);
    }
}