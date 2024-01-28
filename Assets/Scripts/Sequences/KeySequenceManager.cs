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

    public event Action<InvalidKeyEventArgs> GroupFailed;

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
            Debug.Log("Invalid Key Pressed");
            GroupFailed?.Invoke(new InvalidKeyEventArgs(key, false));
        }
    }

    public void HandleReleaseEvent(Key key)
    {
        if (sequence is null || sequence.IsComplete || sequence.IsCurrentGroupComplete())
        {
            return;
        }

        GroupFailed?.Invoke(new InvalidKeyEventArgs(key, true));
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