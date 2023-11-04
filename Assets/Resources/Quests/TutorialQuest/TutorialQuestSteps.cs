using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class TutorialQuestSteps : QuestStep
{
    private void OnTriggerEnter(Collider otherCollider)
    {
       if (otherCollider.CompareTag("Player"))
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
