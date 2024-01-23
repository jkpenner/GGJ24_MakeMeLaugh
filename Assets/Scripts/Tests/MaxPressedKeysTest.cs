using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MaxPressedKeysTest : MonoBehaviour
{
    public TMPro.TMP_Text text;

    Keyboard current;
    Key[] keys;
    HashSet<Key> pressed = new HashSet<Key>();
    private int maxPressedKeys;

    private void Awake()
    {
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
        foreach (Key key in keys)
        {
            try
            {
                if (current[key].wasPressedThisFrame)
                {
                    if (pressed.Add(key))
                    {
                        maxPressedKeys = Mathf.Max(maxPressedKeys, pressed.Count);
                    }
                }

                if (current[key].wasReleasedThisFrame)
                {
                    if (pressed.Remove(key))
                    {
                        maxPressedKeys = Mathf.Max(maxPressedKeys, pressed.Count);
                    }
                }

                text.text = maxPressedKeys.ToString();
            }
            catch (Exception e)
            {
                Debug.LogError($"Key {key} received {e}");
            }
        }
    }
}
