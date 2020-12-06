/* Name: Player.cs
 * Author: Matt Kirchoff
 * Description: This script handles light switches
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : ItemAbs
{
    private bool _isInsideTrigger = false;

    public AudioClip[] sounds;
    public AudioSource audioSource;
    public Light[] lights;
    public bool off;
    public GameObject bulb;
    public Player player;

    void Start()
    {
        player = FindObjectOfType<Player>();

        audioSource = this.gameObject.GetComponent<AudioSource>();

        if (lights.Length == 0)
        {
            lights = GetComponents<Light>();

            try
            {
                 bulb = GameObject.Find("bulb");
            }           
            catch
            {
            }

        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Activate()
    {
        if(!StateChecker.isGhost && !player.GetComponent<Rat>())
        {
            if (!player.GetComponent<Rat>() && !StateChecker.isGhost)
            {
                for (int i = 0; i < lights.Length; i++)
                {
                    if (off)
                    {
                        if (lights[i].gameObject.GetComponentInParent<CeilingFan>())
                        {
                            lights[i].gameObject.GetComponentInParent<CeilingFan>().Activate();

                            if (bulb != null)
                                bulb.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
                            //bulb.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.yellow);
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

                            if (bulb != null)
                                bulb.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
                            //bulb.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.black);
                        }
                    }
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

    public override void SetItemUI()
    {
        if (!player.GetComponent<Rat>() && !StateChecker.isGhost)
        {
            GetComponent<Item>().dontChangeReticleColor = false;

            if (off)
            {
                GetComponent<Item>().SetUI(null, null, null, "Press F to turn On", true);

            }
            else
            {
                GetComponent<Item>().SetUI(null, null, null, "Press F to turn Off", true);
            }
        }
        else
        {
            GetComponent<Item>().SetUI(null, null, null, "", true);
            GetComponent<Item>().dontChangeReticleColor = true;
        }

    }
}
