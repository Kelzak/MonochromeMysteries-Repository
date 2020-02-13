using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//observer to state checker to display outline on item
public class Item : MonoBehaviour, IObserver
{
    public StateChecker stateChecker;

    public Material glowMat;
    public Material defaultMat;

    Renderer rend;

    private bool isGhost;

    //gets data sent fron statechecker
    public void UpdateData(bool check)
    {
        isGhost = check;
    }

    // Start is called before the first frame update
    void Start()
    {
        rend = this.GetComponent<Renderer>();

        // adds this object to observer list in statechecker
        stateChecker.RegisterObserver(this);
    }

    // Update is called once per frame
    void Update()
    {
        //changes materials 
        if(isGhost == true)
        {
            this.rend.material = glowMat;
        }
        else
            this.rend.material = defaultMat;
    }
}
