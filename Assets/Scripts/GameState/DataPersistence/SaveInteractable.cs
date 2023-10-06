using UnityEngine;

public class SaveInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactText;
    
    public void Interact()
    {
        Debug.Log("Interacted with save!");
        DataPersistenceManager.Instance.SaveGame();
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