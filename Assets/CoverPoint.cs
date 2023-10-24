using UnityEngine;

public class CoverPoint : MonoBehaviour
{
    public bool IsCoverSafe(Vector3 threatPosition, LayerMask IgnoreLayers, GameObject Threat)
    {
        RaycastHit hit;
        if (Physics.Linecast(transform.position, threatPosition , out hit, ~IgnoreLayers))
        {
            Debug.Log(hit.collider.name);
            return hit.collider.gameObject != Threat;
        }
        return false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red; // Set gizmo color to blue
        Gizmos.DrawSphere(transform.position, 0.5f); // Draw a sphere at the cover point's position
    }
}