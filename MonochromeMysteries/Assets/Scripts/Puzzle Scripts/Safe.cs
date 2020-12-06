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
    

    public GameObject keypadPanel;
    public GameObject symbolPanel;
    public InputField inputField;
    public InputField symbolInputField;
    public GameObject darkBackground;
    public GameObject pressEscToClose;

    private SafeAnim animator;

    [HideInInspector]
    public bool readTime;
    private bool closeTime;

    public static bool uiOpen;
    private bool safeOpened;
    public bool ghostSafe;

    public AudioClip safeOpeningSFX;
    public AudioClip buttonPressedSFX;
    public AudioClip incorrectSFX;
    private AudioSource audioSource;

    public Safe[] safes;
    public List<GameObject> safeGameobjects = new List<GameObject>();

    public GameObject[] buttons;
    public GameObject[] spiritButtons;
    public List<Button> keypadButtons = new List<Button>();
    public List<Button> spiritKeypadButtons = new List<Button>();

    [Range(0.0f, 1.0f)]
    public float soundVolume = .5f;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        //player = FindObjectOfType<Player>();
        safes = FindObjectsOfType<Safe>();
        foreach(Safe safe in safes)
        {
            safeGameobjects.Add(safe.gameObject);
        }

        buttons = GameObject.FindGameObjectsWithTag("keypadButton");
        spiritButtons = GameObject.FindGameObjectsWithTag("spiritButton");

        foreach(GameObject button in buttons)
        {
            keypadButtons.Add(button.GetComponent<Button>());
        }
        foreach (Button button in keypadButtons)
        {
            button.onClick.RemoveAllListeners();
        }
        foreach (GameObject button in spiritButtons)
        {
            spiritKeypadButtons.Add(button.GetComponent<Button>());
        }
        foreach (Button button in spiritKeypadButtons)
        {
            button.onClick.RemoveAllListeners();
        }

        Debug.Log(keypadButtons);

        photographer = FindObjectOfType<Photographer>();
        
        //Background = player.darkBackground;
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = soundVolume;

        keypadPanel = GameObject.Find("HUD").transform.Find("SafeKeypad").gameObject;
        symbolPanel = GameObject.Find("HUD").transform.Find("SymbolKeypad").gameObject;
        inputField = keypadPanel.transform.Find("InputtedCode").GetComponent<InputField>();
        symbolInputField = symbolPanel.transform.Find("InputtedCode").GetComponent<InputField>();
        darkBackground = GameObject.Find("HUD").transform.Find("DarkBackground").gameObject;
        pressEscToClose = GameObject.Find("HUD").transform.Find("PressEscToClose").gameObject;
    
        audioSource.rolloffMode = AudioRolloffMode.Custom;
        audioSource.minDistance = 0f;
        audioSource.maxDistance = 5f;
        audioSource.spatialBlend = .5f;
        animator = transform.Find("Hinge").GetComponent<SafeAnim>();

        inputField.characterLimit = 6;
        symbolInputField.characterLimit = 3;

        //keypadPanel.SetActive(false);
        StartCoroutine(DisableSafesInTime());
    }

    public IEnumerator DisableSafesInTime()
    {
        yield return new WaitForSecondsRealtime(.1f);
        keypadPanel.SetActive(false);
        symbolPanel.SetActive(false);
        //DisableSafes();
    }

    // Update is called once per frame
    void Update()
    {
        
        //fliping
        if (uiOpen)
        {
            //close readable
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("close");
                //Close();
                HideKeypadAndReset();
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
        //KeypadInput();
    }

    public override void Activate()
    {
        if(!player.GetComponent<Rat>())
        {
            if((ghostSafe && StateChecker.isGhost) || (!ghostSafe && !StateChecker.isGhost))
            {
                //SelectCurrentSafe();
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
                    //Background.SetActive(true);
                    StartCoroutine(ReadTime());

                    //gameObject.GetComponent<Safe>().enabled = true;

                    //audioSource.PlayOneShot(safeOpeningSFX);
                }
            }

        }
        
    }

    public void SelectCurrentSafe()
    {
        foreach (GameObject safe in safeGameobjects)
        {
            if (safe != gameObject)
            {
                safe.GetComponent<Safe>().enabled = false;
            }
            else
            {
                safe.GetComponent<Safe>().enabled = true;
            }

        }
    }

    public void SelectButtonsinSafe()
    {
        if(!ghostSafe)
        {
            foreach (Button button in keypadButtons)
            {
                button.onClick.AddListener(() => DetermineNumberPressed(button.gameObject.name));
            }
        }
        else
            foreach (Button button in spiritKeypadButtons)
            {
                button.onClick.AddListener(() => DetermineNumberPressed(button.gameObject.name));
            }

    }

    public void DeselectButtonsinSafe()
    {
        foreach (Button button in keypadButtons)
        {
            button.onClick.RemoveAllListeners();
        }
        foreach (Button button in spiritKeypadButtons)
        {
            button.onClick.RemoveAllListeners();
        }
    }

    public void DisableSafes()
    {
        foreach(GameObject safe in safeGameobjects)
        {
            safe.GetComponent<Safe>().enabled = false;
        }

    }

    /*public void Close()
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
            GameController.TogglePause();
            uiOpen = false;
            audioSource.PlayOneShot(incorrectSFX);
            readTime = false;
            closeTime = true;
            HideKeypadAndReset();
            DisableSafes();
            StartCoroutine(CloseTime());
        }
       
    }*/

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
        SelectButtonsinSafe();
        darkBackground.SetActive(true);
        //GameController.TogglePause();
        photographer.CameraLensActive = false;
        photographer.canTakePhoto = false;
        Time.timeScale = 0;
        Player.canMove = false;
        Player.canLook = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        pressEscToClose.SetActive(true);
}
    public void HideKeypadAndReset()
    {
        Debug.Log("IT GOES TO HIDEKEYAD");
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
            //photographer.canTakePhoto = true;
        }
        DeselectButtonsinSafe();
        pressEscToClose.SetActive(false);
        darkBackground.SetActive(false);
        //GameController.TogglePause();
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
        //Background.SetActive(false);
        readTime = false;
        //DisableSafes();
        StartCoroutine(WaitToTurnOnCamera());
        StartCoroutine(WaitToBeAbleToPause());
        StartCoroutine(CloseTime());
        //enteredCode1 = enteredCode2 = enteredCode3 = "";
        //totalInputs = 0;
    }

    public IEnumerator WaitToBeAbleToPause()
    {
        yield return new WaitForSecondsRealtime(1);
        uiOpen = false;
    }

    public IEnumerator WaitToTurnOnCamera()
    {
        yield return new WaitForSecondsRealtime(1);
        photographer.canTakePhoto = true;
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
        //gameObject.GetComponent<Safe>().enabled = false;
    }

    public void DetermineNumberPressed(string numberPressed)
    {
        audioSource.PlayOneShot(buttonPressedSFX);

        if (numberPressed == "enter")
        {
            CheckifCodeisCorrect();
        }
        else if (numberPressed != "backspace")
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

    /*
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
    }*/

    public void CheckifCodeisCorrect()
    {
        /*if(ghostSafe)
        {
            if(symbolInputField.text == code)
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
        }*/
        if(inputField.text == code || symbolInputField.text == code)
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
        else
        {
            audioSource.PlayOneShot(incorrectSFX);
            inputField.placeholder.GetComponent<Text>().text = "Incorrect";
            symbolInputField.placeholder.GetComponent<Text>().text = "Incorrect";
            //inputCodeText.text = "";
            inputField.text = "";
            symbolInputField.text = "";
        }
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
