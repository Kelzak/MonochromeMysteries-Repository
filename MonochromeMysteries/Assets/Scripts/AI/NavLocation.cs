/* Name: Player.cs
 * Author: Matt Kirchoff
 * Description: This script shows a location using gizmos for easier engine editing
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavLocation : MonoBehaviour
{
    public float gizmoSize = .25f;
    public Color gizmoColor = Color.blue;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(transform.position, gizmoSize);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, gizmoSize);
    }
}
