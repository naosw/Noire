using UnityEngine;
using System;

public class BedRockPlainsEvents
{
    public event Action OnLampInteract;
    public void LampInteract() => OnLampInteract?.Invoke();
}