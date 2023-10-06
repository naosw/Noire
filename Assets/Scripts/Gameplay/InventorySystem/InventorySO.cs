using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory", menuName = "Inventory/Inventory")]
public class InventorySO : ScriptableObject
{
    public Dictionary<CollectableItemSO, int> inventory { get; private set; }
    
    public void Init()
    {
        inventory = new Dictionary<CollectableItemSO, int>();
    }
    
    // returns true upon successful add
    public bool Add(CollectableItemSO item)
    {
        inventory.TryAdd(item, 0);
        if (inventory[item] < item.StackLimit)
        {
            inventory[item]++;
            return true;
        }

        return false;
    }

    // returns true upon successful removal
    public bool Remove(CollectableItemSO item)
    {
        if (inventory.ContainsKey(item))
        {
            inventory[item]--;
            if (inventory[item] == 0)
                inventory.Remove(item);

            return true;
        }

        return false;
    }
}