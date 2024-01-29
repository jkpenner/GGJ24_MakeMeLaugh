using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyboardVisual : MonoBehaviour
{
    private Dictionary<Key, KeyVisual> visuals;

    Keyboard current;
    Key[] keys;

    private void Awake()
    {
        ExtractKeyVisuals();
    }

    private void ExtractKeyVisuals()
    {
        visuals ??= new Dictionary<Key, KeyVisual>();
        visuals.Clear();

        // Loop through all rows of the keyboard.
        for (int i = 0; i < transform.childCount; i++)
        {
            // Loop through all keys in the row.
            var row = transform.GetChild(i);
            for (int j = 0; j < row.childCount; j++)
            {
                var key = row.GetChild(j);
                if (!key.TryGetComponent(out KeyVisual visual))
                {
                    continue;
                }

                // Key naming convention starts with 'Key_'.
                var name = key.name.Replace("Key_", "");
                if (!Enum.TryParse<Key>(name, true, out Key keyValue))
                {
                    Debug.LogWarning($"Failed to parse key {name}");
                    continue;
                }

                try
                {
                    //                    Debug.Log($"Adding {keyValue} from {name}");
                    visuals.Add(keyValue, visual);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Issue with key {name}, {e}");
                }
            }
        }
    }

    /// <summary>
    /// Check if a KeyVisual exists for the target Key value.
    /// </summary>
    public bool HasKeyVisual(Key key)
    {
        return visuals?.ContainsKey(key) ?? false;
    }

    /// <summary>
    /// Gets the KeyVisual for the desired Key value.
    /// </summary>
    public KeyVisual GetKeyVisual(Key key)
    {
        if (TryGetKeyVisual(key, out KeyVisual visual))
        {
            return visual;
        }
        return null;
    }

    /// <summary>
    /// Attempt to get the KeyVisual for the desired Key value.
    /// </summary>
    public bool TryGetKeyVisual(Key key, out KeyVisual visual)
    {
        if (visuals is null)
        {
            visual = null;
            return false;
        }

        return visuals.TryGetValue(key, out visual);
    }

    public void SetKeyPressed(Key key, bool isPressed)
    {
        var keyVisual = GetKeyVisual(key);
        if (keyVisual != null)
        {
            keyVisual.SetPressed(isPressed);
        }
    }

    public void ResetAllKeys()
    {
        foreach(var key in visuals.Values)
        {
            key.SetActiveKey(false);
            key.SetErrorKey(false);
        }
    }
}
