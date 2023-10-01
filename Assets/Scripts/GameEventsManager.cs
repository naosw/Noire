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
            Debug.LogError("Found more than one Game Events Manager in the scene.");
        }
        Instance = this;

        // initialize all events
        questEvents = new QuestEvents();
        playerEvents = new PlayerEvents();
    }
}