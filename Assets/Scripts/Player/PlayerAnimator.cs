using System;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
    private const string WALK = "PlayerWalk";
    private const string IDLE = "PlayerIdle";
    public event EventHandler OnAttackAnimationStarted;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        animator.SetBool(WALK, Player.Instance.IsWalking());
        animator.SetBool(IDLE, Player.Instance.IsIdle());
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
        OnAttackAnimationStarted?.Invoke(this, EventArgs.Empty);
    }
}
