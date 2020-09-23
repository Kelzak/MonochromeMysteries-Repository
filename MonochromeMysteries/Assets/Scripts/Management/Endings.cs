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
    public GameObject darkBackground;
    public GameObject menu;
    public TMP_Text personDecidedText;
    public static bool isUsingKnife;

    // Start is called before the first frame update
    void Start()
    {
        personDecidedText = GameController._instance.mainHUD.transform.Find("KnifeConfirmationPanel").Find("PersonToKill").GetComponent<TMP_Text>();
        menu = GameObject.Find("HUD").transform.Find("Menu").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.K) && Player.hasKnife && !menu.activeSelf && !Readables.isReadingLetter && !PadlockPuzzle.keypadisUp && !StateChecker.isGhost && !Player.isRat)
        {
            knifeConfirmation.SetActive(true);
            isUsingKnife = true;
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
        Dialogue.AddLine(Dialogue.Character.Pete, "I’m glad to see you’ve progressed this far in your search for truth. Well done. I can sense your motivation is at its peak.");
        Dialogue.AddLine(Dialogue.Character.Pete, "It gives me great joy to see you’ve surpassed my expectations. You’ve listened to me well, and for that, I thank you.");
        Dialogue.AddLine(Dialogue.Character.Pete, "Now, I suspect you’ve been pondering how youll escape this purgatory caused by the turmoil in your heart. It's enough to drive even the strongest men mad.");
        Dialogue.AddLine(Dialogue.Character.Pete, "What you desire is closure, and you’ll need it to heal your fractured heart. By any means necessary, no?");
        Dialogue.AddLine(Dialogue.Character.Pete, "I’ve brought you a gift locked in that safe that’ll prove useful in your search for justice. You’ll be able to access it and use it as a soul.");
        Dialogue.AddLine(Dialogue.Character.Pete, "Your decision to use this item is up to you, but remember why you’re doing this and what justice means to you. The fate of your heart - and your killer’s - is in your hands.");
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
        isUsingKnife = true;
        Readables.isReadingLetter = true;
        Read.isReading = true;
        darkBackground.SetActive(true);
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
        isUsingKnife = false;
        darkBackground.SetActive(false);
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
        Read.isReading = false;
    }

    public IEnumerator WaitToTurnOnCamera()
    {
        yield return new WaitForSecondsRealtime(1);
        photographer.canTakePhoto = true;
    }
}
