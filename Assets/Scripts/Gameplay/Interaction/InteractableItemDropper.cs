using UnityEngine;

/// <summary>
/// drops a collectableItemSO upon interaction
/// </summary>

public class InteractableItemDropper : InteractableObject
{
    [SerializeField] private CollectableItemSO[] itemDroppedWhenInteract;
    [SerializeField] private float dreamShardsAward;
    [SerializeField] private float dreamThreadsAward;
    
    public override void Interact()
    {
        onInteractIndicator.Play();
        interactionsOccured++;
        
        // TODO: play animation of item dropping
        // TODO: play sound of item dropping
        
        foreach (var item in itemDroppedWhenInteract)
            if (!Player.Instance.AddItem(item))
                Debug.Log("Cannot add item: reached item upper limit");

        GameEventsManager.Instance.PlayerEvents.DreamShardsChange(dreamShardsAward);
        GameEventsManager.Instance.PlayerEvents.DreamThreadsChange(dreamThreadsAward);
        FinishInteract();
    }
}