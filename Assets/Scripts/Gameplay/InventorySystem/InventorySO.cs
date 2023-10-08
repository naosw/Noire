using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory", menuName = "Inventory/Inventory")]
public class InventorySO : ScriptableObject
{
    public Dictionary<CollectableItemSO, int> Inventory { get; private set; }
    
    public void Init()
    {
        Inventory = new Dictionary<CollectableItemSO, int>();
    }
    
    // returns true upon successful add
    public bool Add(CollectableItemSO item)
    {
        Inventory.TryAdd(item, 0);
        if (Inventory[item] < item.StackLimit)
        {
            Inventory[item]++;
            return true;
        }

        return false;
    }

    // returns true upon successful removal
    public bool Remove(CollectableItemSO item)
    {
        if (Inventory.ContainsKey(item))
        {
            Inventory[item]--;
            if (Inventory[item] == 0)
                Inventory.Remove(item);

            return true;
        }

        return false;
    }
}