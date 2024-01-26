using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {
    [SerializeField] private List<KeyObject.KeyColors> keyColors;
    [SerializeField] private List<KeyObject.KeyTypes> keyTypes;
    [SerializeField] private float timeToSendNextMove;
    private KeyObject keyObject;
    
    public static event EventHandler<OnNewKeyObjectArgs> OnNewKeyObject;
    
    public class OnNewKeyObjectArgs : EventArgs {
        public KeyObject.KeyTypes KeyType;
        public KeyObject.KeyColors KeyColor;
    }

   
    void Start() {
        StartCoroutine(SequenceToPerform());
    }

    IEnumerator SequenceToPerform() {
        for(int i = 0; i < keyColors.Count; i++) {
            yield return new WaitForSeconds(timeToSendNextMove);
            OnNewKeyObject?.Invoke(this, new OnNewKeyObjectArgs {
                KeyType = keyTypes[i],
                KeyColor = keyColors[i]
            });
        }
    }
}
