using UnityEngine;

/// <summary>
/// A base class for interactable objects
/// </summary>

[RequireComponent(typeof(Collider))]
public class InteractableObject : MonoBehaviour, IInteractable, IDataPersistence
{
    [SerializeField] private string ID; // object id, should uniquely identifies an object
    [SerializeField] protected string interactText;
    [SerializeField] protected int maxInteractions;
    [SerializeField] protected ParticleSystemBase interactableIndicator;
    [SerializeField] protected ParticleSystemBase onInteractIndicator;
    protected int interactionsOccured = 0;
    protected bool disabled;

    private void Awake()
    {
        if(!interactableIndicator)
            Debug.LogError($"No interactable indicator found on {ID}");
        if(!onInteractIndicator)
            Debug.LogError($"No onInteractIndicator found on {ID}");
    }

    protected void FinishInteract()
    {
        if(!CanInteract())
            interactableIndicator.Stop();
    }
    
    #region IInteractable
    
    public virtual void Interact()
    {
        Debug.Log("Interacted!");
    }

    public virtual bool CanInteract()
    {
        return !disabled && interactionsOccured < maxInteractions;
    }

    public virtual void Disable()
    {
        disabled = true;
        interactableIndicator.Stop();
    }
    
    public virtual void Enable()
    {
        disabled = false;
        if (CanInteract())
            interactableIndicator.Play();
    }
    
    public virtual string GetInteractText() => interactText;
    
    public virtual Transform GetTransform() => transform;

    #endregion

    #region IDataPersistence
    public void LoadData(GameData gameData)
    {
        if (gameData.InteractableProgress.TryGetValue(ID, out InteractableProgress progress))
        {
            interactionsOccured = progress.interactionsOccured;
            disabled = progress.disabled;
            if (CanInteract())
                interactableIndicator.Play();
        }
    }
    
    public void SaveData(GameData gameData)
    {
        gameData.InteractableProgress[ID] = new InteractableProgress(interactionsOccured, disabled);
    }
    #endregion
}
