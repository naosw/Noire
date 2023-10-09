using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }
    
    // list of events
    public event Action<bool> OnCameraTurn; // 1 for right. 0 for left.
    public event Action OnPlayerMenuToggle;
    public event Action OnPauseAction;
    public event Action OnInteract;
    public event Action<int> OnAbilityCast;
    
    public enum Bindings
    {
        Menu,
        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight,
        CameraLeft,
        CameraRight,
        Interact,
        Dash,
        LightAttack,
        StrongAttack,
        Ability1,
        Ability2,
        Ability3,
    }

    private GameInputActions gameInputActions;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        gameInputActions = new GameInputActions();
    }
    
    // subscribe listeners
    private void Start()
    {
        // enable all maps
        gameInputActions.Player.Enable();
        gameInputActions.Menu.Enable();

        // handle subscriptions
        gameInputActions.Player.Menu.performed += PlayerMenu_performed;
        gameInputActions.Menu.Pause.performed += Pause_performed;
        
        gameInputActions.Player.CameraRight.performed += CameraRight_performed;
        gameInputActions.Player.CameraLeft.performed += CameraLeft_performed;
        
        gameInputActions.Player.LightAttack.performed += Attack1_performed;
        
        gameInputActions.Player.Interact.performed += Interact_performed;
        
        gameInputActions.Player.Dash.performed += Dash_performed;
        gameInputActions.Player.Ability1.performed += Ability1_performed;
        gameInputActions.Player.Ability2.performed += Ability2_performed;
        gameInputActions.Player.Ability3.performed += Ability3_performed;
        
        // game states
        GameEventsManager.Instance.GameStateEvents.OnPauseToggle += TogglePause;
        GameEventsManager.Instance.GameStateEvents.OnLoadToggle += ToggleLoad;
    }
    
    private void OnDisable()
    {
        gameInputActions.Player.Menu.performed -= PlayerMenu_performed;
        gameInputActions.Player.CameraRight.performed -= CameraRight_performed;
        gameInputActions.Player.CameraLeft.performed -= CameraLeft_performed;
        gameInputActions.Player.LightAttack.performed -= Attack1_performed;
        gameInputActions.Menu.Pause.performed -= Pause_performed;
        gameInputActions.Player.Interact.performed -= Interact_performed;
        gameInputActions.Player.Dash.performed -= Dash_performed;
        gameInputActions.Player.Ability1.performed -= Ability1_performed;
        gameInputActions.Player.Ability2.performed -= Ability2_performed;
        gameInputActions.Player.Ability3.performed -= Ability3_performed;
        
        // game states
        GameEventsManager.Instance.GameStateEvents.OnPauseToggle -= TogglePause;
        GameEventsManager.Instance.GameStateEvents.OnLoadToggle -= ToggleLoad;
        
        // dispose all maps
        gameInputActions.Dispose();
    }

    private void TogglePause(bool paused)
    {
        if (paused)
        {
            gameInputActions.Player.Disable();
        }
        else
        {
            gameInputActions.Player.Enable();
        }
    }

    private void ToggleLoad(bool finished)
    {
        if (finished)
        {
            gameInputActions.Player.Enable();
            gameInputActions.Menu.Enable();
        }
        else
        {
            gameInputActions.Player.Disable();
            gameInputActions.Menu.Disable();
        }
    }

    // invoke events
    
    private void PlayerMenu_performed(InputAction.CallbackContext obj) => OnPlayerMenuToggle?.Invoke();
    
    private void Pause_performed(InputAction.CallbackContext obj) => OnPauseAction?.Invoke();

    private void CameraLeft_performed(InputAction.CallbackContext obj)
    {
        OnCameraTurn?.Invoke(true);
    }

    private void CameraRight_performed(InputAction.CallbackContext obj)
    {
        OnCameraTurn?.Invoke(false);
    }
    
    private void Interact_performed(InputAction.CallbackContext obj) => OnInteract?.Invoke();
    
    private void Attack1_performed(InputAction.CallbackContext obj)
    {
        OnAbilityCast?.Invoke(0);
    }
    
    private void Dash_performed(InputAction.CallbackContext obj)
    {
        OnAbilityCast?.Invoke(1);
    }

    private void Ability1_performed(InputAction.CallbackContext obj)
    {
        OnAbilityCast?.Invoke(2);
    }
    
    private void Ability2_performed(InputAction.CallbackContext obj)
    {
        OnAbilityCast?.Invoke(3);
    }
    
    private void Ability3_performed(InputAction.CallbackContext obj)
    {
        OnAbilityCast?.Invoke(4);
    }
    
    public float GetZoomVal()
    {
        return gameInputActions.Player.CameraZoom.ReadValue<float>();
    }

    public Vector3 GetMovementVectorNormalized()
    {
        Vector2 readVal = gameInputActions.Player.Move.ReadValue<Vector2>();
        return new Vector3(readVal.x, 0, readVal.y);
    }

    public string GetBindingText(Bindings binding)
    {
        switch (binding)
        {
            default:
            case Bindings.MoveUp:
                return gameInputActions.Player.Move.bindings[1].ToDisplayString();
            case Bindings.MoveDown:
                return gameInputActions.Player.Move.bindings[2].ToDisplayString();
            case Bindings.MoveLeft:
                return gameInputActions.Player.Move.bindings[3].ToDisplayString();
            case Bindings.MoveRight:
                return gameInputActions.Player.Move.bindings[4].ToDisplayString();
            case Bindings.LightAttack:
                return gameInputActions.Player.LightAttack.bindings[0].ToDisplayString();
            case Bindings.StrongAttack:
                return gameInputActions.Player.StrongAttack.bindings[0].ToDisplayString();
            case Bindings.CameraLeft:
                return gameInputActions.Player.CameraLeft.bindings[0].ToDisplayString();
            case Bindings.CameraRight:
                return gameInputActions.Player.CameraRight.bindings[0].ToDisplayString();
            case Bindings.Dash:
                return gameInputActions.Player.Dash.bindings[0].ToDisplayString();
            case Bindings.Interact:
                return gameInputActions.Player.Interact.bindings[0].ToDisplayString();
            case Bindings.Ability1:
                return gameInputActions.Player.Ability1.bindings[0].ToDisplayString();
            case Bindings.Ability2:
                return gameInputActions.Player.Ability2.bindings[0].ToDisplayString();
            case Bindings.Ability3:
                return gameInputActions.Player.Ability3.bindings[0].ToDisplayString();
            case Bindings.Menu:
                return gameInputActions.Player.Menu.bindings[0].ToDisplayString();
        }
    }

    public void RebindBinding(Bindings binding, Action onActionRebound)
    {
        gameInputActions.Player.Disable();

        InputAction inputAction;
        int bindingIndex;

        switch (binding)
        {
            default:
            case Bindings.MoveUp:
                inputAction = gameInputActions.Player.Move;
                bindingIndex = 1;
                break;
            case Bindings.MoveDown:
                inputAction = gameInputActions.Player.Move;
                bindingIndex = 2;
                break;
            case Bindings.MoveLeft:
                inputAction = gameInputActions.Player.Move;
                bindingIndex = 3;
                break;
            case Bindings.MoveRight:
                inputAction = gameInputActions.Player.Move;
                bindingIndex = 4;
                break;
            case Bindings.CameraLeft:
                inputAction = gameInputActions.Player.CameraLeft;
                bindingIndex = 0;
                break;
            case Bindings.CameraRight:
                inputAction = gameInputActions.Player.CameraRight;
                bindingIndex = 0;
                break;
            case Bindings.LightAttack:
                inputAction = gameInputActions.Player.LightAttack;
                bindingIndex = 0;
                break;
            case Bindings.StrongAttack:
                inputAction = gameInputActions.Player.StrongAttack;
                bindingIndex = 0;
                break;
            case Bindings.Dash:
                inputAction = gameInputActions.Player.Dash;
                bindingIndex = 0;
                break;
            case Bindings.Interact:
                inputAction = gameInputActions.Player.Interact;
                bindingIndex = 0;
                break;
            case Bindings.Ability1:
                inputAction = gameInputActions.Player.Ability1;
                bindingIndex = 0;
                break;
            case Bindings.Ability2:
                inputAction = gameInputActions.Player.Ability2;
                bindingIndex = 0;
                break;
            case Bindings.Ability3:
                inputAction = gameInputActions.Player.Ability3;
                bindingIndex = 0;
                break;
        }

        inputAction.PerformInteractiveRebinding(bindingIndex)
            .WithCancelingThrough("escape")
            .WithControlsExcluding("escape")
            .OnComplete(callback =>
            {
                callback.Dispose();
                gameInputActions.Player.Enable();
                onActionRebound();
            })
            .Start();
    }
}