public class KeySequenceEventArgs
{
    public readonly KeySequence Sequence;

    public KeySequenceEventArgs(KeySequence sequence)
    {
        Sequence = sequence;
    }
}

public class KeySequenceStepEventArgs
{
    public readonly int StepIndex;
    public readonly KeySequenceStep Step;

    public KeySequenceStepEventArgs(int index, KeySequenceStep step)
    {
        StepIndex = index;
        Step = step;
    }
}