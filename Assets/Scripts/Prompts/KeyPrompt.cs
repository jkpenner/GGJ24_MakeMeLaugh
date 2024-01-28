
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Reflection.Emit;
using TMPro;
using System;

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
    None = 0,
    Yellow,
    Blue,
    Green,
    Red,
}

public class KeyPrompt : MonoBehaviour
{
    [SerializeField] KeyVisual keyPrompt;
    [SerializeField] Image colorPrompt;
    [SerializeField] GameObject randomizerPrompt;
    [SerializeField] TMP_Text holdText;

    private RectTransform rectTransform;

    public int StepIndex { get; private set; }
    public KeySequenceStep Step { get; private set; }

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

    public void Setup(KeySequenceStep step, int stepIndex)
    {
        Step = step;
        StepIndex = stepIndex;

        switch (Step.Type)
        {
            case StepType.Key:
                SetupKeyPrompt();
                break;
            case StepType.Color:
                SetupColorPrompt();
                break;
            case StepType.Randomizer:
                SetupRandomizerPrompt();
                break;
        }
    }

    private void SetupKeyPrompt()
    {
        // Hide all unneeded elements.
        colorPrompt.gameObject.SetActive(false);
        randomizerPrompt.gameObject.SetActive(false);

        // Show required elements.
        keyPrompt.gameObject.SetActive(true);

        // Show/Hide hold text with correct state text.
        holdText.gameObject.SetActive(Step.HoldType != HoldType.None);
        holdText.text = Step.HoldType == HoldType.Hold ? "Hold" : "Release";

        keyPrompt.SetKey(Step.Key);

        // Search for the target key on the keyboard visual, then match it's size.
        if (Keyboard.TryGetKeyVisual(Step.Key, out var keyVisual))
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
            Debug.LogWarning($"Failed to find key on keyboard for {Step.Key}");
        }
    }

    private void SetupColorPrompt()
    {
        // Hide all unneeded elements.
        randomizerPrompt.gameObject.SetActive(false);
        keyPrompt.gameObject.SetActive(false);

        // Show required elements.
        colorPrompt.gameObject.SetActive(true);

        // Show/Hide hold text with correct state text.
        holdText.gameObject.SetActive(Step.HoldType != HoldType.None);
        holdText.text = Step.HoldType == HoldType.Hold ? "Hold" : "Release";

        colorPrompt.color = Step.Color switch
        {
            KeyPromptColor.Yellow => Color.yellow,
            KeyPromptColor.Blue => Color.blue,
            KeyPromptColor.Green => Color.green,
            KeyPromptColor.Red => Color.red,
            _ => Color.magenta,
        };
    }

    private void SetupRandomizerPrompt()
    {
        // Hide all unneeded elements.
        keyPrompt.gameObject.SetActive(false);
        colorPrompt.gameObject.SetActive(false);
        holdText.gameObject.SetActive(false);

        // Show required elements.
        randomizerPrompt.gameObject.SetActive(true);
    }
}