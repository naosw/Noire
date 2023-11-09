using UnityEngine;
using UnityEngine.Serialization;

public partial class Player
{
    [Header("Player Controller")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float gravity = .1f;
    [SerializeField] private float turnSpeed = 30f;
    [SerializeField] private LayerMask raycastHit;

    private PlayerState state;
    private CharacterController controller;
    private Vector3 moveDir;
    private readonly Vector3 raycastOffset = new(0, 1f, 0);
    private readonly float fallingThreshold = 1.3f;
    
    private readonly Quaternion rightRotation = Quaternion.Euler(new Vector3(0, 90, 0));
    
    // move towards `moveDir` with speed
    public void Move(float speed)
    {
        Vector3 velocity = speed * Time.deltaTime * moveDir;
        velocity.y = -gravity;
        controller.Move(velocity);

        float dist = Vector3.Distance(transform.forward, moveDir);
        if(dist > 1.3f)
            transform.forward = Vector3.Lerp(transform.forward, moveDir, 2 * turnSpeed * Time.deltaTime);
        else
            transform.forward = Vector3.Lerp(transform.forward, moveDir, turnSpeed * Time.deltaTime);
    }

    private void HandleFall()
    {
        if (Physics.Raycast(transform.position + raycastOffset, Vector3.down, out RaycastHit hit, Mathf.Infinity, raycastHit)) 
        {
            if (hit.distance > fallingThreshold)
            {
                state = PlayerState.Falling;
            }
            else
            {
                ResetStateAfterAction();
            }
        }
    }
    
    // called when player is either moving or idle
    private void HandleMovement()
    {
        Vector3 inputVector = GameInput.Instance.GetMovementVectorNormalized();
        if (inputVector == Vector3.zero)
        {
            Move(0); // handle fall
            state = PlayerState.Idle;
            return;
        }
        
        // calculates orthographic camera angle
        Vector3 forward = virtualCamera.transform.forward;
        forward.y = 0;
        Vector3 right = rightRotation * forward;
        forward *= inputVector.z;
        right *= inputVector.x;
        moveDir = (forward + right).normalized;
        
        // move
        if (!GameInput.Instance.IsShiftModifierOn())
        {
            state = PlayerState.Walking;
            Move(walkSpeed);
        }
        else
        {
            state = PlayerState.Running;
            Move(runSpeed);
        }
    }
}