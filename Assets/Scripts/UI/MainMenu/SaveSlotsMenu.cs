using System.Collections.Generic;
using UnityEngine;

public class SaveSlotsMenu : UI
{
    public static SaveSlotsMenu Instance { get; private set; }

    [SerializeField] private ButtonUI backButton;
    private SaveSlot[] saveSlots;

    private bool isLoadingView;

    private void Awake()
    {
        Instance = this;
        
        saveSlots = GetComponentsInChildren<SaveSlot>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        backButton.AddListener(BackToMainMenu);
        Hide();
    }

    public void BackToMainMenu() 
    {
        MainMenu.Instance.Show();
        Hide();
    }

    protected override void Activate()
    {
        Dictionary<string, GameData> profilesGameData = DataPersistenceManager.Instance.GetAllProfilesGameData();

        backButton.Enable();

        // loop through each save slot in the UI and set the content appropriately
        foreach (SaveSlot saveSlot in saveSlots) 
        {
            profilesGameData.TryGetValue(saveSlot.GetProfileId(), out GameData profileData);
            saveSlot.SetData(profileData, isLoadingView);
        }
    }

    public void NewGameMenu()
    {
        isLoadingView = false;
        Show();
    }
    
    public void LoadGameMenu()
    {
        isLoadingView = true;
        Show();
    }

    public void DisableMenuButtons() 
    {
        foreach (SaveSlot saveSlot in saveSlots) 
            saveSlot.SetInteractable(false);
        backButton.Disable();
    }
}