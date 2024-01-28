using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyboardVisual : MonoBehaviour
{
    [SerializeField] bool debugInputs = false;

    private Dictionary<Key, KeyVisual> visuals;

    Keyboard current;
    Key[] keys;

    private void Awake()
    {
        ExtractKeyVisuals();

        current = Keyboard.current;
        if (current is null)
        {
            Debug.LogWarning("No keyboard found");
        }

        keys = new Key[(int)Key.IMESelected - 1];
        for (int i = 1; i < (int)Key.IMESelected; i++)
        {
            keys[i - 1] = (Key)i;
        }
    }

    private void Update()
    {
        if (!debugInputs)
        {
            return;
        }

        foreach (Key key in keys)
        {
            try
            {
                if (current[key].wasPressedThisFrame)
                {
                    if(TryGetKeyVisual(key, out KeyVisual visual))
                    {
                        visual.SetPressed(true);
                    }
                }

                if (current[key].wasReleasedThisFrame)
                {
                    if(TryGetKeyVisual(key, out KeyVisual visual))
                    {
                        visual.SetPressed(false);
                    }
                }

            }
            catch (Exception e)
            {
                Debug.LogError($"Key {key} received {e}");
            }
        }
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

                try {
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

    public void PopulateKeyPromptColors()
    {
        int value = 0;

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


                visual.SetPromptColor((KeyPromptColor)(value % 4 + 1));
                value += 1;
            }
        }
    }
}
