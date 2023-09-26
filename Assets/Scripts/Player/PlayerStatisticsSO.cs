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
    
    public void Increase(float increase) => currencyCount += increase;

    public void Decrease(float decrease)
    {
        currencyCount -= decrease;
        if (currencyCount < 0)
            currencyCount = 0;
    }
}
