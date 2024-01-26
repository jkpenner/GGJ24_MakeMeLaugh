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
    [SerializeField] KeyVisual visual;

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

    public KeyboardVisual Keyboard { get; set; }

    public void SetPrompt(KeyPromptType type, Key key, KeyPromptColor color)
    {
        PromptType = type;
        PromptKey = key;
        PromptColor = color;

        // Setup Visuals Here..
        if (key != Key.None)
        {
            visual.gameObject.SetActive(true);
            visual.SetKey(key);
            if (Keyboard.TryGetKeyVisual(key, out var keyVisual))
            {
                visual.transform.position = new Vector3(
                    keyVisual.transform.position.x - keyVisual.GetComponent<RectTransform>().rect.width * 0.5f,
                    visual.transform.position.y,
                    visual.transform.position.z
                );

                var rVisual = visual.GetComponent<RectTransform>();
                var rKeyVisual = keyVisual.GetComponent<RectTransform>();

                

                Debug.Log($"{rVisual.rect.height}, {rKeyVisual.rect.height}");
                // var offset = (rVisual.rect.height - rKeyVisual.rect.height) * 0.5f;
                
                rVisual.anchorMax = new Vector2(0f, 0.5f);
                rVisual.anchorMin = new Vector2(0f, 0.5f);
                rVisual.pivot = new Vector2(0f, 0.5f);
                rVisual.anchoredPosition = new Vector2(rVisual.anchoredPosition.x, 0);
                rVisual.sizeDelta = rKeyVisual.sizeDelta;

                rVisual.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rKeyVisual.rect.width);
                rVisual.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rKeyVisual.rect.height);
            }
            else
            {
                Debug.LogWarning($"Failed to find key on keyboard for {key}");
            }
        }
        else
        {
            visual.gameObject.SetActive(false);
        }
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