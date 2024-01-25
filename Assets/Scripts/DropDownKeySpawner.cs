using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropDownKeySpawner : MonoBehaviour {
    [SerializeField] private GameObject PresssedKeyDropDown;
    [SerializeField] private List<Sprite> keySprites;
    [SerializeField] private Transform keysTransform;
    private KeysUtil KeysUtil;

    private void Awake() {
        KeysUtil = GetComponent<KeysUtil>();
    }


    private void OnEnable() {
        GameManager.OnNewChar += OnNewChar;
    }
    
    private void OnDisable() {
        GameManager.OnNewChar -= OnNewChar;
    }
    
    private void OnNewChar(object sender, GameManager.OnNewCharArgs e) {
        MapCharToSprite(e.newChar); 
    }
    
    private void MapCharToSprite(char newChar) {
       
        Sprite sprite = KeysUtil.GetSpriteFromKey(newChar.ToString());
        if (sprite == null) {
            Debug.LogWarning($"No sprite found for {newChar}");
            return;
        }
        Debug.Log(sprite.name);
        
        //Transform keyTransform = GetKeyTransform(keyboardKeyName);
        //Debug.Log(keyTransform);
        //TODO: Get position of key
        
    }
    
    private Transform GetKeyTransform(string keyboardKeyName) {
        Transform keyTransform = keysTransform.Find(keyboardKeyName);
        if (keyTransform == null) {
            Debug.LogWarning($"Key {keyboardKeyName} not found");
            return null;
        }
        return keyTransform;
    }
}
