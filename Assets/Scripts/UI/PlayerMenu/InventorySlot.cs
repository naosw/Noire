using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Image sprite;
    [SerializeField] private TextMeshProUGUI stackCount;
    [SerializeField] private TextMeshProUGUI displayText;

    private string currItemDescription;
    private Button inventorySlotButton;

    private void Awake()
    {
        ToggleEnable(false);
        inventorySlotButton = GetComponent<Button>();
        inventorySlotButton.onClick.AddListener(OnInventorySlotClicked);
        displayText.enabled = false;
    }

    public void Display(CollectableItemSO newItem, int count)
    {
        ToggleEnable(true);
        
        sprite.sprite = newItem.Sprite;
        currItemDescription = newItem.Description;
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

    private void OnInventorySlotClicked()
    {
        displayText.text = currItemDescription;
        displayText.enabled = true;
    }
}