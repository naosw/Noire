using UnityEngine;

public class PatrolPoint : MonoBehaviour
{
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue; // Set gizmo color to blue
        Gizmos.DrawSphere(transform.position, 0.5f); // Draw a sphere at the cover point's position
    }
}