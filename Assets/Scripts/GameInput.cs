using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }
    
    // list of events
    public event EventHandler<OnCameraTurnEventArgs> OnCameraTurn;
    public class OnCameraTurnEventArgs : EventArgs
    {
        public int turnDir; // -1 for right. 1 for left.
    }
    public event UnityAction OnPauseAction;
    public event UnityAction OnInteract;
    public event EventHandler<OnAbilityCastArgs> OnAbilityCast;
    public class OnAbilityCastArgs : EventArgs
    {
        public int abilityID;
    }
    
    public enum Bindings
    {
        Move_Up,
        Move_Down,
        Move_Left,
        Move_Right,
        Attack,
        Camera_Left,
        Camera_Right,
        // TODO: add dash keybindings
        // TODO: add interact keybindings
        // TODO: add ability keybindings
    }

    private GameInputActions gameInputActions;

    private void Awake()
    {
        Instance = this;
        gameInputActions = new GameInputActions();
    }
    
    // subscribe listeners
    private void Start()
    {
        gameInputActions.Player.Enable();
        gameInputActions.Player.CameraRight.performed += CameraRight_performed;
        gameInputActions.Player.CameraLeft.performed += CameraLeft_performed;
        gameInputActions.Player.Attack1.performed += Attack1_performed;
        gameInputActions.Player.Pause.performed += Pause_performed;
        gameInputActions.Player.Interact.performed += Interact_performed;
        gameInputActions.Player.Dash.performed += Dash_performed;
        gameInputActions.Player.Ability1.performed += Ability1_performed;
        gameInputActions.Player.Ability2.performed += Ability2_performed;
        gameInputActions.Player.Ability3.performed += Ability3_performed;
    }
    
    private void OnDisable()
    {
        gameInputActions.Player.CameraRight.performed -= CameraRight_performed;
        gameInputActions.Player.CameraLeft.performed -= CameraLeft_performed;
        gameInputActions.Player.Attack1.performed -= Attack1_performed;
        gameInputActions.Player.Pause.performed -= Pause_performed;
        gameInputActions.Player.Interact.performed -= Interact_performed;
        gameInputActions.Player.Dash.performed -= Dash_performed;
        gameInputActions.Player.Ability1.performed -= Ability1_performed;
        gameInputActions.Player.Ability2.performed -= Ability2_performed;
        gameInputActions.Player.Ability3.performed -= Ability3_performed;
        
        gameInputActions.Dispose();
    }

    // invoke events
    private void Pause_performed(InputAction.CallbackContext obj) => OnPauseAction?.Invoke();

    private void CameraLeft_performed(InputAction.CallbackContext obj)
    {
        OnCameraTurn?.Invoke(this, new OnCameraTurnEventArgs { turnDir = 1 });
    }

    private void CameraRight_performed(InputAction.CallbackContext obj)
    {
        OnCameraTurn?.Invoke(this, new OnCameraTurnEventArgs { turnDir = -1 });
    }
    
    private void Interact_performed(InputAction.CallbackContext obj) => OnInteract?.Invoke();
    
    private void Attack1_performed(InputAction.CallbackContext obj)
    {
        OnAbilityCast?.Invoke(this, new OnAbilityCastArgs { abilityID = 0 });
    }
    
    private void Dash_performed(InputAction.CallbackContext obj)
    {
        OnAbilityCast?.Invoke(this, new OnAbilityCastArgs { abilityID = 1 });
    }

    private void Ability1_performed(InputAction.CallbackContext obj)
    {
        OnAbilityCast?.Invoke(this, new OnAbilityCastArgs { abilityID = 2 });
    }
    
    private void Ability2_performed(InputAction.CallbackContext obj)
    {
        OnAbilityCast?.Invoke(this, new OnAbilityCastArgs{ abilityID = 3 });
    }
    
    private void Ability3_performed(InputAction.CallbackContext obj)
    {
        OnAbilityCast?.Invoke(this, new OnAbilityCastArgs{ abilityID = 4 });
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
            case Bindings.Move_Up:
                return gameInputActions.Player.Move.bindings[1].ToDisplayString();
            case Bindings.Move_Down:
                return gameInputActions.Player.Move.bindings[2].ToDisplayString();
            case Bindings.Move_Left:
                return gameInputActions.Player.Move.bindings[3].ToDisplayString();
            case Bindings.Move_Right:
                return gameInputActions.Player.Move.bindings[4].ToDisplayString();
            case Bindings.Attack:
                return gameInputActions.Player.Attack1.bindings[0].ToDisplayString ();
            case Bindings.Camera_Left:
                return gameInputActions.Player.CameraLeft.bindings[0].ToDisplayString();
            case Bindings.Camera_Right:
                return gameInputActions.Player.CameraRight.bindings[0].ToDisplayString();
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
            case Bindings.Move_Up:
                inputAction = gameInputActions.Player.Move;
                bindingIndex = 1;
                break;
            case Bindings.Move_Down:
                inputAction = gameInputActions.Player.Move;
                bindingIndex = 2;
                break;
            case Bindings.Move_Left:
                inputAction = gameInputActions.Player.Move;
                bindingIndex = 3;
                break;
            case Bindings.Move_Right:
                inputAction = gameInputActions.Player.Move;
                bindingIndex = 4;
                break;
            case Bindings.Attack:
                inputAction = gameInputActions.Player.Attack1;
                bindingIndex = 0;
                break;
            case Bindings.Camera_Left:
                inputAction = gameInputActions.Player.CameraLeft;
                bindingIndex = 0;
                break;
            case Bindings.Camera_Right:
                inputAction = gameInputActions.Player.CameraRight;
                bindingIndex = 0;
                break;
        }

        inputAction.PerformInteractiveRebinding(bindingIndex)
            .OnComplete(callback =>
            {
                callback.Dispose();
                gameInputActions.Player.Enable();
                onActionRebound();
            })
            .Start();
    }
}