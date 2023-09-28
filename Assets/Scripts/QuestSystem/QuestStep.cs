using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuestStep : MonoBehaviour
{
    private bool isFinished = false;

    protected void FinishQuest()
    {
        if (!isFinished)
        {
            isFinished = true;
            
            Destroy(gameObject);
        }
    }
    
}
