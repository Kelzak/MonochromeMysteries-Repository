/* Name: Player.cs
 * Author: Matt Kirchoff
 * Description: This script handles what music the player hears
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicListener : MonoBehaviour
{
    private AudioSource audioSource;

    [Range(0.0f, 1.0f)]
    public float musicVolume = .5f;

    private float dist;

    List<GameObject> soundObjects = null;


    private MusicBox[] musicBoxes;

    private GameObject temp;
    private GameObject player;
    private int index = 0;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        player = Player.possessedObj;

        //find and add all music boxes
        musicBoxes = FindObjectsOfType<MusicBox>();
        foreach(MusicBox musicBox in musicBoxes)
        {
            soundObjects.Add(musicBox.gameObject);
        }

    }

    // Update is called once per frame
    void Update()
    {
        player = Player.possessedObj;
        if(soundObjects != null)
        {
            for (int i = 0; i < soundObjects.Count; i++)
            {
                dist = Vector3.Distance(player.transform.position, soundObjects[index].transform.position);
                if (dist > Vector3.Distance(player.transform.position, soundObjects[i].transform.position))
                {
                    index = i;
                }
            }
            for (int i = 0; i < soundObjects.Count; i++)
            {
                if (i == index)
                {
                    audioSource = soundObjects[i].GetComponentInChildren<AudioSource>();
                    audioSource.volume = Mathf.Lerp(audioSource.volume, musicVolume, .5f * Time.deltaTime);
                }
                else
                {
                    audioSource = soundObjects[i].GetComponentInChildren<AudioSource>();
                    audioSource.volume = Mathf.Lerp(audioSource.volume, 0f, .5f * Time.deltaTime);
                }

            }
        }
        


    }
}