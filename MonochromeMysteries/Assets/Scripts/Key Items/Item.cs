/*
 * Created by Matt Kirchoff
 * item script for outlining during ghost vision
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//observer to state checker to display outline on item
public class Item : MonoBehaviour, IObserver
{
    public StateChecker stateChecker;
    GameObject item;
    private float dist;
    public float playerDistGlow;
    private GameObject player;


    private bool isGhost;
    private bool isClose;

    //gets data sent fron statechecker
    public void UpdateData(bool check)
    {
        isGhost = check;
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        playerDistGlow = 20f;

        item = this.gameObject;

        // adds this object to observer list in statechecker
        stateChecker.RegisterObserver(this);
    }

    // Update is called once per frame
    void Update()
    {
        //changes materials 
        if(isGhost == true && isClose == true)
        {
            item.GetComponent<Outline>().enabled = true;
        }
        else
            item.GetComponent<Outline>().enabled = false;


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
}


