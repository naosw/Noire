using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class exampleQuestStep : QuestStep
{
    private int interacts = 0;
    private int interactsToDo = 5;

    private void Start()
    {
        GameInput.Instance.OnInteract += OnInteract;
    }

    private void OnDisable()
    {
        GameInput.Instance.OnInteract -= OnInteract;
    }

    private void OnDestroy()
    {
        GameInput.Instance.OnInteract -= OnInteract;
    }
    
    private void OnInteract()
    {
        if (interacts < interactsToDo)
            interacts++;
        if (interacts >= interactsToDo)
        {
            FinishQuestStep();
        }
    }
    
    private void UpdateState()
    {
        string state = interacts.ToString();
        ChangeState(state);
    }

    protected override void SetQuestStepState(string state)
    {
        interacts = Int32.Parse(state);
        UpdateState();
    }
}
