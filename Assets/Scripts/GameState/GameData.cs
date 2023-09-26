[System.Serializable]
public class GameData
{
    public float maxDrowsiness;
    public float attackDamage;
    public float dreamShards;
    public float dreamThreads;
    public float[] position;
    public long lastUpdated;
    public float percentageComplete;

    public GameData()
    {
        maxDrowsiness = 50;
        attackDamage = 5;
        dreamShards = 0;
        dreamThreads = 0;
        position = new float[3];
        position[0] = 90;
        position[1] = 6.7f;
        position[2] = 83;

        percentageComplete = 0;
    }
}
