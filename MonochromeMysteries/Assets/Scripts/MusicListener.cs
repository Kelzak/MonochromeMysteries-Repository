//made by matt kirchoff

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicListener : MonoBehaviour
{
    private AudioSource audioSource;

    [Range(0.0f, 1.0f)]
    public float musicVolume = .5f;

    private float dist;

    public GameObject[] musicBoxes;

    private GameObject temp;
    private GameObject player;
    private int index = 0;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        player = GameObject.FindObjectOfType<Player>().gameObject;
        for (int i = 0; i < musicBoxes.Length; i++)
        {
            dist = Vector3.Distance(player.transform.position, musicBoxes[index].transform.position);
            if (dist > Vector3.Distance(player.transform.position, musicBoxes[i].transform.position))
            {
                index = i;
            }
        }
        for (int i = 0; i < musicBoxes.Length; i++)
        {
            if (i == index)
            {
                audioSource = musicBoxes[i].GetComponentInChildren<AudioSource>();
                audioSource.volume = Mathf.Lerp(0, musicVolume, 1f);
            }
            else
            {
                audioSource = musicBoxes[i].GetComponentInChildren<AudioSource>();
                audioSource.volume = Mathf.Lerp(musicVolume, 0, 1f);
            }

        }


    }
}