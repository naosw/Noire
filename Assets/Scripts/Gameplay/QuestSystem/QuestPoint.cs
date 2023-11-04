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
        GameEventsManager.Instance.QuestEvents.OnQuestStateChange += QuestStateChange;
        GameInput.Instance.OnInteract += OnInteractQuest;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.QuestEvents.OnQuestStateChange -= QuestStateChange;
        GameInput.Instance.OnInteract -= OnInteractQuest;
    }
    private void OnTriggerEnter(Collider otherCollider)
    {
        if (otherCollider.CompareTag("Player"))
        {
            Debug.Log("hi");
            GameEventsManager.Instance.QuestEvents.StartQuest(questId);
            
            if (currentQuestState.Equals(QuestState.CanFinish) && finishPoint)
            {
                GameEventsManager.Instance.QuestEvents.FinishQuest(questId);
            }
        }
    }

        private void OnInteractQuest()
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
            GameEventsManager.Instance.QuestEvents.StartQuest(questId);
        }
        else if (currentQuestState.Equals(QuestState.CanFinish) && finishPoint)
        {
            GameEventsManager.Instance.QuestEvents.FinishQuest(questId);
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