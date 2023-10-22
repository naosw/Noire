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
    [Key(3)] public float AttackDamage;
    [Key(4)] public float DreamShards;
    [Key(5)] public float DreamThreads;
    [Key(6)] public Vector3 Position;
    [Key(10)] public Dictionary<string, int> Inventory;
    
    // game states
    [Key(7)] public string CurrentScene;
    
    // BedrockPlains
    [Key(8)] public Dictionary<string, InteractableProgress> InteractableProgress;
    [Key(9)] public bool LightsOpen;
    
    [SerializationConstructor]
    public GameData(string profileId)
    {
        AttackDamage = 5;
        DreamShards = 0;
        DreamThreads = 0;
        Position = new Vector3(92.64f, 6f, 113.99f);

        ProfileName = profileId;
        PercentageComplete = 0;

        CurrentScene = Loader.FirstScene.ToString();

        Inventory = new();
        
        // maps number of times each instance of interactable object has been interacted
        InteractableProgress = new();
    }
}