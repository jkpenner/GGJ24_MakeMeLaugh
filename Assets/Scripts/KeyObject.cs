using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyObject {
    public enum KeyColors {
        Red,
        Green,
        Blue,
        Yellow,
        White,
    }
    
    public enum KeyTypes {
        Hold,
        Release,
    }
    
    public KeyColors KeyColor;
    public KeyTypes KeyType;
}
