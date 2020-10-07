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
    public int maxFlickers = 4;
    // Start is called before the first frame update
    void Start()
    {
        maxFlickers = 5;
        minWaitTime = .5f;
        maxWaitTime = 5f;
        light = GetComponent<Light>();
        StartCoroutine(Flashing());
    }

    IEnumerator Flashing()
    {
        while(true)
        {
            yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
            for(int i = 0 ; i < Random.Range(1, maxFlickers); i++)
            {
                toggle = !toggle;
                light.enabled = toggle;
                if (GetComponentInParent<MeshRenderer>())
                {
                    if (toggle)
                    {
                        GetComponentInParent<MeshRenderer>().material.EnableKeyword("_EMISSION");

                    }
                    else
                    {
                        GetComponentInParent<MeshRenderer>().material.DisableKeyword("_EMISSION");
                    }
                }
                yield return new WaitForSeconds(.1f);

            }
            
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
