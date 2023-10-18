using Unity.Collections;
using UnityEngine;

/// <summary>
/// Manages the players "health"(drowsiness) for the whole game.
/// </summary>
[CreateAssetMenu(fileName = "Drowsiness", menuName = "Player/Drowsiness")]
public class PlayerHealthSO : ScriptableObject
{
    private float max_hp = 100;
    private float currentDrowsiness;
    
    public float CurrentDrowsiness => currentDrowsiness;
    public float CurrentDrowsinessPercentage => currentDrowsiness / max_hp;

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
        if (currentDrowsiness >= max_hp || regenValue < 0)
            return false;
        
        currentDrowsiness += regenValue;
        if (currentDrowsiness >= max_hp)
            currentDrowsiness = max_hp;
        return true;
    }

    public void ResetHealth()
    {
        currentDrowsiness = max_hp * Player.Instance.DeepThreshold;
    }

    public bool IsDead()
    {
        return currentDrowsiness <= 0;
    }

    public void SetMaxHP(float x) => max_hp = x;
}
