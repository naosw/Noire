using UnityEngine;

public class SaveInteractable : InteractableObject
{
    [SerializeField] private ParticleSystemBase onSaveParticleSystem;
    [SerializeField] private Animator doorAnimator;
    private string DOOR_OPEN = "DoorOpen";
    
    public override void Interact()
    {
        doorAnimator.SetTrigger(DOOR_OPEN);
        onSaveParticleSystem.Restart();
        DataPersistenceManager.Instance.SaveGame();
        FinishInteract();
    }

    public override bool CanInteract()
    {
        return !disabled;
    }
}