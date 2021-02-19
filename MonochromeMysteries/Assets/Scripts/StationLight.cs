using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationLight : MonoBehaviour
{
    [SerializeField]
    private Light light;

    private bool stationPower = false;
    // Start is called before the first frame update
    void Start()
    {
        light = this.GetComponent<Light>();
        light.enabled = stationPower;
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
        light.enabled = true;
    }
}
