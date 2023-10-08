using UnityEngine;

public class SaveInteractable : InteractableObject
{
    public override void Interact()
    {
        Debug.Log("Game saved!");
        DataPersistenceManager.Instance.SaveGame();
    }
}