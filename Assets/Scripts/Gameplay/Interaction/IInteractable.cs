using UnityEngine;

/// <summary>
/// Interface for interactable objects.
/// </summary>
public interface IInteractable 
{
    void Interact();
    string GetInteractText();
    string GetCannotInteractText();
    bool CanInteract();
    Transform GetTransform();
}
