using System;
using TMPro;
using UnityEngine;

public class UserInterface : MonoBehaviour
{
    [SerializeField] KeyboardVisual keyboard;
    [SerializeField] KeyPromptContainer prompts;
    [SerializeField] TMP_Text scoreLabel;
    [SerializeField] TMP_Text multiplierLabel;
    [SerializeField] TMP_Text timerLabel;
    [SerializeField] TMP_Text lifesLabel;

    public KeyboardVisual Keyboard => keyboard;
    public KeyPromptContainer Prompts => prompts;

    public void SetScore(int score)
    {
        if (scoreLabel != null)
        {
            scoreLabel.text = score.ToString();
        }
    }

    public void SetLifes(int lifes)
    {
        if (lifesLabel != null)
        {
            lifesLabel.text = lifes.ToString();
        }
    }

    public void SetMultiplier(int multiplier)
    {
        if (multiplierLabel != null)
        {
            multiplierLabel.text = multiplier.ToString();
        }
    }

    public void SetTimer(float duration)
    {
        int seconds = Mathf.RoundToInt(duration) % 60;
        int minutes = Mathf.RoundToInt(duration) / 60;

        if (timerLabel != null)
        {
            if (minutes > 0)
            {
                timerLabel.text = $"{minutes}m {seconds}s";    
            }
            else
            {
                timerLabel.text = $"{seconds}s";
            }
        }
    }

    public void SetGameSettings(GameSettings settings)
    {
        keyboard.SetGameSettings(settings);
    }
}