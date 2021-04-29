using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.ParticleSystemJobs;

public class PowerSwitch : ItemAbs
{
    public Animator _animator;


    public int orderNumber;
    private static string correctOrder = "43521";
    private static string inputtedCode = "";
    public bool isFlipped;

    public static int numOfPulledSwitches;

    public bool correctCodeInputted;

    public static bool stationPowerOn;

    public GameObject[] powerSwitches;
    public GameObject player;

    public GameObject[] switchLights;

    public AudioSource audioSource;
    public AudioSource generatorAudioSource;
    public AudioClip switchTurnedOn;
    public AudioClip switchesTurnedOff;

    public ParticleSystem sparks;

    // Start is called before the first frame update
    void Start()
    {
        //player = FindObjectOfType<Player>();
        audioSource = GetComponent<AudioSource>();
        powerSwitches = GameObject.FindGameObjectsWithTag("powerswitch");
        //switchLights = GameObject.FindGameObjectsWithTag("switchlight");
        stationPowerOn = false;
        correctCodeInputted = false;
        numOfPulledSwitches = 0;
        inputtedCode = "";
        isFlipped = false;
        //gameObject.GetComponent<MeshRenderer>().material.color = Color.yellow;
    }

    // Update is called once per frame
    void Update()
    {
        player = Player.possessedObj;

        if (numOfPulledSwitches >= 5 && !correctCodeInputted)
        {
            if (inputtedCode == correctOrder)
            {
                foreach (GameObject switchLight in switchLights)
                {
                   switchLight.GetComponent<MeshRenderer>().material.color = Color.green;
                    Destroy(gameObject.GetComponent<Item>());
                    Destroy(gameObject.GetComponent<Outline>());
                }
                stationPowerOn = true;
                correctCodeInputted = true;
                generatorAudioSource.Play();
                sparks.Play();
                //gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
                Debug.Log("Correct!");

            }
            else
            {
                foreach (GameObject powerSwitch in powerSwitches)
                {
                    powerSwitch.GetComponent<PowerSwitch>().isFlipped = false;
                    powerSwitch.GetComponent<PowerSwitch>()._animator.SetBool("flip", false);
                }
                foreach (GameObject switchLight in switchLights)
                {
                    switchLight.GetComponent<MeshRenderer>().material.color = Color.white;
                }
                audioSource.PlayOneShot(switchesTurnedOff);
                Debug.Log("Incorrect!");
                inputtedCode = "";
                numOfPulledSwitches = 0;
            }
        }

        /*if (Input.GetKeyDown(KeyCode.Y))
        {
            stationPowerOn = true;
        }*/
    }

    public override void Activate()
    {
        //if (player.GetComponent<Technician>())
        //{
        if (!correctCodeInputted && !isFlipped && PowerBox.switchesOn)
        {
            ActivateLight();
            audioSource.PlayOneShot(switchTurnedOn);
            Debug.Log("turned on switch");
            numOfPulledSwitches += 1;
            //Debug.Log("number of pulled switches: " + numOfPulledSwitches);
            //gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
            gameObject.GetComponent<Item>().enabled = false;
            isFlipped = true;
            inputtedCode += orderNumber.ToString();
            
            _animator.SetBool("flip", true);
            //set this bool ^ to false to make the animation return to the other way *should

            //Debug.Log("Inputted Code is: " + inputtedCode);
        }
        //}

    }

    public static void SwitchOff()
    {
        //_animator.SetBool("flip", false);
    }

    public void ActivateLight()
    {
        switchLights[orderNumber - 1].gameObject.GetComponent<MeshRenderer>().material.color = Color.yellow;
    }

    public override void SetItemUI()
    {
        if (StateChecker.isGhost)
        {
            GetComponent<Item>().SetUI(null, null, null, "Spirit can't use", true);
        }
        else if(!isFlipped || !stationPowerOn)
        {
            GetComponent<Item>().SetUI(null, null, null, "Press F to Flip Switch", true);
        }

    }
}
