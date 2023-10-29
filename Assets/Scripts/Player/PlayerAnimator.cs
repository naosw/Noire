using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private ParticleSystemBase inkSlash;
    [SerializeField] private Vector3 inkSlashOffset = new(0f, 2.3f, 0f);

    private Animator animator;
    private const string WALK = "PlayerWalk";
    private const string IDLE = "PlayerIdle";
    private const string FALL = "PlayerFall";
    private int WALK_ID;
    private int IDLE_ID;
    private int FALL_ID;
    
    public static PlayerAnimator Instance { get; private set; }
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        WALK_ID  = Animator.StringToHash(WALK);
        IDLE_ID =  Animator.StringToHash(IDLE);
        FALL_ID =  Animator.StringToHash(FALL);
    }

    private void Update()
    {
        animator.SetBool(WALK_ID, Player.Instance.IsWalking());
        animator.SetBool(IDLE_ID, Player.Instance.IsIdle());
        animator.SetBool(FALL_ID, Player.Instance.IsFalling());
    }

    public bool AnimatorIsPlaying(int layer)
    {
        return animator.GetCurrentAnimatorStateInfo(layer).length > animator.GetCurrentAnimatorStateInfo(layer).normalizedTime;
    }

    public bool AnimatorIsPlaying(int layer, string stateName)
    {
        return AnimatorIsPlaying(layer) && animator.GetCurrentAnimatorStateInfo(layer).IsName(stateName);
    }

    private void AnimationStartedTrigger()
    {
        var playerTransform = Player.Instance.transform;
        
        inkSlash.transform.position = playerTransform.position + inkSlashOffset;
        inkSlash.transform.rotation = Quaternion.Euler(new Vector3(0, playerTransform.rotation.eulerAngles.y - 180, 0));
        inkSlash.Restart();
    }
}
