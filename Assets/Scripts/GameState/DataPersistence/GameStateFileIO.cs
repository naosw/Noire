#define DEBUG

using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using MessagePack;

public class GameStateFileIO
{
    private string dataDirPath = "";
    private string dataFileName = "";
    private readonly string backupExtension = ".bak";

    public GameStateFileIO(string dataDirPath, string dataFileName) 
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }

    public GameData Load(string profileId, bool allowRestoreFromBackup = true) 
    {
        if (profileId == null) 
            return null;

        // use Path.Combine -- different OS's having different path separators
        string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
        GameData loadedData = null;
        if (File.Exists(fullPath)) 
        {
            try 
            {
                byte[] dataToLoad = File.ReadAllBytes(fullPath);
                loadedData = MessagePackSerializer.Deserialize<GameData>(dataToLoad);
            }
            catch (Exception e) 
            {
                // since we're calling Load(..) recursively, we need to account for the case where
                // the rollback succeeds, but data is still failing to load for some other reason,
                // which without this check may cause an infinite recursion loop.
                if (allowRestoreFromBackup) 
                {
                    Debug.LogWarning("Failed to load data file. Attempting to roll back.\n" + e);
                    bool rollbackSuccess = AttemptRollback(fullPath);
                    if (rollbackSuccess)
                        loadedData = Load(profileId, false);
                }
                // if we hit this else block, one possibility is that the backup file is also corrupt
                else 
                {
                    Debug.LogError("Error occured when trying to load file at path: " 
                                   + fullPath  + " and backup did not work.\n" + e);
                }
            }
        }
        return loadedData;
    }

    public void Save(GameData data, string profileId) 
    {
        if (profileId == null) 
            return;

        string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
        string fullPathJson = Path.Combine(dataDirPath, profileId, dataFileName) + ".json";
        string backupPath = fullPath + backupExtension;
        
        try 
        {
            // create the directory the file will be written to if it doesn't already exist
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            
            // use MessagePack to binary format the GameData object
            byte[] dataToStore = MessagePackSerializer.Serialize(data);
            
#if DEBUG
    string jsonDataToStore = JsonUtility.ToJson(data, true);
    
    // write the serialized data to the file
    using (FileStream stream = new FileStream(fullPathJson, FileMode.Create))
    {
        using (StreamWriter writer = new StreamWriter(stream)) 
        {
            writer.Write(jsonDataToStore);
        }
    }
#endif
            
            File.WriteAllBytes(fullPath, dataToStore);

            GameData verifiedGameData = Load(profileId);
            if (verifiedGameData != null)
            {
                File.Copy(fullPath, backupPath, true);
                Debug.Log("Saved game to " + fullPath);
                Debug.Log("Saved backup game state to " + backupPath);
            }
            else
            {
                throw new Exception("Save file could not be verified. No backups created.");
            }
        }
        catch (Exception e) 
        {
            Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + e);
        }
    }

    public void Delete(string profileId) 
    {
        if (profileId == null) 
            return;

        string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
        try 
        {
            if (File.Exists(fullPath)) 
            {
                // delete the profile folder and everything within it
                Directory.Delete(Path.GetDirectoryName(fullPath), true);
            }
            else 
                Debug.LogWarning("Tried to delete profile data, but data was not found at path: " + fullPath);
        }
        catch (Exception e) 
        {
            Debug.LogError("Failed to delete profile data for profileId: " 
                           + profileId + " at path: " + fullPath + "\n" + e);
        }
    }
    
    public Dictionary<string, GameData> LoadAllProfiles() 
    {
        Dictionary<string, GameData> profileDictionary = new Dictionary<string, GameData>();

        // loop over all directory names in the data directory path
        IEnumerable<DirectoryInfo> dirInfos = new DirectoryInfo(dataDirPath).EnumerateDirectories();
        foreach (DirectoryInfo dirInfo in dirInfos) 
        {
            string profileId = dirInfo.Name;

            // defensive programming - check if the data file exists
            // if it doesn't, then this folder isn't a profile and should be skipped
            string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
            if (!File.Exists(fullPath))
            {
                Debug.LogWarning("Skipping directory when loading all profiles because it does not contain data: "
                    + profileId);
                continue;
            }

            // load the game data for this profile and put it in the dictionary
            GameData profileData = Load(profileId);
            // defensive programming - ensure the profile data isn't null,
            // because if it is then something went wrong and we should let ourselves know
            if (profileData != null) 
            {
                profileDictionary.Add(profileId, profileData);
            }
            else 
            {
                Debug.LogError("Tried to load profile but something went wrong. ProfileId: " + profileId);
            }
        }

        return profileDictionary;
    }

    public string GetMostRecentlyUpdatedProfileId() 
    {
        Dictionary<string, GameData> profilesGameData = LoadAllProfiles();

        if (profilesGameData == null || profilesGameData.Count == 0)
            return null;

        // MapReduce to find most recent save data id
        return profilesGameData
            .Select(entry => (entry.Key, DateTime.FromBinary(entry.Value.LastUpdated)))
            .Aggregate((a, b) => a.Item2 > b.Item2 ? a : b)
            .Key;
    }

    private bool AttemptRollback(string fullPath) 
    {
        bool success = false;
        string backupFilePath = fullPath + backupExtension;
        try 
        {
            // if the file exists, attempt to roll back to it by overwriting the original file
            if (File.Exists(backupFilePath))
            {
                File.Copy(backupFilePath, fullPath, true);
                success = true;
                Debug.LogWarning("Had to roll back to backup file at: " + backupFilePath);
            }
            // otherwise, we don't yet have a backup file - so there's nothing to roll back to
            else 
            {
                throw new Exception("Tried to roll back, but no backup file exists to roll back to.");
            }
        }
        catch (Exception e) 
        {
            Debug.LogError("Error occured when trying to roll back to backup file at: " 
                           + backupFilePath + "\n" + e);
        }

        return success;
    }
}