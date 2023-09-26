[System.Serializable]
public class GameData
{
    // profile fields
    public string profileName;
    public long lastUpdated;
    public float percentageComplete;
    
    // player fields
    public float maxDrowsiness;
    public float attackDamage;
    public float dreamShards;
    public float dreamThreads;
    public float[] position;

    public GameData(string profileId)
    {
        maxDrowsiness = 50;
        attackDamage = 5;
        dreamShards = 0;
        dreamThreads = 0;
        position = new float[3];
        position[0] = 90;
        position[1] = 6.7f;
        position[2] = 83;

        profileName = profileId;
        percentageComplete = 0;
    }
}
