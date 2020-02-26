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

    private bool isGhost;

    //gets data sent fron statechecker
    public void UpdateData(bool check)
    {
        isGhost = check;
    }

    // Start is called before the first frame update
    void Start()
    {
        item = this.gameObject;

        // adds this object to observer list in statechecker
        stateChecker.RegisterObserver(this);
    }

    // Update is called once per frame
    void Update()
    {
        //changes materials 
        if(isGhost == true)
        {
            item.GetComponent<Outline>().enabled = true;
        }
        else
            item.GetComponent<Outline>().enabled = false;
        }
}
