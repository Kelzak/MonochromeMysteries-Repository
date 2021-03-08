using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerBox : ItemAbs
{
    public GameObject player;

    public static bool switchesOn;

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
            switchesOn = true;
        }

    }

    public override void Activate()
    {
        if(player.GetComponent<Technician>())
        {
            if (Player.fuses.Count == 3 && !switchesOn)
            {
                Debug.Log("Switches on!");
                gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
                switchesOn = true;
            }
            else
            {
                Debug.Log("Need more fuses");
            }
        }
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
