using UnityEngine;

/// <summary>
/// Interface for interactable objects.
/// </summary>
public interface IInteractable 
{
    void Interact();
    string GetInteractText();
    bool CanInteract();
    Transform GetTransform();
}
