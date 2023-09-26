using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : Menu
{
    [Header("Menu Navigation")]
    [SerializeField] private SaveSlotsMenu saveSlotsMenu;

    [Header("Menu Buttons")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueGameButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button quitGameButton;

    private void Awake()
    {
        newGameButton.onClick.AddListener(OnNewGameClicked);
        continueGameButton.onClick.AddListener(OnContinueGameClicked);
        loadGameButton.onClick.AddListener(OnLoadGameClicked);
        quitGameButton.onClick.AddListener(OnQuitGameClicked);
        
        ActivateMenu();
    }

    private void Start() 
    {
        if (!DataPersistenceManager.instance.HasGameData()) 
        {
            continueGameButton.interactable = false;
            loadGameButton.interactable = false;
        }
    }

    private void OnNewGameClicked() 
    {
        saveSlotsMenu.ActivateMenu(false);
        DeactivateMenu();
    }

    private void OnContinueGameClicked() 
    {
        DisableMenuButtons();
        // TODO: add which scene in GameData
        Loader.Load(Loader.Scene.ValleyofSolura);
    }
    
    private void OnLoadGameClicked() 
    {
        saveSlotsMenu.ActivateMenu(true);
        DeactivateMenu();
    }

    private void OnQuitGameClicked()
    {
        Application.Quit();
    }

    private void DisableMenuButtons() 
    {
        newGameButton.interactable = false;
        continueGameButton.interactable = false;
    }

    public void ActivateMenu() 
    {
        gameObject.SetActive(true);
    }

    public void DeactivateMenu() 
    {
        gameObject.SetActive(false);
    }
}