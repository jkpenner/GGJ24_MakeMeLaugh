using TMPro;
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
    private GameSettings settings;

    public bool IsPressed { get; private set; }
    public bool IsActiveKey { get; private set; }
    public bool IsErrorKey { get; private set; }

    private void Awake() {
        image = GetComponent<Image>();
        activeGroup.alpha = 0f;
    }

    public void SetGameSettings(GameSettings settings)
    {
        this.settings = settings;
    }

    public void SetKey(Key key)
    {
        label.text = key switch {
            Key.Backslash => "\\",
            Key.Backquote => "`",
            Key.CapsLock => "Caps Lock",
            Key.RightShift => "R Shift",
            Key.LeftShift => "L Shift",
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
            Key.LeftCtrl => "L Ctrl",
            Key.RightCtrl => "R Ctrl",
            _ => key.ToString()
        };
    }

    public void SetPressed(bool isPressed)
    {
        IsPressed = isPressed;
        UpdateActiveColor();
    }

    public void SetErrorKey(bool isErrorKey)
    {
        IsErrorKey = isErrorKey;
        UpdateActiveColor();
    }

    public void SetActiveKey(bool isActiveKey)
    {
        IsActiveKey = isActiveKey;
        UpdateActiveColor();
    }

    private void UpdateActiveColor()
    {
        Color color;
        if (IsErrorKey)
        {
            color = GameConsts.Red;
        }
        else if (IsActiveKey && IsPressed)
        {
            color = GameConsts.Green;
        }
        else
        {
            color = GameConsts.Blue;
        }

        activeGroup.GetComponent<Image>().color = color;
    }

    private bool ShowHighlighting()
    {
        return IsPressed || IsErrorKey || (IsActiveKey && (settings?.showIncomingHighlights ?? true));
    }

    private void Update()
    {
        var delta = Time.deltaTime / transitionTime;
        t += ShowHighlighting() ? delta : -delta;
        t = Mathf.Clamp01(t);

        activeGroup.gameObject.SetActive(t > 0f);
        activeGroup.alpha = t;
    }
}
