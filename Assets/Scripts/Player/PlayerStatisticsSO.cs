using UnityEngine;

/// <summary>
/// Manages a specific player's currency/statistics.
/// </summary>
[CreateAssetMenu(fileName = "Statistics", menuName = "Player/Statistics")]
public class PlayerStatisticsSO : ScriptableObject
{
    private float currencyCount;
    
    public float GetCurrencyCount() => currencyCount;
    
    public void SetCurrencyCount(float value) => currencyCount = value;

    public void Change(float value)
    {
        if(value > 0)
            currencyCount += value;
        else
        {
            currencyCount -= value;
            if (currencyCount < 0)
                currencyCount = 0;
        }
    }

    public void OnDeath()
    {
        currencyCount /= 2;
    }
}
