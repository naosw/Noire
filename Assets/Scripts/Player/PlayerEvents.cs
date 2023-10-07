using System;

public class PlayerEvents
{
    public event Action<float> OnTakeDamage;
    public void TakeDamage(float value) => OnTakeDamage?.Invoke(value);
    
    public event Action<float> OnDreamShardsChange;
    public void DreamShardsChange(float value) => OnDreamShardsChange?.Invoke(value);
    
    public event Action<float> OnDreamThreadsChange;
    public void DreamThreadsChange(float value) => OnDreamThreadsChange?.Invoke(value);
}
