using UnityEngine;

public class SaveInteractable : InteractableObject
{
    [SerializeField] private ParticleSystemBase onSaveParticleSystem;
    
    public override void Interact()
    {
        onSaveParticleSystem.Restart();
        Debug.Log("Game saved!");
        DataPersistenceManager.Instance.SaveGame();
    }

    public override bool CanInteract()
    {
        return true;
    }
}