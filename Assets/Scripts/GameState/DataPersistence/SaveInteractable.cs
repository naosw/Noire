using UnityEngine;

public class SaveInteractable : InteractableObject
{
    [SerializeField] private ParticleSystemBase onSaveParticleSystem;
    
    public override void Interact()
    {
        onSaveParticleSystem.Restart();
        DataPersistenceManager.Instance.SaveGame();
        FinishInteract();
    }

    public override bool CanInteract()
    {
        return !disabled;
    }
}