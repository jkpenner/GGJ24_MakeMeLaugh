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
        GameManager.OnNewKeyObject += OnNewKeyObject;
    }
    
    private void OnDisable() {
        GameManager.OnNewKeyObject -= OnNewKeyObject;
    }
    
    private void OnNewKeyObject(object sender, GameManager.OnNewKeyObjectArgs e) {
        GameObject dropDownGameObject = Instantiate(KeyDropDownPrefab, spawnPoint.position, Quaternion.identity);
        dropDownGameObject.GetComponent<DropDownKey>().SetKey(e.KeyType, e.KeyColor);
        dropDownGameObject.transform.SetParent(canvas.transform);
    }
    
}
