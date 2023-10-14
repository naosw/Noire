using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using MessagePack;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("Debugging")]
    [SerializeField] private bool disableDataPersistence = false;
    [SerializeField] private bool initializeDataIfNull = false;

    [Header("File Storage Config")]
    [SerializeField] private string fileName;

    [Header("Autosave")] 
    [SerializeField] private float autoSaveTimeSeconds = 240;
    private Coroutine autoSaveCoroutine;
    
    // other fields
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
        
        fileHandler = new GameStateFileIO(Application.persistentDataPath, fileName);
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
        
        // start up the auto saving coroutine
        if (autoSaveCoroutine != null) 
            StopCoroutine(autoSaveCoroutine);
    }

    public void ChangeSelectedProfileId(string newProfileId) 
    {
        selectedProfileId = newProfileId;
        LoadGame();
    }

    public void NewGame(string profileId) 
    {
        gameData = new GameData(profileId);
        fileHandler.Save(gameData, selectedProfileId);
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
        gameData.LastUpdated = System.DateTime.Now.ToBinary();

        fileHandler.Save(gameData, selectedProfileId);
    }

    public void DeleteProfileData(string profileId)
    {
        fileHandler.Delete(profileId);
        InitializeSelectedProfileId();
        LoadGame();
    }

    public string CurrentScene => gameData.CurrentScene;
    
    private void InitializeSelectedProfileId() 
    {
        selectedProfileId = fileHandler.GetMostRecentlyUpdatedProfileId();
        gameData = fileHandler.Load(selectedProfileId);
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects() 
    {
        return FindObjectsOfType<MonoBehaviour>(true)
            .OfType<IDataPersistence>()
            .ToList();
    }

    public bool HasGameData()
    {
        if (gameData != null)
            return true;
        return GetAllProfilesGameData().Count > 0;
    }

    public Dictionary<string, GameData> GetAllProfilesGameData() 
    {
        return fileHandler.LoadAllProfiles();
    }
}