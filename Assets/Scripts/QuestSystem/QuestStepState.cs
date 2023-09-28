using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestStepState
{
    public string state;

    public QuestStepState(string s)
    {
        state = s;
    }

    public QuestStepState()
    {
        state = "";
    }
}