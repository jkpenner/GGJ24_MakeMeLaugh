using UnityEngine;
using UnityEngine.InputSystem;

public enum KeyPromptType
{
    None,
    Key,
    KeyHold,
    KeyRelease,
    Color,
    ColorHold,
    ColorRelease,
    Randomize,
}

public enum KeyPromptColor
{
    None,
    Yellow,
    Blue,
    Green,
    Red,
}

public class KeyPrompt : MonoBehaviour
{
    private RectTransform rectTransform;

    public KeyPromptType PromptType { get; private set; }
    public Key PromptKey { get; private set; }
    public KeyPromptColor PromptColor { get; private set; }

    public RectTransform RectTransform
    {
        get
        {
            if (rectTransform is null)
            {
                rectTransform = GetComponent<RectTransform>();
            }
            return rectTransform;
        }
    }

    public void SetPrompt(KeyPromptType type, Key key, KeyPromptColor color)
    {
        PromptType = type;
        PromptKey = key;
        PromptColor = color;

        // Setup Visuals Here..
    }

    public void SetAsKeyPrompt(Key key, bool isHold)
    {
        SetPrompt(isHold ? KeyPromptType.KeyHold : KeyPromptType.Key, key, KeyPromptColor.None);
    }

    public void SetAsKeyReleasePrompt(Key key)
    {
        SetPrompt(KeyPromptType.KeyRelease, key, KeyPromptColor.None);
    }

    public void SetAsColorPrompt(KeyPromptColor color, bool isHold)
    {
        SetPrompt(isHold ? KeyPromptType.ColorHold : KeyPromptType.Color, Key.None, color);
    }

    public void SetAsColorReleasePrompt(KeyPromptColor color)
    {
        SetPrompt(KeyPromptType.ColorRelease, Key.None, color);
    }

    public void SetAsRandomizePrompt()
    {
        SetPrompt(KeyPromptType.Randomize, Key.None, KeyPromptColor.None);
    }
}