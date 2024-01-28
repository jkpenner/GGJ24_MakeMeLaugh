using UnityEngine.InputSystem;

public class KeySequenceEventArgs
{
    public readonly KeySequence Sequence;

    public KeySequenceEventArgs(KeySequence sequence)
    {
        Sequence = sequence;
    }
}

public class KeySequenceGroupEventArgs
{
    public readonly int KeyIndex;
    public readonly KeySequenceGroup Group;

    public KeySequenceGroupEventArgs(KeySequenceGroup group, int keyIndex)
    {
        KeyIndex = keyIndex;
        Group = group;
    }
}

public class InvalidKeyEventArgs
{
    public readonly Key Key;
    public bool WasReleased;

    public InvalidKeyEventArgs(Key key, bool wasReleased)
    {
        Key = key;
        WasReleased = wasReleased;
    }
}