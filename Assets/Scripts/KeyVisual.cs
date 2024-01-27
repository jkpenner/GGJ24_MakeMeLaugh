using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class KeyVisual : MonoBehaviour
{
    [SerializeField] TMP_Text label;
    [SerializeField] CanvasGroup activeGroup;

    private float t = 0f;
    private float transitionTime = 0.2f;
    private Image image;

    public bool IsPressed { get; private set; }
    public KeyPromptColor PromptColor { get; private set; }

    private void Awake() {
        image = GetComponent<Image>();
        activeGroup.alpha = 0f;
    }

    public void SetKey(Key key)
    {
        label.text = key switch {
            Key.Backslash => "\\",
            Key.Backquote => "`",
            Key.CapsLock => "Caps Lock",
            Key.RightShift => "Shift",
            Key.LeftShift => "Shift",
            Key.Minus => "-",
            Key.Equals => "=",
            Key.Slash => "/",
            Key.Digit0 => "0",
            Key.Digit1 => "1",
            Key.Digit2 => "2",
            Key.Digit3 => "3",
            Key.Digit4 => "4",
            Key.Digit5 => "5",
            Key.Digit6 => "6",
            Key.Digit7 => "7",
            Key.Digit8 => "8",
            Key.Digit9 => "9",
            Key.RightBracket => "]",
            Key.LeftBracket => "[",
            Key.Semicolon => ";",
            Key.Quote => "'",
            Key.Period => ".",
            Key.Comma => ",",
            _ => key.ToString()
        };
    }

    public void SetPromptColor(KeyPromptColor color)
    {
        PromptColor = color;
        image.color = color switch {
            KeyPromptColor.Yellow => Color.yellow,
            KeyPromptColor.Blue => Color.blue,
            KeyPromptColor.Green => Color.green,
            KeyPromptColor.Red => Color.red,
            _ => Color.magenta
        };
    }
    
    public void SetColor(Color color)
    {
        image.color = color;
    }

    public void SetPressed(bool isPressed)
    {
        IsPressed = isPressed;
    }

    private void Update()
    {
        var delta = Time.deltaTime / transitionTime;
        t += IsPressed ? delta : -delta;
        t = Mathf.Clamp01(t);

        activeGroup.gameObject.SetActive(t > 0f);
        activeGroup.alpha = t;
    }
}
