using System.Collections.Generic;
using UnityEngine.InputSystem;

public class KeySequenceGenerator
{
    public static KeySequence GenerateSequence(string text)
    {
        // Normalize string replace all new lines with a space.
        text = text.Replace("\r\n", "\n");

        var sequence = new KeySequence();

        foreach(var group in text.Split('\n'))
        {           
            var keys = new List<Key>();
            foreach(var keyValue in group.Split(','))
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
}