using UnityEngine;


public class PlayerMenu : UI
{
    public static PlayerMenu Instance { get; private set; }
    
    // [Header("PlayerMenu Navigation Buttons")]
    // [SerializeField] private Button swipeLeft;
    // [SerializeField] private Button swipeRight;

    [SerializeField] private InventorySO playerInventory;
    private bool isToggledOn = false;

    private InventorySlot[] inventoryDisplay;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameInput.Instance.OnPlayerMenuToggle += GameInput_OnPlayerMenuToggle;

        inventoryDisplay = GetComponentsInChildren<InventorySlot>();
        
        Hide();
    }
    
    private void OnDestroy()
    {
        GameInput.Instance.OnPlayerMenuToggle -= GameInput_OnPlayerMenuToggle;
    }

    private void GameInput_OnPlayerMenuToggle()
    {
        isToggledOn = !isToggledOn;
        if (isToggledOn)
        {
            Show();
        }
        else
        {
            Hide();
        }
        GameEventsManager.Instance.GameStateEvents.UIToggle(isToggledOn);
    }

    private new void Show()
    {
        gameObject.SetActive(true);

        int i = 0;
        if(playerInventory.Inventory.Count > inventoryDisplay.Length)
            Debug.LogError("Inventory overflow");
        foreach (var (item, count) in playerInventory.Inventory)
        {
            inventoryDisplay[i].Display(item, count);
            i++;
        }
    }
}