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

    public void SetKey(KeyObject.KeyTypes keyType, KeyObject.KeyColors keyColor) {
        if(keyType == KeyObject.KeyTypes.Hold) {
            title.text = "Hold";
        } else {
            title.text = "Release";
        }
        switch (keyColor) {
            case KeyObject.KeyColors.Red:
                image.color = Color.red;
                break;
            case KeyObject.KeyColors.Green:
                image.color = Color.green;
                break;
            case KeyObject.KeyColors.Blue:
                image.color = Color.blue;
                break;
            case KeyObject.KeyColors.Yellow:
                image.color = Color.yellow;
                break;
            case KeyObject.KeyColors.White:
                image.color = Color.white;
                break;
            default:
                Debug.LogWarning($"No color found for {keyType}, {keyColor}");
                break;
        }
    }
}
