using UnityEngine;
/// <summary>
/// Handles all NON-INPUT events. Classes should subscribe/unsubscribe from events here.
/// </summary>
public class GameEventsManager : MonoBehaviour
{
    public static GameEventsManager Instance { get; private set; }

    public PlayerEvents PlayerEvents;
    public QuestEvents QuestEvents;
    public BedrockPlainsEvents BedrockPlainsEvents;

    private void Awake()
    {
        if (Instance != null) 
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // initialize all events
        QuestEvents = new QuestEvents();
        PlayerEvents = new PlayerEvents();
        BedrockPlainsEvents = new BedrockPlainsEvents();
    }
}