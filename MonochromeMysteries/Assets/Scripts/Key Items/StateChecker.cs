// created my matt kirchoff
// checks and updates observers to see if is ghost or not


using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//this class is the subject to the item obsevers
public class StateChecker : MonoBehaviour, ISubject
{
    private List<IObserver> observers = new List<IObserver>();

    int[] temp;

    public GameObject player;

    private bool isGhost;
    public void NotifyObservers()
    {
        foreach (IObserver observer in observers)
        {
            observer.UpdateData(isGhost);
        }
    }

    public void RegisterObserver(IObserver observer)
    {
        observers.Add(observer);

        observer.UpdateData(isGhost);
    }

    public void RemoveObserver(IObserver observer)
    {
        if (observers.Contains(observer))
        {
            observers.Remove(observer);
        }
    }

    //sets the bool and then sends it to observers (items)
    public void SetSight(bool sight)
    {
        foreach(int x in temp)
        {
            this.isGhost = sight;
            NotifyObservers();
        }
        //Debug.Log("notify observers");
    }
    // Start is called before the first frame update
    void Start()
    {
        isGhost = true;
    }

    // Update is called once per frame
    void Update()
    {
        temp = ClueCatalogue._instance.DetectCluesOnScreen();

        //checks if player is active (if in ghost form)
        if (player.activeInHierarchy)
        {
            SetSight(true);
        }
        else
        {
            SetSight(false);
        }
    }
}
