using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// Manages saving/loading player game state as
/// local file through binary serialization
/// </summary>
public static class GameStateSaver
{
    private const string SAVE_PATH = "/player.state"; 
    
    public static void SavePlayer(Player player)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + SAVE_PATH;
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerDataSerializable data = new PlayerDataSerializable(player);
        
        formatter.Serialize(stream, data);
        stream.Close();
        Debug.Log("Saved game state to " + path);
    }

    public static PlayerDataSerializable LoadPlayer()
    {
        string path = Application.persistentDataPath + SAVE_PATH;
        if (!File.Exists(path))
        {
            Debug.LogError("Game state file not found in " + path);
            return null;
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Open);
            
        PlayerDataSerializable data = formatter.Deserialize(stream) as PlayerDataSerializable;
        stream.Close();

        return data;
    }
}
