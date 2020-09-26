using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swing_Light : MonoBehaviour
{
    public Transform hingePoint;
    public Transform bulbPoint;
    public LineRenderer lineRenderer;
    public HingeJoint hingeJoint;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        hingeJoint.anchor = hingePoint.position;

        //Render Line
        lineRenderer.SetPosition(0, hingePoint.position);
        lineRenderer.SetPosition(1, bulbPoint.position);
    }
}
