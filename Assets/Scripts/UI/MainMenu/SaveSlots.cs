using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
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

    [SerializeField] private SaveSlotsMenu saveSlotsMenu;

    private Button saveSlotButton;

    private void Awake() 
    {
        saveSlotButton = GetComponent<Button>();
        saveSlotButton.onClick.AddListener(OnSaveSlotClicked);
        clearButton.onClick.AddListener(OnClear);
    }
    
    private void OnSaveSlotClicked() 
    {
        saveSlotsMenu.DisableMenuButtons();

        DataPersistenceManager.Instance.ChangeSelectedProfileId(profileId);

        // create a new game - which will initialize our data to a clean slate
        if (!saveSlotsMenu.IsLoading) 
            DataPersistenceManager.Instance.NewGame(profileId);
        
        Loader.Load(Loader.Scene.ValleyofSolura);
    }

    private void OnClear()
    {
        DataPersistenceManager.Instance.DeleteProfileData(profileId);
        saveSlotsMenu.Refresh();
    }
    
    public void SetData(GameData data) 
    {
        if (data == null) 
        {
            noDataContent.SetActive(true);
            hasDataContent.SetActive(false);
            clearButton.gameObject.SetActive(false);
        }
        else 
        {
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