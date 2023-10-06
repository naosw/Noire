using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image defaultSprite;
    
    // private CollectableItemSO item;
    
    private Image icon;
    private TextMeshProUGUI displayText;
    private TextMeshProUGUI stackCount;

    private void Awake()
    {
        icon = defaultSprite;
        displayText.enabled = false;
        stackCount.enabled = false;
    }

    public void Display(CollectableItemSO newItem, int count)
    {
        ToggleEnable(true);
        
        icon.sprite = newItem.Sprite;
        displayText.text = newItem.Description;
        if (count > 1)
            stackCount.text = count.ToString();
        else
            stackCount.enabled = false;
    }

    public void Clear()
    {
        ToggleEnable(false);
    }

    private void ToggleEnable(bool val)
    {
        icon.enabled = val;
        displayText.enabled = val;
        stackCount.enabled = val;
    }
}