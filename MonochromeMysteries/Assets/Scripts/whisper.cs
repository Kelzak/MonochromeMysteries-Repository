using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class whisper : MonoBehaviour
{
    public AudioClip[] whispers;
    private AudioSource audioSource;

    public float whisperInterval;

    [Range(0f,1f)]
    public float whisperVolume = .5f;
    private int rand;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        InvokeRepeating("Speak", 10f, whisperInterval);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Speak()
    {
        if(StateChecker.isGhost)
        {
            rand = Random.Range(0, whispers.Length);
            audioSource.PlayOneShot(whispers[rand]);
        }
        
    }
}
