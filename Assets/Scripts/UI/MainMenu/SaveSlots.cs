using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SaveSlot : UI
{
    [Header("Profile")]
    [SerializeField] private string profileId = "default";

    [Header("Clear Button")] 
    [SerializeField] private Button clearButton;
    
    [Header("Content")]
    [SerializeField] private GameObject noDataContent;
    [SerializeField] private GameObject hasDataContent;
    [SerializeField] private TextMeshProUGUI percentageCompleteText;
    [SerializeField] private TextMeshProUGUI profileName;
    
    private bool hasData = false;
    private Button saveSlotButton;

    private void Awake() 
    {
        saveSlotButton = GetComponent<Button>();
        saveSlotButton.onClick.AddListener(OnSaveSlotClicked);
        clearButton.onClick.AddListener(OnClear);
    }
    
    private void OnSaveSlotClicked() 
    {
        SaveSlotsMenu.Instance.DisableMenuButtons();

        if (SaveSlotsMenu.Instance.IsLoading)
        {
            DataPersistenceManager.Instance.ChangeSelectedProfileId(profileId);
            Loader.Load(DataPersistenceManager.Instance.CurrentScene);
        }
        else if (hasData)
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
                    SaveSlotsMenu.Instance.Refresh();
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
                    SaveSlotsMenu.Instance.Refresh();                    
                }
            },
            () => {
                SaveSlotsMenu.Instance.Refresh();
            }
        );
    }
    
    public void SetData(GameData data) 
    {
        if (data == null)
        {
            hasData = false;
            noDataContent.SetActive(true);
            hasDataContent.SetActive(false);
            clearButton.gameObject.SetActive(false);
        }
        else
        {
            hasData = true;
            noDataContent.SetActive(false);
            hasDataContent.SetActive(true);
            clearButton.gameObject.SetActive(true);

            percentageCompleteText.text = data.percentageComplete + "% COMPLETE";
            profileName.text = data.profileName;
        }
    }

    public string GetProfileId() => profileId;

    public void SetInteractable(bool interactable)
    {
        saveSlotButton.interactable = interactable;
        clearButton.interactable = interactable;
    }
}