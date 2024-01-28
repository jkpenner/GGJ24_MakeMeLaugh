using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeySequenceManager : MonoBehaviour
{
    private KeySequence sequence = null;
    private GameSettings settings;

    public bool IsComplete => sequence?.IsComplete ?? true;

    public event Action<KeySequenceEventArgs> SequenceCompleted;

    public event Action<KeySequenceGroupEventArgs> GroupKeyIndexChanged;
    public event Action<KeySequenceGroupEventArgs> GroupCompleted;
    public event Action<KeySequenceGroupEventArgs> GroupFailed;

    public event Action<KeyEventArgs> KeyEventTriggered;

    public void SetGameSettings(GameSettings settings)
    {
        if (settings is null)
        {
            Debug.LogError("Game Settings are null. Unable to setup KeySequence");
            return;
        }

        this.settings = settings;

        if (settings.sequenceSource is not null && !string.IsNullOrEmpty(settings.sequenceSource.text))
        {
            sequence = KeySequenceGenerator.GenerateSequence(settings.sequenceSource.text);
        }
        else
        {
            Debug.LogWarning($"KeySequence word source is not setup.");
        }
    }

    public bool IsValidKeyInput(Key key)
    {
        if (sequence is null)
        {
            return false;
        }

        return sequence.IsActiveKey(key);
    }

    public void HandlePressEvent(Key key)
    {
        if (sequence is null || sequence.IsComplete || sequence.IsCurrentGroupComplete())
        {
            return;
        }

        if (sequence.IsActiveKey(key))
        {
            KeyEventTriggered?.Invoke(new KeyEventArgs(
                key,
                sequence.State.keyIndex,
                sequence.GetCurrentGroup(),
                KeyEventType.Success
            ));

            sequence.MoveToNextKey();

            // If at the max key limit just move to end of sequence to mark complete
            if (sequence.KeyIndex >= settings.maxKeysHeldAtOnce)
            {
                sequence.State.keyIndex = sequence.GetCurrentGroup().Keys.Count;
            }

            if (sequence.IsCurrentGroupComplete())
            {
                GroupCompleted?.Invoke(new KeySequenceGroupEventArgs(
                    sequence.GetCurrentGroup(),
                    sequence.State.keyIndex
                ));
            }
            else
            {
                GroupKeyIndexChanged?.Invoke(new KeySequenceGroupEventArgs(
                    sequence.GetCurrentGroup(),
                    sequence.State.keyIndex
                ));
            }
        }
        else
        {
            KeyEventTriggered?.Invoke(new KeyEventArgs(
                key,
                sequence.State.keyIndex,
                sequence.GetCurrentGroup(),
                KeyEventType.WrongKeyPressed
            ));

            MarkGroupFailed(key, false);
        }
    }

    public void HandleReleaseEvent(Key key)
    {
        if (sequence is null || sequence.IsComplete || sequence.IsCurrentGroupComplete())
        {
            return;
        }

        KeyEventTriggered?.Invoke(new KeyEventArgs(
            key,
            sequence.State.keyIndex,
            sequence.GetCurrentGroup(),
            KeyEventType.KeyReleased
        ));

        MarkGroupFailed(key, true);
    }

    public void MarkGroupFailed(Key key, bool wasReleased)
    {
        if (sequence is null || sequence.IsComplete || sequence.IsCurrentGroupComplete())
        {
            return;
        }

        var group = sequence.GetCurrentGroup();
        if (group.State == CompletionState.Pending)
        {
            group.MarkFailed();
            GroupFailed?.Invoke(new KeySequenceGroupEventArgs(sequence.GetCurrentGroup(), sequence.KeyIndex));
        }
    }

    public void StartNextGroup()
    {
        if (sequence is null || sequence.IsComplete)
        {
            return;
        }

        sequence.MoveToNextGroup();

        if (!sequence.IsComplete)
        {
            GroupKeyIndexChanged?.Invoke(new KeySequenceGroupEventArgs(
                sequence.GetCurrentGroup(),
                sequence.State.keyIndex
            ));
        }
    }
}