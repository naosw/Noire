using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveSlotsMenu : Menu
{
    [Header("Menu Navigation")]
    [SerializeField] private MainMenu mainMenu;

    [Header("Menu Buttons")]
    [SerializeField] private Button backButton;

    private SaveSlot[] saveSlots;

    private bool isLoadingGame = false;

    private void Awake() 
    {
        saveSlots = GetComponentsInChildren<SaveSlot>();
        backButton.onClick.AddListener(OnBackClicked);
        
        Hide();
    }
    
    private void OnBackClicked() 
    {
        mainMenu.Show();
        Hide();
    }

    public void Show(bool isLoading) 
    {
        gameObject.SetActive(true);
        isLoadingGame = isLoading;

        Dictionary<string, GameData> profilesGameData = DataPersistenceManager.Instance.GetAllProfilesGameData();

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

        // set the first selected button
        SetFirstSelected(firstSelected.GetComponent<Button>());
    }

    public void Refresh()
    {
        Show(isLoadingGame);
    }

    private void Hide() 
    {
        gameObject.SetActive(false);
    }

    public void DisableMenuButtons() 
    {
        foreach (SaveSlot saveSlot in saveSlots) 
            saveSlot.SetInteractable(false);
        backButton.interactable = false;
    }

    public bool IsLoading => isLoadingGame;
}