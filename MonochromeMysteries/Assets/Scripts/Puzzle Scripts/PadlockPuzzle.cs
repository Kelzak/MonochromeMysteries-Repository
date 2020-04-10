using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PadlockPuzzle : MonoBehaviour
{
    [SerializeField]
    public static string correctCode1 = "030646"; //The mechanic's birthday
    public static string correctCode2 = "654321";
    public static string correctCode3 = "000000";
    /*public static string enteredCode1;
    public static string enteredCode2;
    public static string enteredCode3;*/
    public AudioClip safeOpeningSFX;
    public AudioClip buttonPressedSFX;
    public AudioClip incorrectSFX;
    public AudioSource audioSource;

    //public Text inputCodeText;

    public GameObject safe1;
    public GameObject safe2;
    public GameObject safe3;
   
    public GameObject keypadPanel;
    public InputField inputField;

    public Photographer photographer;

    //public int totalInputs = 0;

    // Start is called before the first frame update
    void Start()
    {
        //inputCodeText.text = "";
        //enteredCode1 = enteredCode2 = enteredCode3 = "";
        inputField.characterLimit = 6;
    }

    // Update is called once per frame
    void Update()
    {
        //FOR TESTING PURPOSES
        if(Input.GetKeyDown(KeyCode.H))
        {
            HideKeypadAndReset();
        }
        else if(Input.GetKeyDown(KeyCode.J))
        {
            ShowKeypad();
        }
    }

    private void OnMouseDown()
    {
        //enteredCode1 += gameObject.name;
        //totalInputs += 1;
    }

    public void DetermineNumberPressed(string numberPressed)
    {
        audioSource.PlayOneShot(buttonPressedSFX);
        if(numberPressed != "backspace")
        {
            if (inputField.text.Length != 6)
            {
                if (Player.safeName == safe1.name)
                {
                    inputField.text += numberPressed;
                    //enteredCode1 += numberPressed;
                    //totalInputs += 1;
                }
                else if (Player.safeName == safe2.name)
                {
                    inputField.text += numberPressed;
                    //enteredCode2 += numberPressed;
                   //totalInputs += 1;
                }
                else if (Player.safeName == safe3.name)
                {
                    inputField.text += numberPressed;
                    //enteredCode3 += numberPressed;
                    //totalInputs += 1;
                }
            }
        }
        else //when backspace is pressed
        {
            if(inputField.text.Length > 0)
            {
                inputField.text = inputField.text.Remove(inputField.text.Length - 1, 1);
               // totalInputs -= 1;
            }
        }
    }

    public void ShowKeypad()
    {
        GameController.TogglePause();
        photographer.CameraLensActive = false;
        photographer.canTakePhoto = false;
        Time.timeScale = 0;
        Debug.Log(Player.safeName);
        keypadPanel.SetActive(true);
        Player.canMove = false;
        Player.canLook = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void HideKeypadAndReset()
    {
        GameController.TogglePause();
        Time.timeScale = 1;
        audioSource.PlayOneShot(buttonPressedSFX);
        keypadPanel.SetActive(false);
        Player.canMove = true;
        Player.canLook = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        inputField.text = "";
        inputField.placeholder.GetComponent<Text>().text = "Enter password...";
        photographer.CameraLensActive = true;
        StartCoroutine(WaitToTurnOnCamera());
        //enteredCode1 = enteredCode2 = enteredCode3 = "";
        //totalInputs = 0;
    }

    public IEnumerator WaitToTurnOnCamera()
    {
        yield return new WaitForSecondsRealtime(1);
        photographer.canTakePhoto = true;
    }

    public void CheckifCodeisCorrect()
    {
        if (Player.safeName == safe1.name  && inputField.text == correctCode1)
        {
            audioSource.PlayOneShot(safeOpeningSFX);
            Debug.Log("Correct!");
            HideKeypadAndReset();
            safe1.SetActive(false);
        }
        else if (Player.safeName == safe2.name && inputField.text == correctCode2)
        {
            audioSource.PlayOneShot(safeOpeningSFX);
            Debug.Log("Correct!");
            HideKeypadAndReset();
            safe2.SetActive(false);
        }
        else if (Player.safeName == safe3.name && inputField.text == correctCode3)
        {
            audioSource.PlayOneShot(safeOpeningSFX);
            Debug.Log("Correct!");
            HideKeypadAndReset();
            safe3.SetActive(false);
        }
        else
        {
            audioSource.PlayOneShot(incorrectSFX);
            inputField.placeholder.GetComponent<Text>().text = "Incorrect";
            //inputCodeText.text = "";
            inputField.text = "";
            //enteredCode1 = enteredCode2 = enteredCode3 = "";
            //totalInputs = 0;
        }
    }
}

/*if (Input.GetKeyDown(KeyCode.Alpha1))
{
    if (Player.isAtTheFirstSafe)
    {
        totalInputs += 1;
        enteredCode1 += "1";
    }
    if (Player.isAtTheSecondSafe)
    {
        totalInputs += 1;
        enteredCode2 += "1";
    }
    if (Player.isAtTheThirdSafe)
    {
        totalInputs += 1;
        enteredCode3 += "1";
    }
}
else if (Input.GetKeyDown(KeyCode.Alpha2))
{
    if (Player.isAtTheFirstSafe)
    {
        totalInputs += 1;
        enteredCode1 += "2";
    }
    if (Player.isAtTheSecondSafe)
    {
        totalInputs += 1;
        enteredCode2 += "2";
    }
    if (Player.isAtTheThirdSafe)
    {
        totalInputs += 1;
        enteredCode3 += "2";
    }
}
else if (Input.GetKeyDown(KeyCode.Alpha3))
{
    if (Player.isAtTheFirstSafe)
    {
        totalInputs += 1;
        enteredCode1 += "3";
    }
    if (Player.isAtTheSecondSafe)
    {
        totalInputs += 1;
        enteredCode2 += "3";
    }
    if (Player.isAtTheThirdSafe)
    {
        totalInputs += 1;
        enteredCode3 += "3";
    }
}
else if (Input.GetKeyDown(KeyCode.Alpha4))
{
    if (Player.isAtTheFirstSafe)
    {
        totalInputs += 1;
        enteredCode1 += "4";
    }
    if (Player.isAtTheSecondSafe)
    {
        totalInputs += 1;
        enteredCode2 += "4";
    }
    if (Player.isAtTheThirdSafe)
    {
        totalInputs += 1;
        enteredCode3 += "4";
    }
}
else if (Input.GetKeyDown(KeyCode.Alpha5))
{
    if (Player.isAtTheFirstSafe)
    {
        totalInputs += 1;
        enteredCode1 += "5";
    }
    if (Player.isAtTheSecondSafe)
    {
        totalInputs += 1;
        enteredCode2 += "5";
    }
    if (Player.isAtTheThirdSafe)
    {
        totalInputs += 1;
        enteredCode3 += "5";
    }
}
else if (Input.GetKeyDown(KeyCode.Alpha6))
{
    if (Player.isAtTheFirstSafe)
    {
        totalInputs += 1;
        enteredCode1 += "6";
    }
    if (Player.isAtTheSecondSafe)
    {
        totalInputs += 1;
        enteredCode2 += "6";
    }
    if (Player.isAtTheThirdSafe)
    {
        totalInputs += 1;
        enteredCode3 += "6";
    }
}
else if (Input.GetKeyDown(KeyCode.Alpha7))
{
    if (Player.isAtTheFirstSafe)
    {
        totalInputs += 1;
        enteredCode1 += "7";
    }
    if (Player.isAtTheSecondSafe)
    {
        totalInputs += 1;
        enteredCode2 += "7";
    }
    if (Player.isAtTheThirdSafe)
    {
        totalInputs += 1;
        enteredCode3 += "7";
    }
}
else if (Input.GetKeyDown(KeyCode.Alpha8))
{
    if (Player.isAtTheFirstSafe)
    {
        totalInputs += 1;
        enteredCode1 += "8";
    }
    if (Player.isAtTheSecondSafe)
    {
        totalInputs += 1;
        enteredCode2 += "8";
    }
    if (Player.isAtTheThirdSafe)
    {
        totalInputs += 1;
        enteredCode3 += "8";
    }
}
else if (Input.GetKeyDown(KeyCode.Alpha9))
{
    if (Player.isAtTheFirstSafe)
    {
        totalInputs += 1;
        enteredCode1 += "9";
    }
    if (Player.isAtTheSecondSafe)
    {
        totalInputs += 1;
        enteredCode2 += "9";
    }
    if (Player.isAtTheThirdSafe)
    {
        totalInputs += 1;
        enteredCode3 += "9";
    }
}
else if (Input.GetKeyDown(KeyCode.Alpha0))
{
    if (Player.isAtTheFirstSafe)
    {
        totalInputs += 1;
        enteredCode1 += "0";
    }
    if (Player.isAtTheSecondSafe)
    {
        totalInputs += 1;
        enteredCode2 += "0";
    }
    if (Player.isAtTheThirdSafe)
    {
        totalInputs += 1;
        enteredCode3 += "0";
    }
}*/
