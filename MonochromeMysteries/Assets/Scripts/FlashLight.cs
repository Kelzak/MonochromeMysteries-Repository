using UnityEngine;
using System.Collections;

public class FlashLight : MonoBehaviour
{
    private bool on;
    public Light light;
    // Use this for initialization
    void Start()
    {
        light = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        if (StateChecker.isGhost)
        {
            light.enabled = true;
        }
        else
        {
            light.enabled = false;
        }
    }
}
