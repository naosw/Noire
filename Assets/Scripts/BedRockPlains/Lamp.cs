using UnityEngine;

public class Lamp : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactText;
    
    public void Interact()
    {
        GameEventsManager.Instance.bedRockPlainsEvents.LampInteract();
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