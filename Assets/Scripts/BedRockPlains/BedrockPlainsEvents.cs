using System;

public class BedrockPlainsEvents
{
    public event Action OnLampInteract;
    public void LampInteract() => OnLampInteract?.Invoke();
}