using UnityEngine;

public class Lamp : InteractableObject
{
    public override void Interact()
    {
        if (CanInteract())
        {
            interactionsOccured++;
            if (!PauseMenuManager.Instance.IsGamePaused)
                GameEventsManager.Instance.BedrockPlainsEvents.LampInteract();
        }
    }
}