using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DropDownKey : MonoBehaviour {
    [SerializeField] private float speed = 100f;
    private TextMeshProUGUI title;
    private Image image;

    private void Awake() {
        image = GetComponent<Image>();
        title = GetComponentInChildren<TextMeshProUGUI>();
    }
    
    void Update() {
        transform.position += Vector3.down * speed * Time.deltaTime;
    }

    public void SetKey(KeyAction keyAction) {
        if(keyAction.keyActionType == KeyAction.KeyActionType.Hold) {
            title.text = "Hold";
        } else {
            title.text = "Release";
        }
        switch (keyAction.KeyColor) {
            case KeyAction.KeyColors.Red:
                image.color = Color.red;
                break;
            case KeyAction.KeyColors.Green:
                image.color = Color.green;
                break;
            case KeyAction.KeyColors.Blue:
                image.color = Color.blue;
                break;
            case KeyAction.KeyColors.Yellow:
                image.color = Color.yellow;
                break;
            case KeyAction.KeyColors.White:
                image.color = Color.white;
                break;
            default:
                Debug.LogWarning($"No color found for {keyAction.keyActionType}, {keyAction.KeyColor}");
                break;
        }
    }
}
