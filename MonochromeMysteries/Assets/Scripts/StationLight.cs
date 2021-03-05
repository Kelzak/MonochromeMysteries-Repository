using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationLight : MonoBehaviour
{
    [SerializeField]
    private Light light;

    private bool stationPower = false;
    public bool emergencyLight = false;
    // Start is called before the first frame update
    void Start()
    {
        light = this.GetComponent<Light>();
        light.enabled = stationPower;
        light.enabled = emergencyLight;
    }

    // Update is called once per frame
    void Update()
    {
        if (PowerSwitch.stationPowerOn)
        {
            stationPower = true;
            ActivateLight();
        }
    }

    void ActivateLight()
    {
        if(emergencyLight)
        {
            light.enabled = false;
        }
        else
        {
            light.enabled = true;
        }
    }
}
