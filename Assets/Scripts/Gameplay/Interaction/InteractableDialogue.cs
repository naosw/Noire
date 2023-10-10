using UnityEngine;
using Ink.Runtime;

public class InteractableDialogue : InteractableObject
{
    [SerializeField] private TextAsset inkJSONAsset = null;
    public override void Interact()
    {
        if (CanInteract())
        {
            interactionsOccured++;
            var story = new Story(inkJSONAsset.text);
            DialogueUI.Instance.StartDialogue(story);
        }
    }
}