using UnityEngine;

public class KeyPromptController : MonoBehaviour
{
    [SerializeField] KeyPrompt keyPromptPrefab;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        SpawnKeyPrompt();
    }

    public void SpawnKeyPrompt()
    {
        var instance = Instantiate(keyPromptPrefab);
        instance.transform.SetParent(transform);

        
        var yOffset = rectTransform.rect.height;
        instance.transform.localPosition = new Vector3(0f, yOffset, 0f);
    }
}