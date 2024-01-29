
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using Unity.VisualScripting;

public enum KeyPromptState
{
    Spawned,
    Normal,
    Success,
    Failed,
}

public class KeyPrompt : MonoBehaviour
{
    [SerializeField] KeyVisual keyPrompt;
    [SerializeField] Image backgroundImage;
    [SerializeField] GameObject holdVisuals;

    private RectTransform rectTransform;

    public int KeyIndex { get; private set; }
    public Key Key => Group?.Keys[KeyIndex] ?? Key.None;
    public KeySequenceGroup Group { get; private set; }
    public KeyPromptState State { get; private set; }

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
            var rVisual = keyPrompt.GetComponent<RectTransform>();
            var rKeyVisual = keyVisual.GetComponent<RectTransform>();

            keyPrompt.transform.position = new Vector3(
                keyVisual.transform.position.x - rKeyVisual.sizeDelta.x * 0.5f,
                keyPrompt.transform.position.y,
                keyPrompt.transform.position.z
            );

            Debug.Log($"Key Visual Width {rKeyVisual.sizeDelta.x} / {rKeyVisual.rect.width}");

            rVisual.anchorMax = new Vector2(0f, 1f);
            rVisual.anchorMin = new Vector2(0f, 0f);
            rVisual.pivot = new Vector2(0f, 0.5f);
            rVisual.anchoredPosition = new Vector2(rVisual.anchoredPosition.x, 0);
            rVisual.sizeDelta = new Vector2(rKeyVisual.sizeDelta.x, rVisual.sizeDelta.y);

            rVisual.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rKeyVisual.sizeDelta.x);
            // rVisual.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rKeyVisual.rect.height);
        }
        else
        {
            Debug.LogWarning($"Failed to find key on keyboard for {Key}");
        }

        SetAsNormal();
    }

    public void SetAsFailed()
    {
        if (State == KeyPromptState.Failed)
        {
            return;
        }

        State = KeyPromptState.Failed;

        backgroundImage.color = GameConsts.Red;
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 60f);
        holdVisuals.gameObject.SetActive(false);
    }

    public void SetAsSuccess()
    {
        if (State == KeyPromptState.Success)
        {
            return;
        }

        State = KeyPromptState.Success;

        backgroundImage.color = GameConsts.Green;
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 60f);
        holdVisuals.gameObject.SetActive(false);
    }

    public void SetAsNormal()
    {
        if (State == KeyPromptState.Normal)
        {
            return;
        }

        State = KeyPromptState.Normal;

        backgroundImage.color = GameConsts.Blue;
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 80f);
        holdVisuals.gameObject.SetActive(true);
    }
}