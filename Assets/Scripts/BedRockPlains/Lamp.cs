public class Lamp : InteractableObject
{
    public override void Interact()
    {
        onInteractIndicator.Play();
        interactionsOccured++;
        GameEventsManager.Instance.BedrockPlainsEvents.LampInteract();
        FinishInteract();
    }
}