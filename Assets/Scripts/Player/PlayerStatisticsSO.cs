using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Manages a specific player's currency/statistics.
/// </summary>
[CreateAssetMenu(fileName = "Statistics", menuName = "Player/Statistics")]
public class PlayerStatisticsSO : ScriptableObject
{
    private int currencyCount;
    public int getCurrencyCount() => currencyCount;
    public void setCurrencyCount(int value) => currencyCount = value;
    public void increaseCurrencyCount(int increase) => currencyCount += increase;

    public void decreaseCurrencyCount(int decrease)
    {
        currencyCount -= decrease;
        if (currencyCount < 0)
            currencyCount = 0;
    }
}
