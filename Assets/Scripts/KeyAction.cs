using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class KeyAction {
    public enum KeyColors {
        Red,
        Green,
        Blue,
        Yellow,
        White,
    }
    
    public enum KeyActionType {
        Hold,
        Release,
    }
    
    public KeyColors KeyColor;
    public KeyActionType keyActionType;
}
