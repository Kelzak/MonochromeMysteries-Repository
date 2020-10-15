using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindAudio : MonoBehaviour
{
    AudioSource[] audioSources;
    // Start is called before the first frame update
    void Start()
    {
        audioSources = FindObjectsOfType<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {

        foreach (AudioSource audioSource in audioSources)
        {
            try
            {
                //Debug.Log(audioSource.gameObject.name + ": audio clip: " + audioSource.clip.name);

            }
            catch
            {
            }
        }
    }
}
