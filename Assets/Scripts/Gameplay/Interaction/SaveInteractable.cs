using System;
using UnityEngine;

public class SaveInteractable : InteractableObject
{
    public override void Interact()
    {
        interactionsOccured++;
        interactableIndicator.Stop();
        onInteractIndicator.Restart();
        DataPersistenceManager.Instance.SaveGame();
    }

    public override bool CanInteract()
    {
        return !disabled;
    }
}