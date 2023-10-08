using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Image sprite;
    [SerializeField] private TextMeshProUGUI stackCount;

    private void Awake()
    {
        ToggleEnable(false);
    }

    public void Display(CollectableItemSO newItem, int count)
    {
        ToggleEnable(true);
        
        sprite.sprite = newItem.Sprite;
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
        sprite.sprite = defaultSprite;
        stackCount.enabled = val;
    }
}