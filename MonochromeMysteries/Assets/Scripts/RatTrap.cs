//Made by matt kirchoff

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatTrap : MonoBehaviour
{ 
    public AudioClip disableTrap;
    private AudioSource audioSource;

    private AudioClip sound;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Activate()
    {
        AudioClip sound = disableTrap;
        audioSource.PlayOneShot(sound);

        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerEnter(Collider other)
    {
        
    }

    void OnTriggerExit(Collider other)
    {
        
    }
}
