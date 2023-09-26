using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    [Header("Profile")]
    [SerializeField] private string profileId = "";

    [Header("Content")]
    [SerializeField] private GameObject noDataContent;
    [SerializeField] private GameObject hasDataContent;
    [SerializeField] private TextMeshProUGUI percentageCompleteText;

    [SerializeField] private SaveSlotsMenu saveSlotsMenu;

    private Button saveSlotButton;

    private void Awake() 
    {
        saveSlotButton = GetComponent<Button>();
        saveSlotButton.onClick.AddListener(OnSaveSlotClicked);
    }
    
    private void OnSaveSlotClicked() 
    {
        saveSlotsMenu.DisableMenuButtons();

        DataPersistenceManager.instance.ChangeSelectedProfileId(profileId);

        // create a new game - which will initialize our data to a clean slate
        if (!saveSlotsMenu.IsLoading) 
            DataPersistenceManager.instance.NewGame();
        Loader.Load(Loader.Scene.ValleyofSolura);
    }

    public void SetData(GameData data) 
    {
        if (data == null) 
        {
            noDataContent.SetActive(true);
            hasDataContent.SetActive(false);
        }
        else 
        {
            noDataContent.SetActive(false);
            hasDataContent.SetActive(true);

            percentageCompleteText.text = data.percentageComplete + "% COMPLETE";
        }
    }

    public string GetProfileId() => profileId;

    public void SetInteractable(bool interactable)
    {
        saveSlotButton.interactable = interactable;
    }
}