using System;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// A base class for interactable objects
/// </summary>

[RequireComponent(typeof(Collider))]
public class InteractableObject : MonoBehaviour, IInteractable, IDataPersistence
{
    [SerializeField] private string ID; // object id, should uniquely identifies an object
    [SerializeField] protected string interactText;
    [SerializeField] protected int maxInteractions;
    protected int interactionsOccured = 0;

    #region IInteractable

    public virtual void Interact()
    {
        Debug.Log("Interacted!");
    }

    public virtual bool CanInteract()
    {
        return interactionsOccured < maxInteractions;
    }
    
    public string GetInteractText() => interactText;
    
    public Transform GetTransform() => transform;

    #endregion

    #region IDataPersistence
    public void LoadData(GameData gameData)
    {
        if (gameData.InteractableProgress.TryGetValue(ID, out int value))
            interactionsOccured = value;
    }
    
    public void SaveData(GameData gameData)
    {
        gameData.InteractableProgress[ID] = interactionsOccured;
    }
    #endregion
}
