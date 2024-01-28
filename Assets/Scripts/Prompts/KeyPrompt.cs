
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class KeyPrompt : MonoBehaviour
{
    [SerializeField] KeyVisual keyPrompt;
    [SerializeField] Image colorPrompt;
    [SerializeField] GameObject randomizerPrompt;
    [SerializeField] TMP_Text holdText;

    private RectTransform rectTransform;

    public int KeyIndex { get; private set; }
    public Key Key => Group?.Keys[KeyIndex] ?? Key.None;
    public KeySequenceGroup Group { get; private set; }

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

    public void Setup(KeySequenceGroup group, int keyIndex)
    {
        Group = group;
        KeyIndex = keyIndex;

        keyPrompt.SetKey(Key);

        // Search for the target key on the keyboard visual, then match it's size.
        if (Keyboard.TryGetKeyVisual(Key, out var keyVisual))
        {
            keyPrompt.transform.position = new Vector3(
                keyVisual.transform.position.x - keyVisual.GetComponent<RectTransform>().rect.width * 0.5f,
                keyPrompt.transform.position.y,
                keyPrompt.transform.position.z
            );

            var rVisual = keyPrompt.GetComponent<RectTransform>();
            var rKeyVisual = keyVisual.GetComponent<RectTransform>();

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
            Debug.LogWarning($"Failed to find key on keyboard for {Key}");
        }
    }
}