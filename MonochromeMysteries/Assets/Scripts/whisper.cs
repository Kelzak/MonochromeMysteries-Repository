/* Name: Player.cs
 * Author: Matt Kirchoff
 * Description: This script plays whispers every so often in ghost form
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class whisper : MonoBehaviour
{
    public AudioClip[] whispers;
    private AudioSource audioSource;

    public float whisperInterval = 30f;

    [Range(0f,1f)]
    public float whisperVolume = .5f;
    private int rand;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("whisper script object: " + gameObject.name);
        audioSource = GetComponent<AudioSource>();
        Invoke("Speak", 10f);
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
        Invoke("Speak", Random.Range(whisperInterval / 2, whisperInterval * 2));
    }
}
