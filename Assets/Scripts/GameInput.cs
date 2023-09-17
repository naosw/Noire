using System;
using Unity.VisualScripting;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    public event EventHandler<OnCameraTurnEventArgs> OnCameraTurn;
    public event EventHandler OnPauseAction;

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
    }

    private void Start()
    {
        gameInputActions = new GameInputActions();
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
}