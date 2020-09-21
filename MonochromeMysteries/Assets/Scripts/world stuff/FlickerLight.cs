/* Name: Player.cs
 * Author: Matt Kirchoff
 * Description: This script handles flickering lights in game
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickerLight : MonoBehaviour
{
    private Light light;
    private bool toggle;
    public float minWaitTime;
    public float maxWaitTime;
    // Start is called before the first frame update
    void Start()
    {
        minWaitTime = .2f;
        maxWaitTime = 5f;
        light = GetComponent<Light>();
        StartCoroutine(Flashing());
    }

    IEnumerator Flashing()
    {
        while(true)
        {
            yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
            toggle = !toggle;
            light.enabled = toggle;
            if(GetComponentInParent<MeshRenderer>())
            {
                if(toggle)
                {
                    GetComponentInParent<MeshRenderer>().material.EnableKeyword("_EMISSION");

                }
                else
                {
                    GetComponentInParent<MeshRenderer>().material.DisableKeyword("_EMISSION");
                }
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
