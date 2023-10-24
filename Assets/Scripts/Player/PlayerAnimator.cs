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
    
    public static PlayerAnimator Instance { get; private set; }
    
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
        inkSlash.transform.position = Player.Instance.transform.position + inkSlashOffset;
        
        var playerRot = Player.Instance.transform.rotation.eulerAngles;
        inkSlash.transform.rotation = Quaternion.Euler(new Vector3(0, playerRot.y - 180, 0));
        inkSlash.Restart();
    }
}
