/*
 * Created by Matt Kirchoff
 * Ghost or not, sends info to items for outlines
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//this class is the subject to the item obsevers
public class StateChecker : MonoBehaviour
{
    //private List<IObserver> observers = new List<IObserver>();

    public GameObject player;

    public static bool isGhost;

    //public void NotifyObservers()
    //{
    //    foreach (IObserver observer in observers)
    //    {
    //        observer.UpdateData(isGhost);
    //    }
    //}

    //public void RegisterObserver(IObserver observer)
    //{
    //    observers.Add(observer);

    //    observer.UpdateData(isGhost);
    //}

    //public void RemoveObserver(IObserver observer)
    //{
    //    if (observers.Contains(observer))
    //    {
    //        observers.Remove(observer);
    //    }
    //}

    ////sets the bool and then sends it to observers (items)
    //public void SetSight(bool sight)
    //{
    //    isGhost = sight;
    //    NotifyObservers();

    //    /*foreach (int x in temp)
    //    {
    //        this.isGhost = sight;
    //        NotifyObservers();
    //    }*/
    //    //Debug.Log("notify observers");
    //}
    // Start is called before the first frame update
    void Start()
    {
        isGhost = true;
    }

    // Update is called once per frame
    void Update()
    {
        //temp = ClueCatalogue._instance.DetectCluesOnScreen();

        //checks if player is active (if in ghost form)
        if (player.activeInHierarchy)
        {
            isGhost = true;
            //SetSight(true);
        }
        else
        {
            isGhost = false;

            //SetSight(false);
        }
    }
}
