using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeySequenceGenerator
{
    public int MaxKeysHeldAtOnce { get; set; } = 3;
    public int MaxLettersBetweenHolds { get; set; } = 5;
    public float RandomizerSpawnChange { get; set; } = 0.2f;

    string[] words;
    int wordIndex;
    int letterIndex;

    HashSet<Key> heldKeys = new HashSet<Key>();
    KeyPromptColor heldColor = KeyPromptColor.None;

    public KeySequenceGenerator(string[] words)
    {
        this.words = words;
    }

    public KeySequence Generate()
    {
        List<KeySequenceStep> steps = new List<KeySequenceStep>();

        for (int wordIndex = 0; wordIndex < words.Length; wordIndex++)
        {
            var word = words[wordIndex];

            // Use less than equal to, when letter index is equal then that 
            // will be considered the end of the word and a color or other
            // step should be inserted.
            for (int letterIndex = 0; letterIndex <= word.Length; letterIndex++)
            {
                if (letterIndex < word.Length)
                {
                    char letter = word[letterIndex];
                    if (TryProduceKeyStep(letter, out KeySequenceStep step))
                    {
                        steps.Add(step);
                    }
                }
                else
                {
                    steps.Add(GenerateRandomFillerStep());
                }
            }
        }

        return new KeySequence(steps);
    }



    private int GetHeldKeyCount()
    {
        return heldKeys.Count + (heldColor != KeyPromptColor.None ? 1 : 0);
    }

    private bool TryProduceKeyStep(char letter, out KeySequenceStep step)
    {
        if (!System.Enum.TryParse(letter.ToString(), true, out Key key))
        {
            step = null;
            return false;
        }

        step = new KeySequenceStep
        {
            Type = StepType.Key,
            Key = key,
            HoldType = HoldType.None
        };

        if (CheckIfKeyShouldBeReleased(key))
        {
            step.HoldType = HoldType.Release;
            heldKeys.Remove(key);
        }
        else if (CheckIfKeyShouldBeHeld(key))
        {
            step.HoldType = HoldType.Hold;
            heldKeys.Add(key);
        }

        return true;
    }

    private bool CheckIfKeyShouldBeReleased(Key key)
    {
        if (!heldKeys.Contains(key))
        {
            return false;
        }

        return true;
    }

    private bool CheckIfKeyShouldBeHeld(Key key)
    {
        if (heldKeys.Contains(key))
        {
            return false;
        }

        if (GetHeldKeyCount() >= MaxKeysHeldAtOnce)
        {
            return false;
        }

        if (!IsNextMatchingKeyWithinDistance(key, MaxLettersBetweenHolds))
        {
            return false;
        }

        return true;
    }

    private bool IsNextEndOfWordWithinDistance(int maxDistance)
    {
        return words[wordIndex + 1].Length < maxDistance;
    }

    private bool IsNextMatchingKeyWithinDistance(Key key, int maxDistance)
    {
        int distance = 0;
        int workingIndex = letterIndex + 1;

        for (int i = wordIndex; i < words.Length; i++)
        {
            for (int j = workingIndex; j < words[wordIndex].Length; j++)
            {
                distance += 1;
                if (distance >= maxDistance)
                {
                    return false;
                }

                var letter = words[wordIndex][workingIndex].ToString();
                if (!System.Enum.TryParse(letter, true, out Key nextKey))
                {
                    continue;
                }

                if (nextKey == key)
                {
                    return true;
                }
            }
            workingIndex = 0;
        }

        return false;
    }

    private KeySequenceStep GenerateRandomFillerStep()
    {
        if (UnityEngine.Random.Range(0f, 1f) < RandomizerSpawnChange)
        {
            return GenerateRandomizerStep();
        }
        else
        {
            return GenerateColorStep();
        }
    }

    private KeySequenceStep GenerateColorStep()
    {
        if (heldColor != KeyPromptColor.None)
        {
            return new KeySequenceStep
            {
                Type = StepType.Color,
                Color = heldColor,
                HoldType = HoldType.Release,
            };
        }
        else
        {
            bool isHeld = true;
            if (GetHeldKeyCount() >= MaxKeysHeldAtOnce)
            {
                isHeld = false;
            }
            else if (!IsNextEndOfWordWithinDistance(MaxLettersBetweenHolds))
            {
                isHeld = false;
            }

            return new KeySequenceStep
            {
                Type = StepType.Color,
                Color = (KeyPromptColor)UnityEngine.Random.Range(1, 5),
                HoldType = isHeld ? HoldType.Hold : HoldType.None,
            };
        }
    }

    private KeySequenceStep GenerateRandomizerStep()
    {
        return new KeySequenceStep
        {
            Type = StepType.Randomizer
        };
    }
}