using UnityEngine;

public class Lamp : InteractableObject
{
    public override void Interact()
    {
        if(!PauseMenuManager.Instance.IsGamePaused)
            GameEventsManager.Instance.BedRockPlainsEvents.LampInteract();
    }
}