using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using System;

public class NextStation : ItemAbs
{

    public GameObject player;

    public AudioSource audioSource;
    public AudioClip choochoo;

    public Image fadeToBlackScreen;
    public TextMeshProUGUI toBeContinued;


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
                gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
                Debug.Log("You win!");
            }
        }
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
        else
        {
            GetComponent<Item>().SetUI(null, null, null, "Press F to go to the next station", true);
        }
    }
}
