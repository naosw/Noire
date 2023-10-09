using UnityEngine;
using UnityEngine.Serialization;

public class Duck : InteractableObject
{
    [SerializeField] private ParticleSystemBase firefliesBurst;
    public override void Interact()
    {
        if (CanInteract())
        {
            interactionsOccured++;
            firefliesBurst.Play();
        }
    }
}