/* Name: Player.cs
 * Author: Matt Kirchoff
 * Description: This script handles the ghosts spot light
 */

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

    private void OnEnable()
    {
        MainMenu.OnMainMenuTriggered += HideLight;
    }

    private void OnDisable()
    {
        MainMenu.OnMainMenuTriggered -= HideLight;
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

    private void HideLight(bool shouldHide)
    {
        if (shouldHide)
            gameObject.SetActive(false);
        else
            gameObject.SetActive(true);
    }

}
