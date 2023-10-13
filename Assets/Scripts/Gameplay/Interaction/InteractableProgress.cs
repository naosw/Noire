using MessagePack;

[MessagePackObject]
public struct InteractableProgress
{
    [Key(0)] public int interactionsOccured;
    [Key(1)] public bool disabled;

    [SerializationConstructor]
    public InteractableProgress(int interactionsOccured, bool disabled)
    {
        this.interactionsOccured = interactionsOccured;
        this.disabled = disabled;
    }
}