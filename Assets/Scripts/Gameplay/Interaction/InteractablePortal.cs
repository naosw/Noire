using System.Collections;
using UnityEngine;

/// <summary>
/// loads target scene upon interaction
/// </summary>

public class InteractablePortal : InteractableObject
{
    [SerializeField] private GameScene destinationScene;
    [SerializeField] private Animator doorAnimator;
    private string DOOR_OPEN = "DoorOpen";
    
    public override void Interact()
    {
        StartCoroutine(NextScene());
    }

    IEnumerator NextScene()
    {
        doorAnimator.SetTrigger(DOOR_OPEN);
        yield return new WaitForSeconds(1f);
        Loader.Load(destinationScene);
    }
    
    public override bool CanInteract()
    {
        return !disabled;
    }
}