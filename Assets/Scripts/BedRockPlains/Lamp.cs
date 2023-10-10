using UnityEngine;

public class Lamp : InteractableObject
{
    [SerializeField] private BGMAudio bgmAudio;
    public override void Interact()
    {
        if (CanInteract())
        {
            interactionsOccured++;
            GameEventsManager.Instance.BedrockPlainsEvents.LampInteract();
            bgmAudio.PlayBgmAudio();
        }
    }
}