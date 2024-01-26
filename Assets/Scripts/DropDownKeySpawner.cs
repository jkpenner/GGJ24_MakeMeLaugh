using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DropDownKeySpawner : MonoBehaviour {
    [SerializeField] private GameObject KeyDropDownPrefab;
    [SerializeField] private Transform keysTransform;
    [SerializeField] private Transform spawnPoint;
    
    private GameObject canvas;
    
    private void Awake() {
        canvas = GameObject.Find("Canvas");
    }
    
    private void OnEnable() {
        GameManager.OnNewKeyAction += OnNewKeyAction;
    }
    
    private void OnDisable() {
        GameManager.OnNewKeyAction -= OnNewKeyAction;
    }
    
    private void OnNewKeyAction(object sender, GameManager.OnNewKeyActionArgs e) {
        GameObject dropDownGameObject = Instantiate(KeyDropDownPrefab, spawnPoint.position, Quaternion.identity);
        dropDownGameObject.GetComponent<DropDownKey>().SetKey(e.KeyAction);
        dropDownGameObject.transform.SetParent(canvas.transform);
    }
    
}
