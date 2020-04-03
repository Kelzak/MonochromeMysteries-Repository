using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class onlyGhostSees : MonoBehaviour
{

    public bool onlyGhostSee;

    private MeshRenderer renderer;

    private Material mat;
    // Start is called before the first frame update
    void Start()
    {
        mat = gameObject.GetComponent<MeshRenderer>().material;
        renderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!StateChecker.isGhost)
        {
            if(onlyGhostSee)
                renderer.enabled = false;
        }
        else
        {
            renderer.enabled = true;
        }
    }
}