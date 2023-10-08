using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveSlotsMenu : UI
{
    public static SaveSlotsMenu Instance { get; private set; }

    [SerializeField] private Button backButton;
    private SaveSlot[] saveSlots;
    private bool isLoadingGame = false;

    private void Awake()
    {
        Instance = this;
        
        saveSlots = GetComponentsInChildren<SaveSlot>();
        backButton.onClick.AddListener(BackToMainMenu);
        
        Hide();
    }
    
    public void BackToMainMenu() 
    {
        MainMenu.Instance.Show();
        Hide();
    }

    public void Activate(bool isLoading) 
    {
        gameObject.SetActive(true);
        isLoadingGame = isLoading;

        Dictionary<string, GameData> profilesGameData = DataPersistenceManager.Instance.GetAllProfilesGameData();

        backButton.interactable = true;

        // loop through each save slot in the UI and set the content appropriately
        GameObject firstSelected = backButton.gameObject;
        foreach (SaveSlot saveSlot in saveSlots) 
        {
            profilesGameData.TryGetValue(saveSlot.GetProfileId(), out GameData profileData);
            saveSlot.SetData(profileData);
            if (profileData == null && isLoadingGame) 
                saveSlot.SetInteractable(false);
            else 
            {
                saveSlot.SetInteractable(true);
                if (firstSelected.Equals(backButton.gameObject))
                    firstSelected = saveSlot.gameObject;
            }
        }
    }

    public void Refresh()
    {
        Activate(isLoadingGame);
    }

    public void DisableMenuButtons() 
    {
        foreach (SaveSlot saveSlot in saveSlots) 
            saveSlot.SetInteractable(false);
        backButton.interactable = false;
    }

    public bool IsLoading => isLoadingGame;
}