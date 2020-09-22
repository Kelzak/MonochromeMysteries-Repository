//Made by matt kirchoff + kevon long

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class Safe : MonoBehaviour
{
    public string code;

    private Photographer photographer;
    private Player player;

    private GameObject Background;

    public GameObject keypadPanel;
    public GameObject symbolPanel;
    public InputField inputField;
    public InputField symbolInputField;

    private SafeAnim animator;

    [HideInInspector]
    public bool readTime;
    private bool closeTime;

    private bool uiOpen;
    private bool safeOpened;
    private bool ghostSafe;

    public AudioClip safeOpeningSFX;
    public AudioClip buttonPressedSFX;
    public AudioClip incorrectSFX;
    private AudioSource audioSource;


    [Range(0.0f, 1.0f)]
    public float soundVolume = .5f;

    // Start is called before the first frame update
    void Start()
    {
        photographer = FindObjectOfType<Photographer>();
        player = FindObjectOfType<Player>();
        Background = player.darkBackground;
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = soundVolume;

        audioSource.rolloffMode = AudioRolloffMode.Custom;
        audioSource.minDistance = 0f;
        audioSource.maxDistance = 5f;
        audioSource.spatialBlend = .5f;
        animator = transform.Find("Hinge").GetComponent<SafeAnim>();

        inputField.characterLimit = 6;
        symbolInputField.characterLimit = 3;
    }
        // Update is called once per frame
        void Update()
    {
        //fliping
        if (uiOpen)
        {
            //close readable
            if ((Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.F) && readTime))
            {
                Debug.Log("close");
                Close();
            }

            //is right?
            if(inputField.text == code)
            {
                audioSource.PlayOneShot(safeOpeningSFX);
                Debug.Log("Correct!");
                HideKeypadAndReset();
                animator = transform.Find("Hinge").GetComponent<SafeAnim>();
                animator.OpenSafe(this.gameObject);
                safeOpened = true;
                //safe1.SetActive(false);
            }
        }
    }

    public void Activate()
    {
        if (!closeTime && !safeOpened)
        {
            ShowKeypad();
            uiOpen = true;

            if (gameObject.GetComponent<Photographer>())
            {
                photographer.CameraLensActive = false;
                photographer.canTakePhoto = false;
            }

            //GameController.TogglePause();
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            Player.EnableControls(false);
            Background.SetActive(true);
            StartCoroutine(ReadTime());

            
            //audioSource.PlayOneShot(safeOpeningSFX);
        }

    }

    public void Close()
    {
        if (readTime)
        {
            if (player.GetComponent<Photographer>())
            {
                photographer.CameraLensActive = true;
                photographer.canTakePhoto = true;
            }
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Player.EnableControls(true);
            Background.SetActive(false);
            //GameController.TogglePause();
            uiOpen = false;
            audioSource.PlayOneShot(incorrectSFX);
            readTime = false;
            closeTime = true;
            HideKeypadAndReset();
            StartCoroutine(CloseTime());
        }
       
    }

    public void ShowKeypad()
    {
        if (ghostSafe)
        {
            symbolPanel.SetActive(true);
        }
        else
        {
            keypadPanel.SetActive(true);
        }
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
        if (ghostSafe)
        {
            symbolPanel.SetActive(false);
        }
        else
        {
            keypadPanel.SetActive(false);
        }
        if (photographer.GetComponent<Player>())
        {
            photographer.CameraLensActive = true;
            photographer.canTakePhoto = false;

        }
        GameController.TogglePause();
        Time.timeScale = 1;
        audioSource.PlayOneShot(buttonPressedSFX);
        Player.canMove = true;
        Player.canLook = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        inputField.text = "";
        inputField.placeholder.GetComponent<Text>().text = "Enter password...";
        symbolInputField.text = "";
        symbolInputField.placeholder.GetComponent<Text>().text = "Enter password...";
        Background.SetActive(false);
        //enteredCode1 = enteredCode2 = enteredCode3 = "";
        //totalInputs = 0;
    }

    public void DetermineNumberPressed(string numberPressed)
    {
        audioSource.PlayOneShot(buttonPressedSFX);
        if (numberPressed != "backspace")
        {
            if (inputField.text.Length != 3 && ghostSafe)
            {
                symbolInputField.text += numberPressed;
            }
            if (inputField.text.Length != 6)
            {
                inputField.text += numberPressed;
            }
        }
        else //when backspace is pressed
        {
            if (inputField.text.Length > 0)
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
    public void CheckifCodeisCorrect()
    {
        if(inputField.text == code)
        {
            audioSource.PlayOneShot(safeOpeningSFX);
            Debug.Log("Correct!");
            HideKeypadAndReset();
            animator = transform.Find("Hinge").GetComponent<SafeAnim>();
            animator.OpenSafe(this.gameObject);
            safeOpened = true;
        }
    }

    public IEnumerator ReadTime()
    {
        yield return new WaitForSecondsRealtime(.1f);
        readTime = true;
    }
    public IEnumerator CloseTime()
    {
        yield return new WaitForSecondsRealtime(.1f);
        closeTime = false;
    }
}
