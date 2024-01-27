using UnityEngine;
using UnityEngine.InputSystem;

public enum StepType
{
    Key,
    Color,
    Randomizer,
}

public enum HoldType
{
    None,
    Hold,
    Release,
}

[System.Serializable]
public class KeySequenceStep
{
    [SerializeField] StepType type = StepType.Key;
    [SerializeField] Key key = Key.None;
    [SerializeField] KeyPromptColor color = KeyPromptColor.None;
    [SerializeField] HoldType holdType = HoldType.None;

    public StepType Type { get => type; set => type = value; }
    public Key Key { get => key; set => key = value; }
    public KeyPromptColor Color { get => color; set => color = value; }
    public HoldType HoldType { get => holdType; set => holdType = value; }
}