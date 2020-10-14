//Made by matt kirchoff + kevon long

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class Safe : ItemAbs
{
    public string code;

    private Photographer photographer;
    private Player player;

    private GameObject Background;

    public GameObject keypadPanel;
    public GameObject symbolPanel;
    public InputField inputField;
    public InputField symbolInputField;
    public GameObject darkBackground;

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

    public Safe[] safes;
    public List<GameObject> safeGameobjects = new List<GameObject>();

    [Range(0.0f, 1.0f)]
    public float soundVolume = .5f;

    // Start is called before the first frame update
    void Start()
    {
        safes = FindObjectsOfType<Safe>();
        foreach(Safe safe in safes)
        {
            safeGameobjects.Add(safe.gameObject);
        }

        photographer = FindObjectOfType<Photographer>();
        player = FindObjectOfType<Player>();
        Background = player.darkBackground;
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = soundVolume;

        keypadPanel = GameObject.Find("HUD").transform.Find("SafeKeypad").gameObject;
        symbolPanel = GameObject.Find("HUD").transform.Find("SymbolKeypad").gameObject;
        inputField = keypadPanel.transform.Find("InputtedCode").GetComponent<InputField>();
        symbolInputField = symbolPanel.transform.Find("InputtedCode").GetComponent<InputField>();
        darkBackground = GameObject.Find("HUD").transform.Find("DarkBackground").gameObject;

    
        audioSource.rolloffMode = AudioRolloffMode.Custom;
        audioSource.minDistance = 0f;
        audioSource.maxDistance = 5f;
        audioSource.spatialBlend = .5f;
        animator = transform.Find("Hinge").GetComponent<SafeAnim>();

        inputField.characterLimit = 6;
        symbolInputField.characterLimit = 3;

        StartCoroutine(DisableSafesInTime());
    }

    public IEnumerator DisableSafesInTime()
    {
        yield return new WaitForSecondsRealtime(2f);
        DisableSafes();
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
                GetComponent<BoxCollider>().enabled = false;
                Destroy(GetComponent<Item>());
                Destroy(GetComponent<Outline>());
                //Destroy(GetComponent<Safe>());
                //gameObject.GetComponent<Safe>().enabled = false;
                //safe1.SetActive(false);
            }
        }
        KeypadInput();
        
    }

    public override void Activate()
    {
        SelectCurrentSafe();
        Debug.Log(safeGameobjects);

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

            //gameObject.GetComponent<Safe>().enabled = true;

            //audioSource.PlayOneShot(safeOpeningSFX);
        }

    }

    public void SelectCurrentSafe()
    {
        foreach(GameObject safe in safeGameobjects)
        {
            if(safe != gameObject)
            {
                safe.GetComponent<Safe>().enabled = false;
            }
            else
            {
                safe.GetComponent<Safe>().enabled = true;
            }
        }
    }

    public void DisableSafes()
    {
        foreach(GameObject safe in safeGameobjects)
        {
            safe.GetComponent<Safe>().enabled = false;
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
            DisableSafes();
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
        darkBackground.SetActive(true);
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
        darkBackground.SetActive(false);
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
        gameObject.GetComponent<Safe>().enabled = false;
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

    public void KeypadInput()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            DetermineNumberPressed("1");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            DetermineNumberPressed("2");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            DetermineNumberPressed("3");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            DetermineNumberPressed("4");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            DetermineNumberPressed("5");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            DetermineNumberPressed("6");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            DetermineNumberPressed("7");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            DetermineNumberPressed("8");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            DetermineNumberPressed("9");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            DetermineNumberPressed("0");
        }
        else if (Input.GetKeyDown(KeyCode.Backspace))
        {
            DetermineNumberPressed("backspace");
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
            GetComponent<BoxCollider>().enabled = false;
            Destroy(GetComponent<Item>());
            Destroy(GetComponent<Outline>());
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

    public override void SetItemUI()
    {
        if(ghostSafe)
        {
            if(StateChecker.isGhost)
            {
                GetComponent<Item>().SetUI(null, null, null, "Press F to Use", true);
            }
            else
            {
                GetComponent<Item>().SetUI(null, null, null, "Only Spirit can Use", true);
            }
        }
        else
        {
            if (StateChecker.isGhost)
            {
                GetComponent<Item>().SetUI(null, null, null, "Spirit cant Use", true);
            }
            if (player.GetComponent<Rat>())
            {
                GetComponent<Item>().SetUI(null, null, null, "Rat cant Use", true);
            }
            else
            {
                GetComponent<Item>().SetUI(null, null, null, "Press F to Use", true);
            }
        }
    }
}
