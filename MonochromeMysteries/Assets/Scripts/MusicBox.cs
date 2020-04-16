//Made by matt kirchoff

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicBox : MonoBehaviour
{
    private bool _isInsideTrigger = false;

    public AudioClip[] music;
    public AudioSource MusicSource;

    void Start()
    {
        MusicSource = this.gameObject.GetComponent<AudioSource>();
        PlayMusic(music[0]);
    }
    // Update is called once per frame
    void Update()
    {
        if (_isInsideTrigger)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Skip();
            }
        }
    }

    void PlayMusic(AudioClip clip)
    {
        CancelInvoke();
        MusicSource.Stop();
        MusicSource.clip = clip;
        MusicSource.Play();
        Invoke("Skip", MusicSource.clip.length);
    }

    void Skip()
    {
        for (int i = 0; i < music.Length; i++)
        {
            if (i >= music.Length)
                i = 0;
            PlayMusic(music[i]);
        }
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
}
