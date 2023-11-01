using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootMarker : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 0.5f);  // Draws a blue sphere with a radius of 0.5 units
    }
}
