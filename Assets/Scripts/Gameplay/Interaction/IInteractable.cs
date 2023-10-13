using UnityEngine;

/// <summary>
/// Interface for interactable objects.
/// </summary>
public interface IInteractable 
{
    void Interact();
    
    string GetInteractText();

    void Disable();
    
    void Enable();
    
    bool CanInteract();
    
    Transform GetTransform();
}
