using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class QuestPoint : MonoBehaviour
{
    [Header("Quest")]
    [SerializeField] private QuestInfoSO questInfoForPoint;

    [Header("Config")]
    [SerializeField] private bool startPoint = true;
    [SerializeField] private bool finishPoint = true;

    private string questId;
    private QuestState currentQuestState;

    private void Awake() 
    {
        questId = questInfoForPoint.id;
    }

    private void Start()
    {
        GameEventsManager.Instance.questEvents.OnQuestStateChange += QuestStateChange;
        GameInput.Instance.OnInteract += OnInteractQuest;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.questEvents.OnQuestStateChange -= QuestStateChange;
        GameInput.Instance.OnInteract -= OnInteractQuest;
    }

    private void OnInteractQuest(object sender, EventArgs e)
    {
        // checks if player is near first
        // TODO: implement interaction system
        if (Vector3.Distance(Player.Instance.transform.position, transform.position) > 5)
        {
            return;
        }

        // start or finish a quest
        if (currentQuestState.Equals(QuestState.CanStart) && startPoint)
        {
            GameEventsManager.Instance.questEvents.StartQuest(questId);
        }
        else if (currentQuestState.Equals(QuestState.CanFinish) && finishPoint)
        {
            GameEventsManager.Instance.questEvents.FinishQuest(questId);
        }
    }

    private void QuestStateChange(Quest quest)
    {
        // only update the quest state if this point has the corresponding quest
        if (quest.info.id.Equals(questId))
        {
            currentQuestState = quest.state;
        }
    }
}