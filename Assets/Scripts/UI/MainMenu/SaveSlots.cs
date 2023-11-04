using System;
using Common.Extensions;
using TMPro;
using UnityEngine;

public class SaveSlot : UI
{
    [Header("Profile")]
    [SerializeField] private string profileId = "default";

    [SerializeField] private ButtonUI saveSlotButton;
    [SerializeField] private ButtonUI clearButton;
    [SerializeField] private TextMeshProUGUI percentageCompletedText;
    [SerializeField] private TextMeshProUGUI currentAreaText;

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
        SaveSlotsMenu.Instance.ToggleMenuButtons(false);

        if (!SaveSlotsMenu.Instance.IsLoadingView) 
        {
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
        else
        {
            // load the slot
            Loader.Load(DataPersistenceManager.Instance.CurrentScene); 
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
        SaveSlotsMenu.Instance.ToggleMenuButtons(false);
        
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
    
    public void SetData(GameData data) 
    {
        if (data == null)
        {
            hasData = false;

            saveSlotButton.SetText("..");
            SetInteractable(false);
            
            if (!SaveSlotsMenu.Instance.IsLoadingView)
                saveSlotButton.Enable();
        }
        else
        {
            hasData = true;
            
            SetInteractable(true);
            
            saveSlotButton.SetText(data.ProfileName);
            percentageCompletedText.text = data.PercentageComplete + "% Complete";
            currentAreaText.text = data.CurrentScene.SplitCamelCase();
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
        percentageCompletedText.gameObject.SetActive(interactable);
        currentAreaText.gameObject.SetActive(interactable);
    }
}
