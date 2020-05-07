/* Name: Player.cs
 * Author: Matt Kirchoff
 * Description: This script handles light material flickering, but is now obsolete
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LightMatFlicker : MonoBehaviour
{
    private Light light;
    private Material material;
	public float minWaitTime;
    public float maxWaitTime;

    private bool toggle;

    void Start()
    {
        light = GetComponentInChildren<Light>();
        material = GetComponent<Material>();
        material.SetColor("_EmmisionColor", Color.yellow);
        StartCoroutine(Flashing());
    }

    IEnumerator Flashing()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
            light.enabled = !light.enabled;
            if (toggle)
            {
                light.enabled = true;
                material.SetColor("_EmissionColor", Color.yellow);

                toggle = false;
            }
            else
            {
                light.enabled = false;
                material.SetColor("_EmissionColor", Color.black);

                toggle = true;
            }

        }
    }


}
