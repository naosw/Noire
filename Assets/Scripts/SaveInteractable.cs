using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveInteractable : MonoBehaviour, IInteractable
{

    [SerializeField] private string interactText;
    private DataPersistenceManager data = DataPersistenceManager.Instance;
    public void Interact()
    {
        Debug.Log("Interacted with save!");
        data.SaveGame();
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
