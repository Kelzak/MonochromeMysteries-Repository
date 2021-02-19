using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerSwitch : ItemAbs
{

    public int orderNumber;
    private static string correctOrder = "12345";
    private static string inputtedCode = "";
    public bool isFlipped;

    public static int numOfPulledSwitches;

    public bool correctCodeInputted;

    public static bool stationPowerOn;

    public GameObject[] powerSwitches;
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        //player = FindObjectOfType<Player>();
        powerSwitches = GameObject.FindGameObjectsWithTag("powerswitch");
        stationPowerOn = false;
        correctCodeInputted = false;
        numOfPulledSwitches = 0;
        inputtedCode = "";
        isFlipped = false;
        gameObject.GetComponent<MeshRenderer>().material.color = Color.yellow;
    }

    // Update is called once per frame
    void Update()
    {
        player = Player.possessedObj;

        if (numOfPulledSwitches >= 5 && !correctCodeInputted)
        {
            if (inputtedCode == correctOrder)
            {
                stationPowerOn = true;
                correctCodeInputted = true;
                gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
                Debug.Log("Correct!");
            }
            else
            {
                foreach(GameObject powerSwitch in powerSwitches)
                {
                    powerSwitch.GetComponent<MeshRenderer>().material.color = Color.yellow;
                    powerSwitch.GetComponent<PowerSwitch>().isFlipped = false;
                }
                Debug.Log("Incorrect!");
                inputtedCode = "";
                numOfPulledSwitches = 0;
            }
        }

        if(Input.GetKeyDown(KeyCode.Y))
        {
            stationPowerOn = true;
        }
    }

    public override void Activate()
    {
        if (player.GetComponent<Technician>())
        {
            if (!correctCodeInputted && !isFlipped && PowerBox.switchesOn)
            {
                Debug.Log("turned on switch");
                numOfPulledSwitches += 1;
                Debug.Log("number of pulled switches: " + numOfPulledSwitches);
                gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
                isFlipped = true;
                inputtedCode += orderNumber.ToString();
                Debug.Log("Inputted Code is: " + inputtedCode);
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
            GetComponent<Item>().SetUI(null, null, null, "Press F to Flip Switch", true);
        }

    }
}
