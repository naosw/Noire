using UnityEngine;

public class Lamp : InteractableObject
{
    [SerializeField] private BGMAudio bgmAudio;
    public override void Interact()
    {
        interactionsOccured++;
        GameEventsManager.Instance.BedrockPlainsEvents.LampInteract();
        FinishInteract();
    }
}