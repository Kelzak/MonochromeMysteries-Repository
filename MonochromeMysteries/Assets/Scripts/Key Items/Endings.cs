using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Endings : MonoBehaviour
{
    public Photographer photographer;
    //public Player player;
    public GameObject knifeInstructions;
    public GameObject knifeConfirmation;
    public TMP_Text personDecidedText;

    // Start is called before the first frame update
    void Start()
    {
        personDecidedText = GameController.mainHUD.transform.Find("KnifeConfirmationPanel").Find("PersonToKill").GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.K) && Player.hasKnife && !Readables.isReadingLetter && !StateChecker.isGhost && !Player.isRat)
        {
            knifeConfirmation.SetActive(true);
            personDecidedText.text = "Are you certain the <b>" + Player.characterRoleForEnding + "</b> is the person who murdered you?";
            Readables.isReadingLetter = true;
            GameController.TogglePause();
            photographer.CameraLensActive = false;
            photographer.canTakePhoto = false;
        }
        else if(Input.GetKeyDown(KeyCode.Escape) && (knifeConfirmation.activeInHierarchy || knifeInstructions.activeInHierarchy))
        {
            HideKnifeInstructions();
        }
    }

    public IEnumerator KnifeScript()
    {
        yield return new WaitForSeconds(1f);
        Dialogue.AddLine(Dialogue.Character.Pete, "Well I’ll be, if I had to live in this room I’d be madder than a wet hen! And the stench: YUGH!");
        Dialogue.AddLine(Dialogue.Character.Pete, "Anyhoo, it’s good to see you here, son. It shows me how motivated you are to find who did you wrong, even if it means stepping into other people’s business…and rat shit.");
        Dialogue.AddLine(Dialogue.Character.Pete, "You remind me a lot of myself back in my hotel inspectin’ days. Hehe, yeah those were crazy times...I reckon that’s why my gut was screamin’ at me to come lend you a helping hand.");
        Dialogue.AddLine(Dialogue.Character.Pete, "See, without any closure on your life, you’re gonna end up wandering around in this purgatory hell forever, and believe me that’s just enough time to drive any fella mad.");
        Dialogue.AddLine(Dialogue.Character.Pete, "There’s a lot of things that prohibit you from getting closure, and one of ‘ems your own emotions. I reckon you ain’t too happy about gettin’ whacked, huh?");
        Dialogue.AddLine(Dialogue.Character.Pete, "It just ain’t fair...and if you ask this old geezer, ain’t nothing wrong with a little vigilante justice.");
        Dialogue.AddLine(Dialogue.Character.Pete, "I brought you somethin’ that’ll help you out in that safe over yonder. You’ll be able to use the safe and it's contents the way you are now, as a spirit, since I made them myself. I hope it finds you your ticket out, and more importantly, a little peace.");
        Dialogue.AddLine(Dialogue.Character.Pete, "I’ll let you get back to getting revenge on the bastard who put you in this mess. Bless your heart, son.");
    }

    public void UseKnife(string decision)
    {
        if (decision == "kill")
        {
            if (photographer.GetComponent<Player>()) //you win
            {
                //Debug.Log("You chose correctly!");
                SceneManager.LoadScene("WinScene");
            }
            else if (photographer.GetComponent<Player>() == null) //you lose
            {
                //Debug.Log("You chose wrong :(");
                SceneManager.LoadScene("LoseScene");
            }
        }
        else if (decision == "notyet")
        {
            HideKnifeInstructions();
        }
    }

    public void ShowKnifeInstructions()
    {
        Readables.isReadingLetter = true;
        knifeInstructions.SetActive(true);
        GameController.TogglePause();
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        if (photographer.GetComponent<Player>())
        {
            photographer.CameraLensActive = false;
            photographer.canTakePhoto = false;
        }
        Time.timeScale = 0;
        Player.EnableControls(false);
    }

    public void HideKnifeInstructions()
    {
        Cursor.visible = false;
        knifeInstructions.SetActive(false);
        knifeConfirmation.SetActive(false);
        GameController.TogglePause();
        if (photographer.GetComponent<Player>())
        {
            photographer.CameraLensActive = true;
            StartCoroutine(WaitToTurnOnCamera());
        }
        Readables.isReadingLetter = false;
    }

    public IEnumerator WaitToTurnOnCamera()
    {
        yield return new WaitForSecondsRealtime(1);
        photographer.canTakePhoto = true;
    }
}
