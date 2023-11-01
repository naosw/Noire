using Unity.Collections;
using UnityEngine;

/// <summary>
/// Manages the players "health"(drowsiness) for the whole game.
/// </summary>
[CreateAssetMenu(fileName = "Drowsiness", menuName = "Player/Drowsiness")]
public class PlayerHealthSO : ScriptableObject
{
    private int max_hp = 7;
    private int currentDrowsiness;
    
    public int CurrentDrowsiness => currentDrowsiness;
    public void SetCurrentDrowsiness(int newValue) => currentDrowsiness = newValue;
	
    public void InflictDamage(int damageValue)
    {
        if (damageValue < 0) 
            return;
        currentDrowsiness -= damageValue;
    }
    
    // returns true upon successful regen. Should not decrease potion otherwise
    public bool RegenHealth(int regenValue)
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
        currentDrowsiness = Player.Instance.DeepThreshold;
    }

    public bool IsDead()
    {
        return currentDrowsiness <= 0;
    }

    public void SetMaxHP(int x) => max_hp = x;
}
