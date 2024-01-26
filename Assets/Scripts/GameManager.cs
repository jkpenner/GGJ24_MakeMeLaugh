using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] TextAsset wordsAsset;
    [SerializeField] KeyPromptController prompts;
    [SerializeField] KeyboardVisual visual;

    Keyboard current;
    Key[] keys;

    string[] words;
    int wordIndex = 0;
    int letterIndex = 0;

    private void Awake()
    {
        current = Keyboard.current;
        if (current is null)
        {
            Debug.LogWarning("No keyboard found");
        }

        keys = new Key[(int)Key.IMESelected - 1];
        for (int i = 1; i < (int)Key.IMESelected; i++)
        {
            keys[i - 1] = (Key)i;
        }

        if (wordsAsset is not null)
        {
            words = wordsAsset.text.Split("\n");
        }
        else
        {
            words = new string[] { "Hello", "World!" };
        }
    }

    private void Update()
    {
        if (prompts.PromptCount < 6 && wordIndex < words.Length)
        {
            var letter = words[wordIndex][letterIndex];

            letterIndex += 1;
            if (letterIndex >= words[wordIndex].Length)
            {
                letterIndex = 0;

                wordIndex += 1;
                if (wordIndex >= words.Length)
                {
                    Debug.Log("Game Won!");
                }
            }
            
            if (System.Enum.TryParse(letter.ToString(), true, out Key key))
            {
                var prompt = prompts.SpawnKeyPrompt();
                prompt.SetAsKeyPrompt(key, false);
            }
            else
            {
                Debug.Log($"Failed to parse letter to key ({letter})");
            }

            // var rand = Random.Range(0, 1);
            // if (rand == 0)
            // {

            // }
            // else if (rand == 1)
            // {
            //     prompt.SetAsColorPrompt((KeyPromptColor)Random.Range(1, 4), false);
            // }
            // else
            // {
            //     prompt.SetAsRandomizePrompt();
            // }
        }


        foreach (Key key in keys)
        {
            try
            {
                if (current[key].wasPressedThisFrame)
                {
                    var prompt = prompts.CurrentPrompt;
                    if (prompt is not null)
                    {
                        if (key == prompt.PromptKey)
                        {
                            prompts.DespawnKeyPrompt();
                        }
                    }

                    if (visual.TryGetKeyVisual(key, out KeyVisual keyVisual))
                    {
                        keyVisual.SetPressed(true);
                    }
                }

                if (current[key].wasReleasedThisFrame)
                {
                    if (visual.TryGetKeyVisual(key, out KeyVisual keyVisual))
                    {
                        keyVisual.SetPressed(false);
                    }
                }

            }
            catch
            {
                // Debug.LogError($"Key {key} received {e}");
            }
        }
    }
}