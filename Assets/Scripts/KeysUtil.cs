using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KeysUtil : MonoBehaviour {
    [SerializeField] private List<Sprite> sprites;

    // This property will combine keys and sprites into a dictionaryddd
    private Dictionary<string, Sprite> KeySpriteDictionary {
        get {
            return sprites.ToDictionary(sprite => sprite.name);
        }
    }

    public Sprite GetSpriteFromKey(string key) {
        switch (key) {
            case "1":
                key = "Digit1";
                break;
            case "2":
                key = "Digit2";
                break;
            case "3":
                key = "Digit3";
                break;
            case "4":
                key = "Digit4";
                break;
            case "5":
                key = "Digit5";
                break;
            case "6":
                key = "Digit6";
                break;
            case "7":
                key = "Digit7";
                break;
            case "8":
                key = "Digit8";
                break;
            case "9":
                key = "Digit9";
                break;
            case "0":
                key = "Digit0";
                break;
            case " ":
                key = "Space";
                break;
            default:
                break;

        }
        return KeySpriteDictionary[key];
    }
}
