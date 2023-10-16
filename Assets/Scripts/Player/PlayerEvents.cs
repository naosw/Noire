using System;
using UnityEngine;

public class PlayerEvents
{
    public event Action<float, Vector3> OnTakeDamage;
    public void TakeDamage(float value, Vector3 source) => OnTakeDamage?.Invoke(value, source);
    
    public event Action<float> OnHealthRegen;
    public void RegenHealth(float value) => OnHealthRegen?.Invoke(value);
    
    public event Action<float> OnDreamShardsChange;
    public void DreamShardsChange(float value) => OnDreamShardsChange?.Invoke(value);
    
    public event Action<float> OnDreamThreadsChange;
    public void DreamThreadsChange(float value) => OnDreamThreadsChange?.Invoke(value);
    
    public event Action OnDreamShardsChangeFinished;
    public void DreamShardsChangeFinished() => OnDreamShardsChangeFinished?.Invoke();
    
    public event Action OnDreamThreadsChangeFinished;
    public void DreamThreadsChangeFinished() => OnDreamThreadsChangeFinished?.Invoke();

    public event Action OnUpdateHealthBar;
    public void UpdateHealthBar() => OnUpdateHealthBar?.Invoke();
    
    public event Action OnUpdateStaminaBar;
    public void UpdateStaminaBar() => OnUpdateStaminaBar?.Invoke();
}
