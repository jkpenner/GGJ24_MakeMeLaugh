using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyPromptController : MonoBehaviour
{
    [SerializeField] Transform keyPromptParent;
    [SerializeField] KeyPrompt keyPromptPrefab;

    [SerializeField] float keyPromptDropSpeed = 500f;
    [SerializeField] float despawnSpeed = 1000f;
    [SerializeField] float despawnPosition = -160f;

    private RectTransform rectTransform;
    private List<KeyPrompt> prompts = new List<KeyPrompt>();
    private List<KeyPrompt> despawned = new List<KeyPrompt>();

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        SpawnKeyPrompt();
    }

    public void SpawnKeyPrompt()
    {
        var instance = Instantiate(keyPromptPrefab);
        instance.transform.SetParent(keyPromptParent ?? transform);
        instance.RectTransform.anchorMin = new Vector2(0f, 0f);
        instance.RectTransform.anchorMax = new Vector2(1f, 0f);
        instance.RectTransform.anchoredPosition = new Vector2(0f, rectTransform.rect.height);
        instance.RectTransform.sizeDelta = new Vector2(0f, instance.RectTransform.sizeDelta.y);

        prompts.Add(instance);
    }

    public void DespawnKeyPrompt()
    {
        var toRemove = prompts[0];
        // Todo: Mark as removed..

        despawned.Add(toRemove);
        prompts.RemoveAt(0);
    }

    private void Update()
    {
        if (Keyboard.current[Key.Space].wasPressedThisFrame)
        {
            if (prompts.Count > 5)
            {
                DespawnKeyPrompt();
            }

            SpawnKeyPrompt();
        }

        float heightOffset = 0f;

        for (int i = 0; i < prompts.Count; i++)
        {
            var promptRectTrans = prompts[i].RectTransform;

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