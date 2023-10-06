using UnityEngine;

[CreateAssetMenu(fileName = "CollectableItem", menuName = "Inventory/CollectableItem")]
public class CollectableItemSO : ScriptableObject
{
    [SerializeField] private string itemName;
    [SerializeField] private Sprite itemSprite;
    [SerializeField] private int stackUpperlimit;
    [TextArea] [SerializeField] private string itemDescription;

    public string Name => itemName;
    public Sprite Sprite => itemSprite;
    public int StackLimit => stackUpperlimit;
    public string Description => itemDescription;
}