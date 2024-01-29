using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeySequenceManager : MonoBehaviour
{
    [SerializeField] int maxLettersBetweenHolds = 5;
    [SerializeField] int maxKeysHeldAtOnce = 3;

    private KeySequence sequence = null;
    private GameSettings settings;
    private SFXPlayer sfxPlayer;

    public bool IsComplete => sequence?.IsComplete ?? true;

    public event Action<KeySequenceEventArgs> SequenceCompleted;

    public event Action<KeySequenceGroupEventArgs> GroupKeyIndexChanged;
    public event Action<KeySequenceGroupEventArgs> GroupCompleted;
    public event Action<KeySequenceGroupEventArgs> GroupFailed;

    public event Action<KeyEventArgs> KeyEventTriggered;

    private void Awake()
    {
        sfxPlayer = FindAnyObjectByType<SFXPlayer>();
    }

    public void SetGameSettings(GameSettings settings)
    {
        if (settings is null)
        {
            Debug.LogError("Game Settings are null. Unable to setup KeySequence");
            return;
        }

        this.settings = settings;
        Regenerate();
    }

    public void Regenerate()
    {
        if (settings is null)
        {
            Debug.LogError("Game Settings are null. Unable to setup KeySequence");
            return;
        }

        if (settings.mode == GameMode.Fixed)
        {
            if (settings.sequenceSource is not null && !string.IsNullOrEmpty(settings.sequenceSource.text))
            {
                sequence = KeySequenceGenerator.GenerateSequence(settings.sequenceSource.text);
                sequence.State.Reset();
            }
            else
            {
                Debug.LogWarning($"KeySequence word source is not setup.");
            }
        }
        else
        {
            sequence = KeySequenceGenerator.GenerateRandomSequence(settings);
            sequence.State.Reset();
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
            // play sound
            // SFXPlayer.Instance.PlayRandomGoodSFX();
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
                // SFXPlayer.Instance.PlayRandomGroupCompletedSFX();
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
            // SFXPlayer.Instance.PlayRandomFailedSFX();
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

        var group = sequence.GetCurrentGroup();
        for (int i = 0; i < group.Keys.Count; i++)
        {
            if (group.Keys[i] == key)
            {
                KeyEventTriggered?.Invoke(new KeyEventArgs(
                    key,
                    i,
                    group,
                    KeyEventType.KeyReleased
                ));
            }
        }

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