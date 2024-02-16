using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScorePopup : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] TMP_Text headerText;
    [SerializeField] Image headerBackground;
    [SerializeField] TMP_Text baseScoreValue;
    [SerializeField] TMP_Text lifeBonusValue;
    [SerializeField] TMP_Text timerBonusValue;
    [SerializeField] TMP_Text totalScoreValue;
    [SerializeField] float fadeDuration = 0.75f;

    [SerializeField] GameObject newHighScore;
    [SerializeField] TMP_Text highScoreLabel;
    [SerializeField] TMP_Text highScoreValue;

    private GameSettings settings;
    private bool isVisible = false;
    private float counter = 0f;

    public void Start()
    {
        canvasGroup.alpha = 0f;
    }

    public void SetGameSettings(GameSettings settings)
    {
        this.settings = settings;
    }

    public void Show(string header, int score, int lifes, float timer, Color headerColor)
    {
        isVisible = true;

        headerText.text = header;
        headerBackground.color = headerColor;

        var lifeBonus = lifes * settings.lifeScoreBonus;
        var timeBonus = Mathf.Clamp01((timer - settings.timeBonusFalloffStart) / (settings.timeBonusFalloffEnd - settings.timeBonusFalloffStart));
        timeBonus = (1f - timeBonus) * (settings.timeScoreBonusMax - settings.timeScoreBonusMin) + settings.timeScoreBonusMin;

        baseScoreValue.text = score.ToString();
        lifeBonusValue.text = lifeBonus.ToString();

        if (lifes != 0)
        {
            timerBonusValue.text = Mathf.RoundToInt(timeBonus).ToString();
        }
        else
        {
            timerBonusValue.text = "0";
        }

        var highScore = PlayerPrefs.GetInt(settings.name, 0);
        highScoreValue.text = highScore.ToString();
        

        var totalScore = Mathf.RoundToInt(score + lifeBonus + timeBonus);

        if (totalScore >= highScore)
        {
            PlayerPrefs.SetInt(settings.name, totalScore);
            newHighScore.SetActive(true);
            highScoreLabel.text = "Old High Score";
        }
        else
        {
            newHighScore.SetActive(false);
            highScoreLabel.text = "High Score";
        }

        totalScoreValue.text = totalScore.ToString();
    }

    private void Update()
    {
        if (isVisible && canvasGroup.alpha < 1f)
        {
            counter += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(counter / fadeDuration);

            if (counter >= fadeDuration)
            {
                canvasGroup.alpha = 1f;
            }
        }
    }
}
