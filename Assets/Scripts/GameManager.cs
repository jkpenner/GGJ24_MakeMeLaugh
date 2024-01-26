using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] KeyPromptController prompts;
    [SerializeField] KeyboardVisual visual;

    Keyboard current;
    Key[] keys;


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
    }

    private void Update()
    {
        if (prompts.PromptCount < 6)
        {
            var prompt = prompts.SpawnKeyPrompt();
            var rand = Random.Range(0, 1);
            if (rand == 0)
            {
                var key = (Key)Random.Range((int)Key.Space, (int)Key.Digit0);
                prompt.SetAsKeyPrompt(key, false);
            }
            else if (rand == 1)
            {
                prompt.SetAsColorPrompt((KeyPromptColor)Random.Range(1, 4), false);
            }
            else
            {
                prompt.SetAsRandomizePrompt();
            }
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