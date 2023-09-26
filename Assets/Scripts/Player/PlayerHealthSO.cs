using Unity.Collections;
using UnityEngine;

/// <summary>
/// Manages the players "health"(drowsiness) for the whole game.
/// </summary>
[CreateAssetMenu(fileName = "Drowsiness", menuName = "Player/Drowsiness")]
public class PlayerHealthSO : ScriptableObject
{
    [Header("Buffer Settings")]
    [SerializeField][ReadOnly] private float maxBuffer;
    [SerializeField][ReadOnly] private float currentBuffer;
    [SerializeField][ReadOnly] private float maxBufferDamage = 10f;
    
    [Header("Drowsiness Settings")]
    [SerializeField][ReadOnly] private float maxDrowsiness;
    [SerializeField][ReadOnly] private float currentDrowsiness;
    
    public float GetMaxBuffer => maxBuffer;
    public float GetCurrentBuffer => currentBuffer;
    public float GetMaxDrowsiness => maxDrowsiness;
    public float GetCurrentDrowsiness => currentDrowsiness;
    public float GetCurrentDrowsinessPercentage => currentDrowsiness / maxDrowsiness;

    public void SetMaxBuffer(float newValue) => maxBuffer = newValue;

    public void SetCurrentBuffer(float newValue) => currentBuffer = newValue;
    
    public void SetMaxDrowsiness(float newValue) => maxDrowsiness = newValue;
	
    public void InflictDamage(float damageValue)
    {
        if (damageValue < 0) return;
        currentBuffer += damageValue;
        if (currentBuffer > maxBuffer)
        {
            currentBuffer = 0;
            currentDrowsiness -= maxBufferDamage;
        }
    }
    
    // returns 1 upon successful regen. Should not decrease potion otherwise
    public int RegenHealth(float regenValue)
    {
        if (currentDrowsiness == maxDrowsiness)
            return 0;
        
        currentDrowsiness += regenValue;
        if (currentDrowsiness >= maxDrowsiness)
            currentDrowsiness = maxDrowsiness;
        return 1;
    }

    public void RegenBuffer(float regenValue)
    {
        if(regenValue < 0) return;
        currentBuffer -= regenValue;
        if(currentBuffer < 0)
            currentBuffer = 0;
    }

    public void ResetHealth()
    {
        currentBuffer = 0;
        currentDrowsiness = maxDrowsiness / 2;
    }

    public bool IsDead()
    {
        return currentDrowsiness <= 0;
    }
}
