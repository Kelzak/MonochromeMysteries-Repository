using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;
public class BombingTrigger : MonoBehaviour
{
    public float fadeInTime = .1f;
    public float fadeOutTime = 5f;
    public float bombIntensity = 5f;

    public int repeatTime;
    public int repeatPercentage;
    public int fallingParticlePercentage;
    public int lightsPercentage;

    public AudioClip[] bombClips;
    private AudioClip clip;
    public AudioSource audioSource;

    public ParticleSystem[] particles;

    public Light[] regularLights;
    public Light[] emergencyLights;

    private bool toggle;
    public float minWaitTime;
    public float maxWaitTime;
    public int maxFlickers = 4;


    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("TriggerBomb", repeatTime, repeatTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //triggers bomb screen shake, particle systems, and sound
    void TriggerBomb()
    {
        //if percentage is right
        int temp = Random.Range(0, 100);
        if (temp < repeatPercentage)
        {
            print("bomb");

            //screen shake
            CameraShaker.Instance.ShakeOnce(bombIntensity, bombIntensity, fadeInTime, fadeOutTime);


            //particles
            foreach (ParticleSystem particle in particles)
            {
                temp = Random.Range(0, 100);
                if (temp < fallingParticlePercentage)
                {
                    particle.Play();
                }
            }


            //lights
            StartCoroutine(Flashing());
            Invoke("CancelCoroutine", fadeOutTime / 2);

            //sound
            temp = Random.Range(0, bombClips.Length);
            clip = bombClips[temp];
            audioSource.clip = clip;
            audioSource.Play();
        }

    }

    void CancelCoroutine()
    {
        StopAllCoroutines();
        //after flicker, reset to normal
        if (PowerSwitch.stationPowerOn)
        {
            foreach (Light light in regularLights)
            {
                light.enabled = true;

            }
        }
        else
        {
            foreach (Light light in emergencyLights)
            {
                light.enabled = true;
            }
        }
    }

    IEnumerator Flashing()
    {
        while (true)
        {
            for (int i = 0; i < Random.Range(1, maxFlickers); i++)
            {
                toggle = !toggle;

                //regular lights
                if (PowerSwitch.stationPowerOn)
                {
                    foreach (Light light in regularLights)
                    {
                        int temp = Random.Range(0, 100);
                        if (temp < fallingParticlePercentage)
                            light.enabled = toggle;

                    }
                }
                else
                {
                    foreach (Light light in emergencyLights)
                    {
                        int temp = Random.Range(0, 100);
                        if (temp < fallingParticlePercentage)
                            light.enabled = toggle;

                    }
                }

                //GetComponent<Light>().enabled = toggle;
                if (GetComponentInParent<MeshRenderer>())
                {
                    if (toggle)
                    {
                        //GetComponentInParent<MeshRenderer>().material.EnableKeyword("_EMISSION");

                    }
                    else
                    {
                        //GetComponentInParent<MeshRenderer>().material.DisableKeyword("_EMISSION");
                        //audioSource.Play();
                    }
                }
                yield return new WaitForSeconds(.1f);

            }

            
        }
    }
}
