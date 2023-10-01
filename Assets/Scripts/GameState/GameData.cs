using MessagePack;
using UnityEngine;

[MessagePackObject]
public class GameData
{
    // profile fields
    [Key(0)] public string profileName;
    [Key(1)] public long lastUpdated;
    [Key(2)] public float percentageComplete;
    
    // player fields
    [Key(3)] public float drowsiness;
    [Key(4)] public float attackDamage;
    [Key(5)] public float dreamShards;
    [Key(6)] public float dreamThreads;
    [Key(7)] public Vector3 position;
    
    // game states
    [Key(8)] public string currentScene;
    
    [SerializationConstructor]
    public GameData(string profileId)
    {
        drowsiness = 50;
        attackDamage = 5;
        dreamShards = 0;
        dreamThreads = 0;
        position = new Vector3(90, 6.7f, 83);

        profileName = profileId;
        percentageComplete = 0;

        currentScene = Loader.firstScene.ToString();
    }
}
