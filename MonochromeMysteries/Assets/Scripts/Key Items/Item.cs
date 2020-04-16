/*
 * Created by Matt Kirchoff
 * item script for outlining during ghost vision
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//observer to state checker to display outline on item
[RequireComponent(typeof(Outline))]
public class Item : MonoBehaviour, IObserver
{
    public StateChecker stateChecker;
    GameObject item;
    private float dist;
    public float playerDistGlow;
    private GameObject player;
    //public Image reticle;

    private bool isGhost;
    private bool isClose;

    public string itemName;
    private GameObject checker;

    public bool isPickup;

    public bool glowOverride;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        item = this.gameObject;
        checker = GameObject.FindGameObjectWithTag("StateChecker");
        stateChecker = checker.GetComponent<StateChecker>();
        // adds this object to observer list in statechecker
        stateChecker.RegisterObserver(this);

        playerDistGlow = Player.reticleDist;
    }

    // Update is called once per frame
    void Update()
    {
        //changes materials 
        if (isGhost == true && isClose == true || glowOverride)
        {
            item.GetComponent<Outline>().enabled = true;
            //reticle.color = new Color(255.0f, 255.0f, 0.0f);
        }
        else
        {
            //reticle.color = new Color(0.0f, 255.0f, 255.0f);
            item.GetComponent<Outline>().enabled = false;
        }



        dist = Vector3.Distance(this.transform.position, player.transform.position);
        //Debug.Log("Dist: " + dist);
        if (dist < playerDistGlow)
        {
            isClose = true;

        }
        else
        {
            isClose = false;
        }
    }
    //gets data sent fron statechecker
    public void UpdateData(bool check)
    {
        isGhost = check;
    }

    private void OnTriggerStay(Collider other)
    {
        if (Input.GetButton("PickUp") && other.gameObject.tag == "Rat")
        {
            item.GetComponent<Outline>().enabled = false;
        }
        if (Input.GetButtonUp("PickUp") && other.gameObject.tag == "Rat")
        {
            item.GetComponent<Outline>().enabled = true;
        }

    }
}

