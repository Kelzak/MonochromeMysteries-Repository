using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorSounds : MonoBehaviour
{

    //set audio in the audio source, set on loop and turn off play on awake


    private AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(PowerSwitch.stationPowerOn)
        {
            source.Play();
        }
    }
}
