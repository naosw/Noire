using System.Collections.Generic;
using MessagePack;
using UnityEngine;

[MessagePackObject]
public class GameData
{
    // profile fields
    [Key(0)] public string ProfileName;
    [Key(1)] public long LastUpdated;
    [Key(2)] public float PercentageComplete;
    
    // player fields
    [Key(3)] public float Drowsiness;
    [Key(4)] public float AttackDamage;
    [Key(5)] public float DreamShards;
    [Key(6)] public float DreamThreads;
    [Key(7)] public Vector3 Position;
    
    // game states
    [Key(8)] public string CurrentScene;

    [Key(9)] public Dictionary<int, int> InteractableProgress;
    
    [SerializationConstructor]
    public GameData(string profileId)
    {
        Drowsiness = 50;
        AttackDamage = 5;
        DreamShards = 0;
        DreamThreads = 0;
        Position = new Vector3(90, 6.7f, 83);

        ProfileName = profileId;
        PercentageComplete = 0;

        CurrentScene = Loader.FirstScene.ToString();
        
        // maps number of times each instance of interactable object has been interacted
        InteractableProgress = new();
    }
}
