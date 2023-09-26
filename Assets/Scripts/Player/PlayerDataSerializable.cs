using Vector3 = UnityEngine.Vector3;

[System.Serializable]
public class PlayerDataSerializable
{
    private float maxDrowsiness;
    private float attackDamage;
    private float dreamShards;
    private float dreamThreads;
    private float[] position;

    public PlayerDataSerializable(Player player)
    {
        maxDrowsiness = player.GetMaximumDrowsiness();
        attackDamage = player.GetAttackDamage();
        dreamShards = player.GetDreamShards();
        dreamThreads = player.GetDreamThreads();

        position = new float[3];
        position[0] = player.x;
        position[1] = player.y;
        position[2] = player.z;
    }

    public void Apply(Player player)
    {
        player.SetMaximumDrowsiness(maxDrowsiness);
        player.SetAttackDamage(attackDamage);
        player.SetDreamShards(dreamShards);
        player.SetDreamThreads(dreamThreads);
        player.transform.position = new Vector3(position[0], position[1], position[2]);
    }
}
