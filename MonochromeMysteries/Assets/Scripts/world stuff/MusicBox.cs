/* Name: Player.cs
 * Author: Matt Kirchoff
 * Description: This script handles music boxes / radio
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicBox : ItemAbs
{
    private bool _isInsideTrigger = false;

    public AudioClip[] music;
    public AudioSource MusicSource;

    public Player player;

    private int index;

    //used to play first clip of music box, on the box in the tutorial room
    public bool playFirst = false;

    void Start()
    {
        MusicSource = this.gameObject.GetComponent<AudioSource>();

        if(playFirst)
        {
            PlayMusic(music[0]);
            index = 0;
        }
        else
        {
            int rand = Random.Range(0, music.Length);
            PlayMusic(music[rand]);
            index = rand;
        }
        
    }
    // Update is called once per frame
    void Update()
    {
        player = FindObjectOfType<Player>();

        if (_isInsideTrigger)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                //Skip();
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

    public void Skip()
    {

        if (index >= music.Length - 1)
            index = 0;
        else
            index++;

        //Debug.Log("Skip index = " + index);
        PlayMusic(music[index]);

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

    public override void Activate()
    {
        Skip();
    }

    public override void SetItemUI()
    {
        if (!player.GetComponent<Rat>() && !StateChecker.isGhost)
        {
            GetComponent<Item>().SetUI(null, null, null, "Press F to Skip", true);
        }
        else
        {
            GetComponent<Item>().SetUI(null, null, null, "", true);

        }
    }
}
