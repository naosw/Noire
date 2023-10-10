using System;
using UnityEngine;
/// <summary>
/// A base class for interactable objects
/// </summary>

[RequireComponent(typeof(Collider))]
public class InteractableObject : MonoBehaviour, IInteractable
{
    [SerializeField] protected string interactText;
    [SerializeField] protected string cannotInteractText;
    [SerializeField] protected int maxInteractions;
    protected int interactionsOccured = 0;

    public virtual void Interact()
    {
        Debug.Log("Interacted!");
    }

    public virtual bool CanInteract()
    {
        return interactionsOccured < maxInteractions;
    }


    public string GetInteractText() => interactText;
    public string GetCannotInteractText() => cannotInteractText;

    public Transform GetTransform() 
    { 
        return transform; 
    }
}
