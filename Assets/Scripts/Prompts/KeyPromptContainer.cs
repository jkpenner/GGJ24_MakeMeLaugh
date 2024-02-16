using System;
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
    [SerializeField] float bottomOffset = 10f;

    private RectTransform rectTransform;
    private List<KeyPrompt> despawned = new List<KeyPrompt>();

    public Dictionary<int, KeyPrompt> PromptIndexMap { get; private set; }
    public List<KeyPrompt> Prompts { get; private set; }

    public event Action PromptsCleared;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        
        PromptIndexMap = new Dictionary<int, KeyPrompt>();
        Prompts = new List<KeyPrompt>();
    }

    public void SpawnKeyPrompt(KeySequenceGroup group, int keyIndex)
    {
        var instance = Instantiate(keyPromptPrefab);
        instance.transform.SetParent(keyPromptParent ?? transform);
        instance.RectTransform.anchorMin = new Vector2(0f, 0f);
        instance.RectTransform.anchorMax = new Vector2(1f, 0f);
        instance.RectTransform.anchoredPosition = new Vector2(0f, rectTransform.rect.height);
        instance.RectTransform.sizeDelta = new Vector2(0f, instance.RectTransform.sizeDelta.y);
        instance.RectTransform.localScale = Vector2.one;

        instance.Keyboard = keyboard;
        instance.Setup(group, keyIndex);

        Prompts.Add(instance);
        PromptIndexMap.Add(keyIndex, instance);
    }

    public void DespawnAll()
    {
        for (int i = Prompts.Count - 1; i >= 0; i--)
        {
            var prompt = Prompts[i];
            
            despawned.Add(prompt);
            
            Prompts.RemoveAt(i);
            PromptIndexMap.Remove(prompt.KeyIndex);
        }
    }

    private void Update()
    {
        float heightOffset = bottomOffset;

        foreach (var prompt in Prompts)
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

                if (!AnyActivePrompts())
                {
                    PromptsCleared?.Invoke();
                }
            }
        }
    }

    public bool AnyActivePrompts()
    {
        return despawned.Count > 0 || Prompts.Count > 0;
    }

    public bool HasAnyMovingPrompts()
    {
        float heightOffset = bottomOffset;
        foreach (var prompt in Prompts)
        {
            var promptRectTrans = prompt.RectTransform;
            if (Mathf.Abs(promptRectTrans.anchoredPosition.y - heightOffset) > Mathf.Epsilon)
            {
                return true;
            }

            heightOffset += promptRectTrans.rect.height + 6f;
        }

        return false;
    }
}