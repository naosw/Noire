using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    public event EventHandler<OnCameraTurnEventArgs> OnCameraTurn;
    public event EventHandler OnPauseAction;

    public enum Bindings
    {
        Move_Up,
        Move_Down,
        Move_Left,
        Move_Right,
        Attack,
        Camera_Left,
        Camera_Right,
    }
    public class OnCameraTurnEventArgs : EventArgs
    {
        // -1 for right. 1 for left.
        public int turnDir;
    }

    public event EventHandler OnAttack1;

    private GameInputActions gameInputActions;

    private void Awake()
    {
        Instance = this;
        gameInputActions = new GameInputActions();

    }

    private void Start()
    {
        gameInputActions.Player.Enable();

        gameInputActions.Player.CameraRight.performed += CameraRight_performed;
        gameInputActions.Player.CameraLeft.performed += CameraLeft_performed;
        gameInputActions.Player.Attack1.performed += Attack1_performed;
        gameInputActions.Player.Pause.performed += Pause_performed;
    }

    private void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) { OnPauseAction?.Invoke(this, EventArgs.Empty); }

    private void Attack1_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) { OnAttack1?.Invoke(this, EventArgs.Empty); }

    private void CameraLeft_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) { OnCameraTurn?.Invoke(this, new OnCameraTurnEventArgs { turnDir = 1 }); }
    private void CameraRight_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) { OnCameraTurn?.Invoke(this, new OnCameraTurnEventArgs { turnDir = -1 }); }

    private void OnDestroy()
    {
        gameInputActions.Player.CameraLeft.performed -= CameraLeft_performed;
        gameInputActions.Player.CameraRight.performed -= CameraRight_performed;
        gameInputActions.Player.Attack1.performed -= Attack1_performed;
        gameInputActions.Player.Pause.performed -= Pause_performed;
        gameInputActions.Dispose();
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