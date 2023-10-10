using System;

public class GameStateEvents
{
    public event Action<bool> OnPauseToggle;
    public void PauseToggle(bool paused) => OnPauseToggle?.Invoke(paused);

    public event Action<bool> OnLoadToggle;
    public void LoadToggle(bool finished) => OnLoadToggle?.Invoke(finished);
    
    public event Action<bool> OnUIToggle;
    public void UIToggle(bool isToggled) => OnUIToggle?.Invoke(isToggled);
}