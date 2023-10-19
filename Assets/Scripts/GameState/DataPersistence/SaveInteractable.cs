using System;
using UnityEngine;

public class SaveInteractable : InteractableObject
{
    [SerializeField] private ParticleSystemBase onSaveParticleSystem;
    [SerializeField] private ParticleSystemBase onFirstSaveParticleSystem;

    private void Start()
    {
        if(interactionsOccured > 0)
            onFirstSaveParticleSystem.Stop();
        else
            onFirstSaveParticleSystem.Play();
    }

    public override void Interact()
    {
        interactionsOccured++;
        onFirstSaveParticleSystem.Stop();
        onSaveParticleSystem.Restart();
        DataPersistenceManager.Instance.SaveGame();
    }

    public override bool CanInteract()
    {
        return !disabled;
    }
}