using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PadlockPuzzle : MonoBehaviour
{
    [SerializeField]
    //Safe1 in the manager's office
    public static string correctCode1 = "030646"; //The mechanic's birthday
    //Safe2 in the rat room
    public static string correctCode2 = "918"; //Symbols are used here
    public static string correctCode3 = "746583"; 
    public static string correctCode4 = "101372";
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
    public GameObject safe4;

    [HideInInspector]
    public bool safe1Open;
    [HideInInspector]
    public bool safe2Open;
    [HideInInspector]
    public bool safe3Open;
    [HideInInspector]
    public bool safe4Open;

    public bool safeIsOpen = false;
   
    public GameObject keypadPanel;
    public GameObject symbolPanel;
    public InputField inputField;
    public InputField symbolInputField;

    public Photographer photographer;

    SafeAnim safeAnim;

    public bool keypadisUp;

    //public int totalInputs = 0;

    // Start is called before the first frame update
    void Start()
    {
        safeAnim = GetComponent<SafeAnim>();
        //inputCodeText.text = "";
        //enteredCode1 = enteredCode2 = enteredCode3 = "";
        inputField.characterLimit = 6;
        symbolInputField.characterLimit = 3;
    }

    // Update is called once per frame
    void Update()
    {
        /* //FOR TESTING
        if(Input.GetKeyDown(KeyCode.H))
        {
            HideKeypadAndReset();
        }
        else if(Input.GetKeyDown(KeyCode.J))
        {
            ShowKeypad();
        }*/

        if (Input.GetKeyDown(KeyCode.Escape) && (keypadPanel.activeSelf || symbolPanel.activeSelf))
        {
            HideKeypadAndReset();
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
            if (inputField.text.Length != 3 && Player.safeName == safe2.name)
            {
                symbolInputField.text += numberPressed;
            }
            if (inputField.text.Length != 6)
            {
                if (Player.safeName == safe1.name)
                {
                    inputField.text += numberPressed;
                    //enteredCode1 += numberPressed;
                    //totalInputs += 1;
                }
                else if (Player.safeName == safe3.name)
                {
                    inputField.text += numberPressed;
                    //enteredCode3 += numberPressed;
                    //totalInputs += 1;
                }
                else if (Player.safeName == safe4.name)
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
            if (symbolInputField.text.Length > 0)
            {
                symbolInputField.text = symbolInputField.text.Remove(symbolInputField.text.Length - 1, 1);
                // totalInputs -= 1;
            }

        }
    }

    public void ShowKeypad()
    {
        if(Player.safeName == safe2.name)
        {
            symbolPanel.SetActive(true);
        }
        else
        {
            keypadPanel.SetActive(true);
        }
        keypadisUp = true;
        GameController.TogglePause();
        photographer.CameraLensActive = false;
        photographer.canTakePhoto = false;
        Time.timeScale = 0;
        Player.canMove = false;
        Player.canLook = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void HideKeypadAndReset()
    {
        if (Player.safeName == safe2.name)
        {
            symbolPanel.SetActive(false);
        }
        else
        {
            keypadPanel.SetActive(false);
        }
        if(photographer.GetComponent<Player>())
        {
            photographer.CameraLensActive = true;
            //photographer.canTakePhoto = false;
        }
        keypadisUp = false;
        GameController.TogglePause();
        Time.timeScale = 1;
        audioSource.PlayOneShot(buttonPressedSFX);
        Player.canMove = true;
        Player.canLook = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        //photographer.CameraLensActive = true;
        inputField.text = "";
        inputField.placeholder.GetComponent<Text>().text = "Enter password...";
        symbolInputField.text = "";
        symbolInputField.placeholder.GetComponent<Text>().text = "Enter password...";
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
            safeAnim = safe1.transform.Find("Hinge").GetComponent<SafeAnim>();
            safeAnim.OpenSafe(safe1);
            safe1Open = true;
            safeIsOpen = true;
            //safe1.SetActive(false);
        }
        else if (Player.safeName == safe2.name && symbolInputField.text == correctCode2)
        {
            audioSource.PlayOneShot(safeOpeningSFX);
            //Debug.Log("Correct!");
            HideKeypadAndReset();
            safeAnim = safe2.transform.Find("Hinge").GetComponent<SafeAnim>();
            safe2Open = true;
            safeAnim.OpenSafe(safe2);
            safeIsOpen = true;
            //safe2.SetActive(false);
        }
        else if (Player.safeName == safe3.name && inputField.text == correctCode3)
        {
            audioSource.PlayOneShot(safeOpeningSFX);
            //Debug.Log("Correct!");
            HideKeypadAndReset();
            safeAnim = safe3.transform.Find("Hinge").GetComponent<SafeAnim>();
            safe3Open = true;
            safeAnim.OpenSafe(safe3);
            safeIsOpen = true;
            //safe3.SetActive(false);
        }
        else if (Player.safeName == safe4.name && inputField.text == correctCode4)
        {
            audioSource.PlayOneShot(safeOpeningSFX);
            //Debug.Log("Correct!");
            HideKeypadAndReset();
            safeAnim = safe4.transform.Find("Hinge").GetComponent<SafeAnim>();
            safe4Open = true;
            safeAnim.OpenSafe(safe4);
            safeIsOpen = true;
            //safe3.SetActive(false);
        }
        else
        {
            audioSource.PlayOneShot(incorrectSFX);
            inputField.placeholder.GetComponent<Text>().text = "Incorrect";
            symbolInputField.placeholder.GetComponent<Text>().text = "Incorrect";
            //inputCodeText.text = "";
            inputField.text = "";
            symbolInputField.text = "";
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
