using UnityEngine;

public static class GameConsts
{
    public const float InactiveTimeout = 30f;

    public const int MinKeySeqence = 4;
    public const int MaxKeySeqence = 8;
    public const int DefaultMaxKeySequence = 5;
    public const string MaxSequenceLengthKey = "MaxSequenceLength";

    public const int StartupBuildIndex = 0;
    public const int MainMenuBuildIndex = 1;
    public const int GamePlayBuildIndex = 2;

    public readonly static Color Green = new Color(0.2f, 0.804f, 0.357f, 1f);
    public readonly static Color Red = new Color(0.929f, 0.306f, 0.263f, 1f);
    public readonly static Color Blue = new Color(0.267f, 0.498f, 0.886f, 1f);
    public readonly static Color Yellow = new Color(0.949f, 0.98f, 0.349f);
}