using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyPromptContainer : MonoBehaviour
{
    [SerializeField] KeyboardVisual keyboard;
    [SerializeField] Transform keyPromptParent;
    [SerializeField] KeyPrompt keyPromptPrefab;

    [SerializeField] float keyPromptDropSpeed = 500f;
    [SerializeField] float despawnSpeed = 1000f;
    [SerializeField] float despawnPosition = -160f;

    private RectTransform rectTransform;
    private List<KeyPrompt> despawned = new List<KeyPrompt>();
    private Dictionary<int, KeyPrompt> prompts = new Dictionary<int, KeyPrompt>();

    public int PromptCount => prompts.Count;
    public Dictionary<int, KeyPrompt> Prompts => prompts;
    public KeyPrompt CurrentPrompt => prompts.Count > 0 ? prompts[0] : null;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public KeyPrompt SpawnKeyPrompt(int stepIndex, KeySequenceStep step)
    {
        var instance = Instantiate(keyPromptPrefab);
        instance.transform.SetParent(keyPromptParent ?? transform);
        instance.RectTransform.anchorMin = new Vector2(0f, 0f);
        instance.RectTransform.anchorMax = new Vector2(1f, 0f);
        instance.RectTransform.anchoredPosition = new Vector2(0f, rectTransform.rect.height);
        instance.RectTransform.sizeDelta = new Vector2(0f, instance.RectTransform.sizeDelta.y);

        instance.Keyboard = keyboard;
        instance.Setup(step, stepIndex);
        prompts.Add(stepIndex, instance);
        return instance;
    }

    public void DespawnKeyPrompt(int stepIndex)
    {
        if (prompts.TryGetValue(stepIndex, out var toRemove))
        {
            despawned.Add(toRemove);
            prompts.Remove(stepIndex);
        }
    }

    private void Update()
    {
        float heightOffset = 0f;

        foreach (var prompt in prompts.Values)
        {
            var promptRectTrans = prompt.RectTransform;

            var dropAmount = Time.deltaTime * keyPromptDropSpeed;
            var target = Mathf.MoveTowards(promptRectTrans.anchoredPosition.y, heightOffset, dropAmount);
            promptRectTrans.anchoredPosition = new Vector2(0f, target);

            heightOffset = target + promptRectTrans.rect.height + 6f;
        }

        for (int i = despawned.Count - 1; i >= 0; i--)
        {
            var promptRectTrans = despawned[i].RectTransform;

            var dropAmount = Time.deltaTime * despawnSpeed;
            var target = Mathf.MoveTowards(promptRectTrans.anchoredPosition.y, despawnPosition, dropAmount);
            promptRectTrans.anchoredPosition = new Vector2(0f, target);

            if (promptRectTrans.anchoredPosition.y <= despawnPosition)
            {
                Destroy(despawned[i].gameObject);
                despawned.RemoveAt(i);
            }
        }
    }
}