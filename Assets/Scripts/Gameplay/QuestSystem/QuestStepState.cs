// can add new fields to be used in data persistence and quest progress saver
[System.Serializable]
public class QuestStepState
{
    public string state;

    public QuestStepState(string s)
    {
        state = s;
    }

    public QuestStepState()
    {
        state = "";
    }
}