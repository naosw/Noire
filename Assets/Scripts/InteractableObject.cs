using UnityEngine;
/// <summary>
/// A base class for interactable objects
/// </summary>
public class InteractableObject : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactText;
    
    public void Interact()
    {
        Debug.Log("Interacted!");
    }

    public string GetInteractText()
    {
        return interactText;
    }

    public Transform GetTransform() 
    { 
        return transform; 
    }
}
