using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private float interactDistance = 6f;
    
    public void Interact(){
        GetInteractableObject()?.Interact();
    }
    
    // returns a if a or b is closer to transform.position
    private IInteractable MinInteractable(IInteractable a, IInteractable b)
    {
        var d1 = Vector3.Distance(a.GetTransform().position, transform.position);
        var d2 = Vector3.Distance(b.GetTransform().position, transform.position);
        return d1 < d2 ? a : b;
    }
    
    public IInteractable GetInteractableObject()
    {
        List<IInteractable> interactableList = new List<IInteractable>();
        
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactDistance);
        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent(out IInteractable interactable))
            {
                interactableList.Add(interactable);
            }
        }

        if (interactableList.Count == 0)
            return null;
        
        IInteractable closestInteractable = interactableList.Aggregate(MinInteractable);
        
        return closestInteractable;
    }
}
