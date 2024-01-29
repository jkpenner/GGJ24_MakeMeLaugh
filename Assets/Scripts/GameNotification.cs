using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameNotification : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] TMP_Text header;
    [SerializeField] Image headerBackground;
    [SerializeField] TMP_Text body;
    [SerializeField] float displayDuration = 2f;
    [SerializeField] float fadeDuration = 0.75f;

    private float counter;
    private bool isFading;

    private void Start()
    {
        canvasGroup.alpha = 0f;
    }

    public void ShowMessage(string header, string body, Color headerColor)
    {
        canvasGroup.alpha = 1f;
        counter = 0f;
        isFading = false;

        this.header.text = header;
        this.body.text = body;
        this.headerBackground.color = headerColor;
    }

    private void Update()
    {
        if (canvasGroup.alpha > 0f)
        {
            counter += Time.deltaTime;
            if (isFading)
            {
                if (fadeDuration <= 0f)
                {
                    canvasGroup.alpha = 0f;
                }
                else
                {
                    canvasGroup.alpha = Mathf.Clamp01(1f - (counter / fadeDuration));
                    if (counter > fadeDuration)
                    {
                        canvasGroup.alpha = 0f;
                    }
                }
            }
            else
            {
                if (counter >= displayDuration)
                {
                    isFading = true;
                    counter = 0f;
                }
            }
        }
    }
}
