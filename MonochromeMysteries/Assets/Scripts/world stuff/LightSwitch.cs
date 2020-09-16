/* Name: Player.cs
 * Author: Matt Kirchoff
 * Description: This script handles light switches
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    private bool _isInsideTrigger = false;

    public AudioClip[] sounds;
    public AudioSource audioSource;
    public Light[] lights;
    public bool off;

    void Start()
    {
        audioSource = this.gameObject.GetComponent<AudioSource>();
        if (lights == null)
        {
            lights = GetComponents<Light>();

        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate()
    {
        for (int i = 0; i < lights.Length; i++)
        {
            if (off)
            {
                if (lights[i].gameObject.GetComponentInParent<CeilingFan>())
                {
                    lights[i].gameObject.GetComponentInParent<CeilingFan>().Activate();
                    GameObject bulb = GameObject.Find("bulb");

                    bulb.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
                }  
                lights[i].enabled = true;
                off = false;
                PlaySound(sounds[0]);
            }
            else
            {
                lights[i].enabled = false;
                off = true;
                PlaySound(sounds[1]);
                if (lights[i].gameObject.GetComponentInParent<CeilingFan>())
                {
                    lights[i].gameObject.GetComponentInParent<CeilingFan>().Activate();
                    GameObject bulb = GameObject.Find("bulb");

                    bulb.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
                }
            }
        }
    }

    void PlaySound(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Person")
        {
            if (other.GetComponent<Player>())
            {
                _isInsideTrigger = true;
                //OpenPanel.SetActive(true);
            }

        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Person")
        {
            _isInsideTrigger = false;
            //OpenPanel.SetActive(false);
        }
    }
}
