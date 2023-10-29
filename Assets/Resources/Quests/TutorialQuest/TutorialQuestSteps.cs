using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialQuestSteps : QuestStep
{
    private void Update()
    {
       if  (Input.GetKeyDown(KeyCode.W))
        {
            FinishQuestStep();
            Debug.Log("finished quest");
        }
    }
    protected override void SetQuestStepState(string state)
    {
        // no state required
    }
}
