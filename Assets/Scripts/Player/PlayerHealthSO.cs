using Unity.Collections;
using UnityEngine;

/// <summary>
/// Manages the players "health"(drowsiness) for the whole game.
/// </summary>
[CreateAssetMenu(fileName = "Drowsiness", menuName = "Player/Drowsiness")]
public class PlayerHealthSO : ScriptableObject
{
    private const float MAX_HP = 100;
    private float currentDrowsiness;
    
    public float CurrentDrowsiness => currentDrowsiness;
    public float CurrentDrowsinessPercentage => currentDrowsiness / MAX_HP;

    public void SetCurrentDrowsiness(float newValue) => currentDrowsiness = newValue;
	
    public void InflictDamage(float damageValue)
    {
        if (damageValue < 0) 
            return;
        currentDrowsiness -= damageValue;
    }
    
    // returns true upon successful regen. Should not decrease potion otherwise
    public bool RegenHealth(float regenValue)
    {
        if (currentDrowsiness >= MAX_HP || regenValue < 0)
            return false;
        
        currentDrowsiness += regenValue;
        if (currentDrowsiness >= MAX_HP)
            currentDrowsiness = MAX_HP;
        return true;
    }

    public void ResetHealth()
    {
        currentDrowsiness = MAX_HP * Player.Instance.DeepThreshold;
    }

    public bool IsDead()
    {
        return currentDrowsiness <= 0;
    }
}
