using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationText : MonoBehaviour
{
    public PlayerHUD playerHUD;
    public GameObject HUDobj;
    public static bool showOverride;
    // Start is called before the first frame update
    void Start()
    {
        //playerHUD = FindObjectOfType<PlayerHUD>();
        playerHUD = HUDobj.GetComponent<PlayerHUD>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<Player>())
        {
            PlayerHUD.locationString = gameObject.name;
            PlayerHUD.showLocation = true;
            showOverride = true;
            //playerHUD.showLocation = true;
        }
        else if(!showOverride)
        {
            PlayerHUD.showLocation = false;
            //playerHUD.showLocation = false;
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Player>())
        {
            showOverride = false;

        }
    }

}
