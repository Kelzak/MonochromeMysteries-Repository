using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PowerBox : ItemAbs
{
    public GameObject player;

    public static bool switchesOn;

    public GameObject fuseObject;

    public Transform fuse1Position;
    public Transform fuse2Position;
    public Transform fuse3Position;

    public ParticleSystem sparks;

    public GameObject fusePic1;
    public GameObject fusePic2;
    public GameObject fusePic3;

    public AudioSource sparkingSFX;
    public AudioClip sparking;
    // Start is called before the first frame update
    void Start()
    {
        switchesOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        player = Player.possessedObj;

        if(Input.GetKeyDown(KeyCode.F1))
        {
            PowerBoxOn();
        }

    }

    public override void Activate()
    {
        if(player.GetComponent<Technician>())
        {
            if (Player.fuses.Count == 3 && !switchesOn)
            {
                PowerBoxOn();
            }
            else
            {
                Debug.Log("Need more fuses");
            }
        }
    }

    private void PowerBoxOn()
    {
        Debug.Log("Switches on!");
        //gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
        switchesOn = true;
        sparks.Play();
        sparkingSFX.Play();
        fusePic1.SetActive(false);
        fusePic2.SetActive(false);
        fusePic3.SetActive(false);
        Instantiate(fuseObject, fuse1Position.position, Quaternion.identity);
        Instantiate(fuseObject, fuse2Position.position, Quaternion.identity);
        Instantiate(fuseObject, fuse3Position.position, Quaternion.identity);
        Destroy(this);
        Destroy(gameObject.GetComponent<Item>());

    }

    public override void SetItemUI()
    {
        if (StateChecker.isGhost)
        {
            GetComponent<Item>().SetUI(null, null, null, "Spirit can't use", true);
        }
        else if(player.GetComponent<Technician>())
        {
            if(Player.fuses.Count == 3)
            {
                GetComponent<Item>().SetUI(null, null, null, "Press F to put in fuses", true);
            }
            else
            {
                GetComponent<Item>().SetUI(null, null, null, "Needs 3 Fuses", true);
            }
        }
        else
        {
            GetComponent<Item>().SetUI(null, null, null, "Needs Technician", true);
        }
    }
}

