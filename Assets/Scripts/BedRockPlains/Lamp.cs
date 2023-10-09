using UnityEngine;

public class Lamp : InteractableObject
{
    [SerializeField] private BGMAudio bgmAudio;
    public override void Interact()
    {
        if (CanInteract())
        {
            interactionsOccured++;
            if (!PauseMenu.Instance.IsGamePaused)
            {
                GameEventsManager.Instance.BedrockPlainsEvents.LampInteract();
                bgmAudio.PlayBgmAudio();
            }
        }
    }
}