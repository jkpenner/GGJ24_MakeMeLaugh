using TMPro;
using UnityEngine;

public class KeyVisual : MonoBehaviour
{
    [SerializeField] TMP_Text label;
    [SerializeField] CanvasGroup activeGroup;

    private float t = 0f;
    private float transitionTime = 0.2f;

    public bool IsPressed { get; private set; }

    private void Awake()
    {
        activeGroup.alpha = 0f;
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
