using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestInfoSO", menuName = "QuestSystem/QuestInfo", order = 1)]
public class QuestInfoSO : ScriptableObject
{
    [field: SerializeField] public string id { get; private set; }

    [Header("General")] 
    [SerializeField] private string displayName;

    [Header("Requirements")] 
    public int levelRequirement;
    public QuestInfoSO[] prereqsRequirement;

    [Header("Steps")] 
    [SerializeField] private GameObject[] questStepPrefabs;

    [Header("Rewards")] 
    [SerializeField] private int dreamShardsReward;
    [SerializeField] private int dreamThreadsReward;
    // insert other possible rewards here
    
    private void OnValidate()
    {
        #if UNITY_EDITOR
        id = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
        #endif
    }
}
