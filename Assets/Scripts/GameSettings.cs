using UnityEngine;

public enum GameMode
{
    Fixed,
    Random,
}

[CreateAssetMenu(menuName = "Game Settings")]
public class GameSettings : ScriptableObject
{
    public int lifeCount = 3;
    public int maxKeysHeldAtOnce = 5;
    public bool showIncomingHighlights = true;


    [Header("Game Mode")]
    public GameMode mode;
    

    [Header("Fixed Settings")]
    public TextAsset sequenceSource;

    [Header("Random Settings")]
    public int generateGroupCount = 5;
    public bool infinite = false;

    [Header("Scoring")]
    public int baseButtonPressScore = 10;
    public int baseSequenceCompleteScore = 100;

    public int maxMultiplier = 4;
    public int requireStreakPerRank = 10;
    
    [Tooltip("Amount of score received for each life remaining at the end of the game.")]
    public int lifeScoreBonus = 1000;
    
    [Tooltip("The max amount of score player can receive base on their time")]
    public int timeScoreBonusMax = 1000;
    [Tooltip("The min amount of score player can receive base on their time")]
    public int timeScoreBonusMin = 100;

    [Tooltip("The amount of time until time bonus starts to decrease")]
    public float timeBonusFalloffStart = 30f;
    [Tooltip("The amount of time until time bonus is at it's minimum")]
    public float timeBonusFalloffEnd = 120f; // Seconds
}