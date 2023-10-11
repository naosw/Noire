using System;
using UnityEngine;

public class SaveSlot : UI
{
    [Header("Profile")]
    [SerializeField] private string profileId = "default";

    [SerializeField] private ButtonUI saveSlotButton;
    [SerializeField] private ButtonUI clearButton;

    private bool hasData = false;

    private void Awake() 
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        saveSlotButton.AddListener(OnSaveSlotClicked);
        clearButton.AddListener(OnClear);
    }

    private void OnSaveSlotClicked() 
    {
        SaveSlotsMenu.Instance.DisableMenuButtons();
        
        if (hasData)
        {
            // prompt to start new game
            ConfirmationPopupMenu.Instance.ActivateMenu(
                "Starting a New Game with this slot will override the currently saved data. Are you sure?",
                () =>
                {
                    NewGame();
                },
                () =>
                {
                    SaveSlotsMenu.Instance.Show();
                }
            );
        }
        else 
        {
            NewGame();
        }
    }

    private void NewGame()
    {
        DataPersistenceManager.Instance.ChangeSelectedProfileId(profileId);
        DataPersistenceManager.Instance.NewGame(profileId);
        Loader.Load(Loader.FirstScene);
    }

    private void OnClear()
    {
        SaveSlotsMenu.Instance.DisableMenuButtons();
        
        ConfirmationPopupMenu.Instance.ActivateMenu(
            "Are you sure you want to delete this saved data?",
            () => {
                DataPersistenceManager.Instance.DeleteProfileData(profileId);
                if (!DataPersistenceManager.Instance.HasGameData())
                {
                    SaveSlotsMenu.Instance.BackToMainMenu();
                }
                else
                {
                    SaveSlotsMenu.Instance.Show();
                }
            },
            () => {
                SaveSlotsMenu.Instance.Show();
            }
        );
    }
    
    public void SetData(GameData data, bool isLoadingView) 
    {
        if (data == null)
        {
            hasData = false;
            
            clearButton.Disable();
            saveSlotButton.SetText("Empty");
            
            if(isLoadingView)
                saveSlotButton.Disable();
            else
                saveSlotButton.Enable();
        }
        else
        {
            hasData = true;
            
            clearButton.Enable();
            saveSlotButton.Enable();
            saveSlotButton.SetText(data.profileName + ":" + data.percentageComplete + "% COMPLETE");
        }
    }

    public string GetProfileId() => profileId;

    public void SetInteractable(bool interactable)
    {
        if (interactable)
        {
            saveSlotButton.Enable();
            clearButton.Enable();
        }
        else
        {
            saveSlotButton.Disable();
            clearButton.Disable();
        }
    }
}