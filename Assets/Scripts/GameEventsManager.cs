using UnityEngine;
/// <summary>
/// Handles all NON-INPUT events. Classes should subscribe/unsubscribe from events here.
/// </summary>
public class GameEventsManager : MonoBehaviour
{
    public static GameEventsManager Instance { get; private set; }

    public PlayerEvents playerEvents;
    public QuestEvents questEvents;

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
        questEvents = new QuestEvents();
        playerEvents = new PlayerEvents();
    }
}