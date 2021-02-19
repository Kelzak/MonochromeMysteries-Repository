using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextStation : ItemAbs
{

    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        //player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        player = Player.possessedObj;
    }

    public override void Activate()
    {
        if(player.gameObject.GetComponent<Conductor>())
        {
            if(PowerSwitch.stationPowerOn)
            {
                gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
                Debug.Log("You win!");
            }
        }
    }

    public override void SetItemUI()
    {
        if (StateChecker.isGhost)
        {
            GetComponent<Item>().SetUI(null, null, null, "Spirit can't use", true);
        }
        else
        {
            GetComponent<Item>().SetUI(null, null, null, "Press F to go to the next station", true);
        }
    }
}
