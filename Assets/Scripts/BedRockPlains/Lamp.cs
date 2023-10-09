using UnityEngine;

public class Lamp : InteractableObject
{
    public override void Interact()
    {
        if (CanInteract())
        {
            interactionsOccured++;
            if (!PauseMenu.Instance.IsGamePaused)
                GameEventsManager.Instance.BedrockPlainsEvents.LampInteract();
        }
    }
}