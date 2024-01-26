using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] TextAsset wordsAsset;
    [SerializeField] KeyPromptController prompts;
    [SerializeField] KeyboardVisual visual;
    [SerializeField] KeyColorLayoutsScriptableObject keyColorLayouts;

    Keyboard current;
    Key[] keys;

    string[] words;
    int wordIndex = 0;
    int letterIndex = 0;

    int maxKeysHeldAtOnce = 3;
    int maxLettersBetweenHolds = 5;
    HashSet<char> heldLetters = new HashSet<char>();
    HashSet<char> releasePrompts = new HashSet<char>();

    KeyPromptColor heldColor = KeyPromptColor.None;
    bool hasReleasePrompt = false;

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

    private void Start()
    {
        SetKeyboardColors(keyColorLayouts.redKeys, Color.red);
        SetKeyboardColors(keyColorLayouts.greenKeys, Color.green);
        SetKeyboardColors(keyColorLayouts.blueKeys, Color.blue);
        SetKeyboardColors(keyColorLayouts.yellowKeys, Color.yellow);
    }

    private void Update()
    {
        if (prompts.PromptCount < 6 && wordIndex < words.Length)
        {
            var letter = words[wordIndex][letterIndex];


            letterIndex += 1;

            // Check for just pass the word length. When letter index
            // is equal to the word length assume that is a color
            // or randomizer prompt.
            if (letterIndex > words[wordIndex].Length)
            {
                letterIndex = 0;
                wordIndex += 1;
                if (wordIndex >= words.Length)
                {
                    Debug.Log("Game Won!");
                }
            }

            if (letterIndex < words[wordIndex].Length)
            {
                if (System.Enum.TryParse(letter.ToString(), true, out Key key))
                {
                    var prompt = prompts.SpawnKeyPrompt();
                    if (heldLetters.Contains(letter) && !releasePrompts.Contains(letter))
                    {
                        prompt.SetAsKeyReleasePrompt(key);
                        releasePrompts.Add(letter);
                    }
                    else if (CheckIfLetterShouldBeHeld(wordIndex, letterIndex))
                    {
                        prompt.SetAsKeyPrompt(key, true);
                    }
                    else
                    {
                        prompt.SetAsKeyPrompt(key, false);
                    }
                }
                else
                {
                    Debug.Log($"Failed to parse letter to key ({letter})");
                }
            }
            else // End of a word time for a color or randomizer
            {
                var prompt = prompts.SpawnKeyPrompt();
                if (heldColor != KeyPromptColor.None && !hasReleasePrompt)
                {
                    prompt.SetAsColorReleasePrompt(heldColor);
                }
                else
                {
                    if (Random.Range(0f, 1f) <= 0.8f)
                    {
                        var color = (KeyPromptColor)Random.Range(1, 5);
                        var isHeld = !hasReleasePrompt && Random.Range(0, 2) == 0;

                        prompt.SetAsColorPrompt(color, isHeld);
                        if (isHeld)
                        {
                            heldColor = color;
                        }
                    }
                    else
                    {
                        prompt.SetAsRandomizePrompt();
                    }
                }
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

    private bool CheckIfLetterShouldBeHeld(int wordIndex, int letterIndex)
    {
        

        if (wordIndex >= words.Length || letterIndex >= words[wordIndex].Length)
        {
            return false;
        }

        int heldKeys = heldLetters.Count + heldColor != KeyPromptColor.None ? 1 : 0;
        if (heldKeys >= maxKeysHeldAtOnce)
        {
            return false;
        }

        int distance = 0;
        char letter = words[wordIndex][letterIndex];

        if (heldLetters.Contains(letter))
        {
            return false;
        }

        for (int i = wordIndex; i < words.Length; i++)
        {
            for (int j = letterIndex; j < words[wordIndex].Length; j++)
            {
                distance += 1;
                if (distance >= maxLettersBetweenHolds)
                {
                    return false;
                }

                if (letter == words[wordIndex][letterIndex])
                {
                    return true;
                }
            }
            letterIndex = 0;
        }

        return false;
    }

    void SetKeyboardColors(Key[] keys, Color color)
    {
        foreach (var key in keys)
        {
            visual.GetKeyVisual(key).SetColor(color);
        }
    }
}