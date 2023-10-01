using System;

public class PlayerEvents
{
    public event Action<float> OnTakeDamage;
    public void TakeDamage(float value)
    {
        if (OnTakeDamage != null)
        {
            OnTakeDamage(value);
        }
    }
    
    public event Action<float> OnDreamShardsChange;
    public void DreamShardsChange(float value)
    {
        if (OnDreamShardsChange != null)
        {
            OnDreamShardsChange(value);
        }
    }
    
    public event Action<float> OnDreamThreadsChange;
    public void DreamThreadsChange(float value)
    {
        if (OnDreamThreadsChange != null)
        {
            OnDreamThreadsChange(value);
        }
    }
}
