using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {
    [SerializeField] private List<KeyAction> keyActionsList;
    [SerializeField] private float timeToSendNextMove;
    private KeyAction keyAction;
    
    public static event EventHandler<OnNewKeyActionArgs> OnNewKeyAction;
    
    public class OnNewKeyActionArgs : EventArgs {
        public KeyAction KeyAction;
    }

    private void Start() {
        StartCoroutine(SequenceToPerform());
    }

    private IEnumerator SequenceToPerform() {
        for(int i = 0; i < keyActionsList.Count; i++) {
            yield return new WaitForSeconds(timeToSendNextMove);
            OnNewKeyAction?.Invoke(this, new OnNewKeyActionArgs {
                KeyAction = keyActionsList[i]
            });
        }
    }
}
