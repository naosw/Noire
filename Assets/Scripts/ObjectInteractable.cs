using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInteractable : MonoBehaviour, IInteractable
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
