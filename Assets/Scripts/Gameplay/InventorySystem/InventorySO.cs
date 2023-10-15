using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory", menuName = "Inventory/Inventory")]
public class InventorySO : ScriptableObject
{
    public Dictionary<CollectableItemSO, int> Inventory { get; private set; }
    private Dictionary<string, CollectableItemSO> AllItems;
    
    public void Init()
    {
        Inventory = new Dictionary<CollectableItemSO, int>();
        AllItems = Resources.LoadAll<CollectableItemSO>("Items")
            .ToDictionary(x => x.itemName, x => x);
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

    public Dictionary<string, int> ToSerializableInventory()
    {
        return Inventory.ToDictionary(
            x => x.Key.itemName, 
            x => x.Value);
    }

    public void FromSerializedInventory(Dictionary<string, int> serializedInventory)
    {
        Inventory = new();
        foreach (var kv in serializedInventory)
        {
            if (AllItems.TryGetValue(kv.Key, out CollectableItemSO item))
            {
                Inventory[item] = kv.Value;
            }
        }
    }
}