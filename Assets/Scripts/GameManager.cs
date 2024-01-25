using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {
    [SerializeField] private string[] sentences;
    [SerializeField] private float timeToSendNextCharacter;
    
    private string sentenceToType;
    
    public static event EventHandler<OnNewCharArgs> OnNewChar;
    
    public class OnNewCharArgs : EventArgs {
        public char newChar;
    }

    private void Awake() {
        sentenceToType = sentences[Random.Range(0, sentences.Length - 1)];
        Debug.Log(sentenceToType);
    }

    // Start is called before the first frame update
    void Start() {
        StartCoroutine(TypeSentence(sentenceToType));
    }
    
    IEnumerator TypeSentence(string sentence) {
        foreach (char letter in sentence.ToCharArray()) {
            yield return new WaitForSeconds(timeToSendNextCharacter);
            OnNewChar?.Invoke(this, new OnNewCharArgs {
                newChar = letter
            });
            
        }
    }
    
}
