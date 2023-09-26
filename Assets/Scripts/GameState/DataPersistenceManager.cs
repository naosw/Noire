using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("Debugging")]
    [SerializeField] private bool disableDataPersistence = false;
    [SerializeField] private bool overrideSelectedProfileId = false;
    [SerializeField] private bool initializeDataIfNull = false;
    [SerializeField] private string testSelectedProfileId = "test";

    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;

    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private GameStateFileIO fileHandler;

    private string selectedProfileId = "";

    public static DataPersistenceManager Instance { get; private set; }

    private void Awake() 
    {
        // destroys duplicates for scene loading persistence
        if (Instance != null) 
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (disableDataPersistence) 
        {
            Debug.LogWarning("Data Persistence is currently disabled!");
        }
        
        fileHandler = new GameStateFileIO(Application.persistentDataPath, fileName, useEncryption);
        InitializeSelectedProfileId();
    }

    private void OnEnable() 
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable() 
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode) 
    {
        dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    public void ChangeSelectedProfileId(string newProfileId) 
    {
        selectedProfileId = newProfileId;
        LoadGame();
    }

    public void NewGame(string profileId) 
    {
        gameData = new GameData(profileId);
    }

    private void LoadGame()
    {
        if (disableDataPersistence) 
            return;

        gameData = fileHandler.Load(selectedProfileId);

        if (gameData == null && initializeDataIfNull)
            NewGame("dev_default_profile");
        
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects) 
            dataPersistenceObj.LoadData(gameData);
    }

    public void SaveGame()
    {
        if (disableDataPersistence || gameData == null) 
            return;
        
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects) 
            dataPersistenceObj.SaveData(gameData);

        // timestamp the data
        gameData.lastUpdated = System.DateTime.Now.ToBinary();

        fileHandler.Save(gameData, selectedProfileId);
    }

    public void DeleteProfileData(string profileId)
    {
        fileHandler.Delete(profileId);
        InitializeSelectedProfileId();
        LoadGame();
    }
    
    private void InitializeSelectedProfileId() 
    {
        selectedProfileId = fileHandler.GetMostRecentlyUpdatedProfileId();
        
        if (overrideSelectedProfileId) 
        {
            selectedProfileId = testSelectedProfileId;
            Debug.LogWarning("Overrode selected profile id with test id: " + testSelectedProfileId);
        }
    }

    private void OnApplicationQuit() 
    {
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects() 
    {
        return FindObjectsOfType<MonoBehaviour>(true)
            .OfType<IDataPersistence>()
            .ToList();
    }

    public bool HasGameData() 
    {
        return gameData != null;
    }

    public Dictionary<string, GameData> GetAllProfilesGameData() 
    {
        return fileHandler.LoadAllProfiles();
    }
}