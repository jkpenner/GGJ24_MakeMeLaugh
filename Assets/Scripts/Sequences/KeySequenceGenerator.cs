using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeySequenceGenerator
{
    public static KeySequence GenerateSequence(string text)
    {
        // Normalize string replace all new lines with a space.
        text = text.Replace("\r\n", "\n");

        var sequence = new KeySequence();

        foreach (var group in text.Split('\n'))
        {
            var keys = new List<Key>();
            foreach (var keyValue in group.Split(','))
            {
                if (System.Enum.TryParse(keyValue, true, out Key key))
                {
                    keys.Add(key);
                }
            }

            sequence.Groups.Add(new KeySequenceGroup(keys));
        }

        return sequence;
    }

    public static KeySequence GenerateRandomSequence(GameSettings settings)
    {
        var sequence = new KeySequence();
        var available = GetAvailableKeys();

        for (int j = 0; j < settings.generateGroupCount; j++)
        {
            var amount = UnityEngine.Random.Range(
                Mathf.Min(GameConsts.MinKeySeqence, settings.maxKeysHeldAtOnce - 1),
                settings.maxKeysHeldAtOnce
            );

            var keys = new List<Key>();

            for (int i = 0; i < amount; i++)
            {
                int index = Random.Range(0, available.Count);
                keys.Add(available[index]);
                available.RemoveAt(index);
            }

            available.AddRange(keys);
            sequence.Groups.Add(new KeySequenceGroup(keys));
        }
        return sequence;
    }

    public static List<Key> GetAvailableKeys()
    {
        return new List<Key>
        {
            Key.Space,
            Key.Enter,
            Key.Tab,
            Key.Backquote,
            Key.Quote,
            Key.Semicolon,
            Key.Comma,
            Key.Period,
            Key.Slash,
            Key.Backslash,
            Key.LeftBracket,
            Key.RightBracket,
            Key.Minus,
            Key.Equals,
            Key.A,
            Key.B,
            Key.C,
            Key.D,
            Key.E,
            Key.F,
            Key.G,
            Key.H,
            Key.I,
            Key.J,
            Key.K,
            Key.L,
            Key.M,
            Key.N,
            Key.O,
            Key.P,
            Key.Q,
            Key.R,
            Key.S,
            Key.T,
            Key.U,
            Key.V,
            Key.W,
            Key.X,
            Key.Y,
            Key.Z,
            Key.Digit1,
            Key.Digit2,
            Key.Digit3,
            Key.Digit4,
            Key.Digit5,
            Key.Digit6,
            Key.Digit7,
            Key.Digit8,
            Key.Digit9,
            Key.Digit0,
            Key.LeftShift,
            Key.RightShift,
            // Key.LeftAlt,
            // Key.RightAlt,
            Key.LeftCtrl,
            Key.RightCtrl,
            Key.Backspace,
            Key.CapsLock,
        };
    }
}