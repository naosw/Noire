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

    
    
    public float MaxBuffer => maxBuffer;
    public float CurrentBuffer => currentBuffer;
    public float MaxDrowsiness => maxDrowsiness;
    public float CurrentDrowsiness => currentDrowsiness;

    public void setMaxBuffer(float newValue)
    {
        maxBuffer = newValue;
    }

    public void SetCurrentBuffer(float newValue)
    {
        currentBuffer = newValue;
    }
	
    public void InflictDamage(float DamageValue)
    {
        if (DamageValue < 0) return;
        currentBuffer += DamageValue;
        if (currentBuffer > maxBuffer)
        {
            currentBuffer = 0;
            currentDrowsiness -= maxBufferDamage;
        }
    }

    public void RegenBuffer(float BufferRegenValue)
    {
        if(BufferRegenValue < 0) return;
        currentBuffer -= BufferRegenValue;
        if(currentBuffer < 0)
            currentBuffer = 0;
    }

    public void ResetHealth()
    {
        currentBuffer = 0;
        currentDrowsiness = maxDrowsiness;
    }
}
