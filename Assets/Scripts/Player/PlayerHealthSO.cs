using Unity.Collections;
using UnityEngine;

/// <summary>
/// Manages the players "health"(drowsiness) for the whole game.
/// </summary>
[CreateAssetMenu(fileName = "Drowsiness", menuName = "Player/Drowsiness")]
public class PlayerHealthSO : ScriptableObject
{
    public const float MAX_HP = 100;
    public float currentDrowsiness;
    
    public float GetCurrentDrowsiness => currentDrowsiness;
    public float GetCurrentDrowsinessPercentage => currentDrowsiness / MAX_HP;

    public void SetCurrentDrowsiness(float newValue) => currentDrowsiness = newValue;
	
    public void InflictDamage(float damageValue)
    {
        if (damageValue < 0) 
            return;
        currentDrowsiness -= damageValue;
    }
    
    // returns 1 upon successful regen. Should not decrease potion otherwise
    public int RegenHealth(float regenValue)
    {
        if (currentDrowsiness >= MAX_HP)
            return 0;
        
        currentDrowsiness += regenValue;
        if (currentDrowsiness >= MAX_HP)
            currentDrowsiness = MAX_HP;
        return 1;
    }

    public void ResetHealth()
    {
        currentDrowsiness = currentDrowsiness * (1 - Player.Instance.DeepThreshold);
    }

    public bool IsDead()
    {
        return currentDrowsiness <= 0;
    }
}
