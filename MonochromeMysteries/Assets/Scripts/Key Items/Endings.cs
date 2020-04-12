using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Endings : MonoBehaviour
{
    public Photographer photographer;
    //public Player player;
    public GameObject knifeInstructions;
    public GameObject knifeConfirmation;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.K) && Player.hasKnife && !Player.isReadingLetter && !StateChecker.isGhost)
        {
            knifeConfirmation.SetActive(true);
            Player.isReadingLetter = true;
            GameController.TogglePause();
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            photographer.CameraLensActive = false;
            photographer.canTakePhoto = false;
        }
    }

    public void UseKnife(string decision)
    {
        if(decision == "kill")
        {
            if(photographer.GetComponent<Player>()) //you win
            {
                Debug.Log("You chose correctly!");
                //SceneManager.LoadScene("WinScene");
            }
            else if(photographer.GetComponent<Player>() == null) //you lose
            {
                Debug.Log("You chose wrong :(");
                //SceneManager.LoadScene("LoseScene");
            }
        }
        else if(decision == "notyet")
        {
            HideKnifeInstructions();
        }
    }

    public void ShowKnifeInstructions()
    {
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
            photographer.canTakePhoto = true;
        }
        Player.isReadingLetter = false;
    }
}
