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
    private GameObject camera;
    private Vector3 target;
    private int index = 0;
    // Start is called before the first frame update
    void Start()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera");
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        target = camera.transform.position;
        //Debug.Log(target);
        for (int i = 0; i < musicBoxes.Length; i++)
        {
            dist = Vector3.Distance(target, musicBoxes[index].transform.position);
            if (dist > Vector3.Distance(target, musicBoxes[i].transform.position))
            {
                index = i;
            }
        }
        for (int i = 0; i < musicBoxes.Length; i++)
        {
            if(i == index)
            {
                audioSource = musicBoxes[i].GetComponent<AudioSource>();
                audioSource.volume = Mathf.Lerp(0, musicVolume, 10f);
            }
            else
            {
                audioSource = musicBoxes[i].GetComponent<AudioSource>();
                audioSource.volume = Mathf.Lerp(musicVolume, 0, 10f);
            }
            
        }
        

    }
}
