using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationText : MonoBehaviour
{
    public PlayerHUD playerHUD;
    public GameObject HUDobj;
    // Start is called before the first frame update
    void Start()
    {
        playerHUD = FindObjectOfType<PlayerHUD>();
        HUDobj = playerHUD.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<Player>())
        {
            print("locations are printing");
            playerHUD.LocationText = other.gameObject.name;
            playerHUD.showLocation = true;
        }
        else
        {
            playerHUD.showLocation = false;
        }

    }

}
