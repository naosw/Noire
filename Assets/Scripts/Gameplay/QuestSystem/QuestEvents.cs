using System;

public class QuestEvents
{
    public event Action<string> OnStartQuest;
    public void StartQuest(string id)
    {
        if (OnStartQuest != null)
        {
            OnStartQuest(id);
        }
    }

    public event Action<string> OnAdvanceQuest;
    public void AdvanceQuest(string id)
    {
        if (OnAdvanceQuest != null)
        {
            OnAdvanceQuest(id);
        }
    }

    public event Action<string> OnFinishQuest;
    public void FinishQuest(string id)
    {
        if (OnFinishQuest != null)
        {
            OnFinishQuest(id);
        }
    }

    public event Action<Quest> OnQuestStateChange;
    public void QuestStateChange(Quest quest)
    {
        if (OnQuestStateChange != null)
        {
            OnQuestStateChange(quest);
        }
    }

    public event Action<string, int, QuestStepState> OnQuestStepStateChange;
    public void QuestStepStateChange(string id, int stepIndex, QuestStepState questStepState)
    {
        if (OnQuestStepStateChange != null)
        {
            OnQuestStepStateChange(id, stepIndex, questStepState);
        }
    }
}