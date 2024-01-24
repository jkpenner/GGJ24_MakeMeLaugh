using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyboardManager : MonoBehaviour {
    
    [SerializeField] private GameObject highlightedKeyPrefab;
    [SerializeField] private Transform keyboardKeysTransform;
    
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
                        AddHighlightToKey(key);
                    }
                }

                if (current[key].wasReleasedThisFrame)
                {
                    if (pressed.Remove(key))
                    {
                        maxPressedKeys = Mathf.Max(maxPressedKeys, pressed.Count);
                        RemoveHighlightFromKey(key);
                    }
                }
                
            }
            catch (Exception e)
            {
                Debug.LogError($"Key {key} received {e}");
            }
        }
    }

    void AddHighlightToKey(Key key) {
        string keyName = key.ToString();
        Transform keyTransform = keyboardKeysTransform.Find(keyName);
        if (keyTransform == null)
        {
            Debug.LogWarning($"Key {keyName} not found");
            return;
        }
        GameObject highlight = Instantiate(highlightedKeyPrefab, keyTransform);
        highlight.transform.SetParent(keyTransform);
    }
    
    void RemoveHighlightFromKey(Key key) {
        string keyName = key.ToString();
        Transform keyTransform = keyboardKeysTransform.Find(keyName);
        if (keyTransform == null) {
            Debug.LogWarning($"Key {keyName} not found");
            return;
        }
        // foreach (Transform child in keyTransform) {
            Destroy(keyTransform.GetChild(0).gameObject);
        // }
    }
}
