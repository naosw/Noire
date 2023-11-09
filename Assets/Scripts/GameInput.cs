using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }
    
    public event Action<bool> OnCameraTurn; // true iff right turn
    public event Action OnPlayerMenuToggle;
    public event Action OnPauseToggle;
    public event Action OnInteract;
    public event Action<int> OnAbilityCast;
    public event Action OnDebugConsoleToggle;
    public event Action OnDebugConsoleExecute;
    
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
        gameInputActions = new GameInputActions();
        
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // enable all maps
        gameInputActions.Player.Enable();
        gameInputActions.Menu.Enable();
        gameInputActions.UI.Enable();
        gameInputActions.Debug.Enable();
        
        // debug
        gameInputActions.Debug.ToggleDebug.performed += ToggleDebugConsole;
        gameInputActions.Debug.Execute.performed += ExecuteDebugConsole;

        // handle subscriptions
        gameInputActions.UI.Menu.performed += PlayerMenu_performed;
        gameInputActions.Menu.Pause.performed += Pause_performed;
        
        gameInputActions.Player.CameraRight.performed += CameraRight_performed;
        gameInputActions.Player.CameraLeft.performed += CameraLeft_performed;
        
        gameInputActions.Player.LightAttack.performed += Attack1_performed;
        
        gameInputActions.Player.Interact.performed += Interact_performed;
        
        gameInputActions.Player.Dash.performed += Dash_performed;
        gameInputActions.Player.Ability1.performed += Ability1_performed;
        gameInputActions.Player.Ability2.performed += Ability2_performed;
        gameInputActions.Player.Ability3.performed += Ability3_performed;
    }

    private void Start()
    {
        // game states
        GameEventsManager.Instance.GameStateEvents.OnPauseToggle += TogglePause;
        GameEventsManager.Instance.GameStateEvents.OnLoadToggle += ToggleLoad;
        GameEventsManager.Instance.GameStateEvents.OnUIToggle += ToggleUI;
        GameEventsManager.Instance.GameStateEvents.OnMenuToggle += ToggleMenu;
    }

    private void OnDisable()
    {
        // debug
        gameInputActions.Debug.ToggleDebug.performed -= ToggleDebugConsole;
        gameInputActions.Debug.Execute.performed -= ExecuteDebugConsole;
        
        gameInputActions.UI.Menu.performed -= PlayerMenu_performed;
        gameInputActions.Menu.Pause.performed -= Pause_performed;
        
        gameInputActions.Player.CameraRight.performed -= CameraRight_performed;
        gameInputActions.Player.CameraLeft.performed -= CameraLeft_performed;
        
        gameInputActions.Player.LightAttack.performed -= Attack1_performed;
        gameInputActions.Player.Interact.performed -= Interact_performed;
        gameInputActions.Player.Dash.performed -= Dash_performed;
        gameInputActions.Player.Ability1.performed -= Ability1_performed;
        gameInputActions.Player.Ability2.performed -= Ability2_performed;
        gameInputActions.Player.Ability3.performed -= Ability3_performed;
        
        // game states
        GameEventsManager.Instance.GameStateEvents.OnPauseToggle -= TogglePause;
        GameEventsManager.Instance.GameStateEvents.OnLoadToggle -= ToggleLoad;
        GameEventsManager.Instance.GameStateEvents.OnUIToggle -= ToggleUI;
        GameEventsManager.Instance.GameStateEvents.OnMenuToggle -= ToggleMenu;

        // dispose all maps
        gameInputActions.Dispose();
    }

    #region TOGGLES
    
    // toggles UI and player functions. called when inside pause menu.
    private void TogglePause(bool paused)
    {
        if (paused)
        {
            gameInputActions.UI.Disable();
            gameInputActions.Player.Disable();
        }
        else
        {
            gameInputActions.UI.Enable();
            gameInputActions.Player.Enable();
        }
    }

    // toggles all input. called when loading scenes
    private void ToggleLoad(bool finished)
    {
        if (finished)
        {
            gameInputActions.Player.Enable();
            gameInputActions.Menu.Enable();
            gameInputActions.UI.Enable();
        }
        else
        {
            gameInputActions.Player.Disable();
            gameInputActions.Menu.Disable();
            gameInputActions.UI.Disable();
        }
    }
    
    // toggles player and menu commands. called when in dialogues or UIs
    private void ToggleUI(bool isToggled)
    {
        if (isToggled)
        {
            gameInputActions.Player.Disable();
            gameInputActions.Menu.Disable();
        }
        else
        {
            gameInputActions.Player.Enable();
            gameInputActions.Menu.Enable();
        }
    }
    
    // toggles escape key only. Called in transition functions
    private void ToggleMenu(bool isToggled)
    {
        if (isToggled)
        {
            gameInputActions.Menu.Disable();
        }
        else
        {
            gameInputActions.Menu.Enable();
        }
    }

    
    // toggles the debug console. Disables all.
    private bool debug = false;
    private void OnToggleDebugConsole()
    {
        debug = !debug;
        OnDebugConsoleToggle?.Invoke();
        ToggleLoad(!debug);
    }
    #endregion

    #region INVOKE EVENTS
    
    private void ToggleDebugConsole(InputAction.CallbackContext obj) => OnToggleDebugConsole();
    
    private void ExecuteDebugConsole(InputAction.CallbackContext obj) => OnDebugConsoleExecute?.Invoke();
    
    private void PlayerMenu_performed(InputAction.CallbackContext obj) => OnPlayerMenuToggle?.Invoke();
    
    private void Pause_performed(InputAction.CallbackContext obj) => OnPauseToggle?.Invoke();

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

    public bool IsShiftModifierOn()
    {
        return gameInputActions.Player.ShiftModifier.IsPressed();
    }
    
    #endregion

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
                return gameInputActions.UI.Menu.bindings[0].ToDisplayString();
        }
    }

    public void RebindBinding(Bindings binding, Action onActionRebound)
    {
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
            .WithControlsExcluding("<Keyboard>/escape")
            .OnComplete(callback =>
            {
                callback.Dispose();
                onActionRebound();
            })
            .Start();
    }
}