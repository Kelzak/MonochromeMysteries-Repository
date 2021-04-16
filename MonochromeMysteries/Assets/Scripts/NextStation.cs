using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class NextStation : ItemAbs
{

    public GameObject player;

    public AudioSource audioSource;
    public AudioClip choochoo;

    public Image fadeToBlackScreen;
    public TextMeshProUGUI toBeContinued;

    public Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        fadeToBlackScreen.canvasRenderer.SetAlpha(0.0f);
        //player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        player = Player.possessedObj;
        if(Input.GetKeyDown(KeyCode.F4))
        {
            FadeToBlackScreen();
        }
    }

    public override void Activate()
    {
        if(player.gameObject.GetComponent<Conductor>())
        {
            if(PowerSwitch.stationPowerOn)
            {
                audioSource.PlayOneShot(choochoo);
                FadeToBlackScreen();
                _animator.SetBool("flip", true);
                Player.EnableControls(false);
                Read.isReading = true;
                StartCoroutine(SwitchToTitleScreen());
                //GameController.TogglePause();
                //Debug.Log("You win!");
                //gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
            }
        }
    }

    public IEnumerator SwitchToTitleScreen()
    {
        yield return new WaitForSeconds(5f);
        Player.EnableControls(true);
        Read.isReading = false;
        SceneManager.LoadScene("TrainMainMenu");
    }

    private void FadeToBlackScreen()
    {
        fadeToBlackScreen.gameObject.SetActive(true);
        fadeToBlackScreen.CrossFadeAlpha(1, 3, false);
        StartCoroutine(ShowText());
    }

    IEnumerator ShowText()
    {
        yield return new WaitForSecondsRealtime(3f);
        toBeContinued.gameObject.SetActive(true);
        toBeContinued.CrossFadeAlpha(1, 2, false);
        
    }

    public override void SetItemUI()
    {
        if (StateChecker.isGhost)
        {
            GetComponent<Item>().SetUI(null, null, null, "Spirit can't use", true);
        }

        if (!PowerSwitch.stationPowerOn)
        {
            GetComponent<Item>().SetUI(null, null, null, "Needs Station Power on", true);
        }
        else
        {
            if (player.GetComponent<Conductor>())
            {
                GetComponent<Item>().SetUI(null, null, null, "Press F to go to the Next Station", true);
            }
            else//(!PowerSwitch.stationPowerOn && player.GetComponent<Technician>())
            {
                GetComponent<Item>().SetUI(null, null, null, "Needs Conductor", true);
            }

        }



    }
}
