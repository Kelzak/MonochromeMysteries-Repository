/* Name: GameController.cs
 * Primary Author: Zackary Seiple - Initialization for the game, Scrapbook screen, and Tabs at the top for switching menus
 * Contributors: Kevon Long - Notepad and References Screen
 * Description: Handles the basic functions of the game including pulling up menus, pausing, and initial setup for the game
 * Last Updated: 5/6/2020 (Zackary Seiple)
 * Changes: Added Header
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController _instance = null;
    public static GameObject gameManagement;
    public RatTrap[] ratTraps;
    public DoorScript[] doors;

    public Canvas mainHUD;
    public Camera cam;
    public static GameObject soul;
    //Says whether ANY menu is active
    public static bool menuActive = false;
    public GameObject playerSpawn;

    public PadlockPuzzle padlocks;


    public static bool paused = false;

    [Header("Game Stats")]
    public float playTime = 0;
    public float lastSaveTime = 0;

    [Header("Pause Menus")]
    public GameObject tabs;
    private GameObject pauseMenu;
    private GameObject[] pauseMenu_tabs;
    private GameObject[] pauseMenu_menus;
    public enum Menu { Scrapbook, LoadGame, /*Options,*/ Notepad, Reference, Settings, QuitGame};
    private Menu pauseMenu_activeMenu;

    [Header("Load Game"), HideInInspector]
    public static GameObject[] saveSlots;
    private static GameObject[] saveSlots_delete;
    private GameObject loadMenu_confirmation;
    private TMP_Text loadMenu_confirmation_message;
    private Button[] loadMenu_options;

    private AudioSource[] audioSources;

    public GameObject NotepadPage1;
    public GameObject NotepadPage2;
    public GameObject NotepadPage3;
    public GameObject NotepadPage4;


    // Start is called before the first frame update
    private void OnEnable()
    {

        SaveSystem.OnUpdatedSaveStats += UpdateSaveSlotInfo;
        SaveSystem.OnDeleteSave += DeleteSaveSlotInfo;
    }

    private void OnDisable()
    {

       SaveSystem.OnUpdatedSaveStats -= UpdateSaveSlotInfo;
       SaveSystem.OnDeleteSave -= DeleteSaveSlotInfo;
    }

    private void Start()
    {
        NotepadPage1 = pauseMenu.transform.Find("NotepadGroup").transform.Find("Notes").transform.Find("NotesInputField1").gameObject;
        NotepadPage2 = pauseMenu.transform.Find("NotepadGroup").transform.Find("Notes").transform.Find("NotesInputField2").gameObject;
        NotepadPage3 = pauseMenu.transform.Find("NotepadGroup").transform.Find("Notes").transform.Find("NotesInputField3").gameObject;
        NotepadPage4 = pauseMenu.transform.Find("NotepadGroup").transform.Find("Notes").transform.Find("NotesInputField4").gameObject;

        
        if (SaveSystem.gameData != null)
        {
            Debug.Log("Running Start: SAVE DATA EXISTS");
            //LOAD RAT TRAPS
            for (int i = 0; i < GameController._instance.ratTraps.Length; i++)
            {
                GameController._instance.ratTraps[i].Load(SaveSystem.gameData.trapData);
            }

            //DOORS
            for (int i = 0; i < GameController._instance.doors.Length; i++)
            {
                GameController._instance.doors[i].Load(SaveSystem.gameData.doorData);
            }

            //Load General
            _instance.playTime = SaveSystem.gameData.gameStats.playTime;
            if (paused)
                GameController.TogglePause();
        }

        //Initialize Game
        StartCoroutine(InitializeGame());
    }

    public static bool initialLoad = true;
    private void Awake()
    {
        Debug.Log("Running Awake");
        _instance = this;
        if (paused)
            TogglePause();
        if (initialLoad == true)
        {
            SaveSystem.Load(SaveSystem.currentSaveSlot, true);
            initialLoad = false;
        }
        menuActive = false;

        mainHUD = GameObject.Find("HUD").GetComponent<Canvas>();
        pauseMenu = mainHUD.transform.Find("Menu").gameObject;
        tabs = pauseMenu.transform.Find("Tabs").gameObject;
        pauseMenu_tabs = new GameObject[] { tabs.transform.Find("Scrapbook").gameObject,
                                            tabs.transform.Find("LoadGame").gameObject,
                                            //tabs.transform.Find("Options").gameObject,
                                            tabs.transform.Find("Notepad").gameObject,
                                            tabs.transform.Find("References").gameObject,
                                            tabs.transform.Find("Settings").gameObject,
                                            tabs.transform.Find("QuitGame").gameObject
                                           };
        pauseMenu_menus = new GameObject[6];
        pauseMenu_menus[(int)Menu.Settings] = pauseMenu.transform.Find("SettingsGroup").gameObject;
        pauseMenu_menus[(int)Menu.Scrapbook] = pauseMenu.transform.Find("PhotoCollection").gameObject;
        pauseMenu_menus[(int)Menu.LoadGame] = pauseMenu.transform.Find("LoadGame").gameObject;
        pauseMenu_menus[(int)Menu.QuitGame] = pauseMenu.transform.Find("QuitGameGroup").gameObject;
        pauseMenu_menus[(int)Menu.Notepad] = pauseMenu.transform.Find("NotepadGroup").gameObject;
        pauseMenu_menus[(int)Menu.Reference] = pauseMenu.transform.Find("ReferenceGroup").gameObject;



        //Add Listeners to tabs;
        pauseMenu_tabs[(int)Menu.Settings].GetComponent<Button>().onClick.AddListener(() => { ChangeMenu(Menu.Settings); });
        pauseMenu_tabs[(int)Menu.Scrapbook].GetComponent<Button>().onClick.AddListener(() => { ChangeMenu(Menu.Scrapbook); });
        pauseMenu_tabs[(int)Menu.LoadGame].GetComponent<Button>().onClick.AddListener(() => { ChangeMenu(Menu.LoadGame); });
        //pauseMenu_tabs[(int)Menu.Options].GetComponent<Button>().onClick.AddListener(() => { ChangeMenu(Menu.Options); });
        pauseMenu_tabs[(int)Menu.Notepad].GetComponent<Button>().onClick.AddListener(() => { ChangeMenu(Menu.Notepad); });
        pauseMenu_tabs[(int)Menu.Reference].GetComponent<Button>().onClick.AddListener(() => { ChangeMenu(Menu.Reference); });
        pauseMenu_tabs[(int)Menu.QuitGame].GetComponent<Button>().onClick.AddListener(() => { ChangeMenu(Menu.QuitGame); });
        


        pauseMenu.SetActive(false);

        //Load Menu
        loadMenu_confirmation = pauseMenu_menus[(int)Menu.LoadGame].transform.Find("Confirmation").gameObject;
        loadMenu_confirmation_message = loadMenu_confirmation.transform.Find("Message").GetComponent<TMP_Text>();
        loadMenu_options = new Button[2];
        loadMenu_options[0] = loadMenu_confirmation.transform.Find("Options").Find("Confirm").GetComponent<Button>();
        loadMenu_options[1] = loadMenu_confirmation.transform.Find("Options").Find("Cancel").GetComponent<Button>();

        saveSlots = new GameObject[SaveSystem.MAX_SAVE_SLOTS];
        saveSlots_delete = new GameObject[SaveSystem.MAX_SAVE_SLOTS];
        for (int i = 1; i <= SaveSystem.MAX_SAVE_SLOTS; i++)
        {
            int temp = i;
            saveSlots[i - 1] = pauseMenu_menus[(int)Menu.LoadGame].transform.Find("SaveSlots").Find("Slot " + temp).gameObject;
            saveSlots[i - 1].transform.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (!loadMenu_confirmation.activeSelf)
                    StartCoroutine(Confirmation(temp,
                                                string.Format("Are you sure you want to load data in Slot {0}", temp),
                                                () =>
                                                {
                                                    if (paused)
                                                        TogglePause();
                                                    pauseMenu.SetActive(false);
                                                    menuActive = false;
                                                    SaveSystem.Load(temp);
                                                    //SaveSystem.Save(saveSlot);
                                                    //StartCoroutine(InitializeGame());
                                                }));

            });
            saveSlots_delete[i - 1] = pauseMenu_menus[(int)Menu.LoadGame].transform.Find("SaveSlots").Find("Delete " + temp).gameObject;
            saveSlots_delete[i - 1].GetComponent<Button>().onClick.AddListener(() =>
            {
                if (!loadMenu_confirmation.activeSelf)
                    StartCoroutine(Confirmation(temp,
                                                string.Format("Are you sure you want to delete ALL save data in Slot {0}", temp),
                                                () => { SaveSystem.DeleteSave(temp); }));

            });
            if (!SaveSystem.SaveExists(temp))
                saveSlots_delete[i - 1].SetActive(false);
        }

        //General Assignments for player and camera
        cam = Camera.main;
        soul = GameObject.Find("Player");
        Player.ResetStaticVariables();

        //Keep Track of Rat Traps
        ratTraps = GameObject.FindObjectsOfType(typeof(RatTrap)) as RatTrap[];
        System.Array.Sort(ratTraps, (RatTrap x, RatTrap y) => { return x.GetID().CompareTo(y.GetID()); });

        //Keep Track of Doors
        doors = GameObject.FindObjectsOfType(typeof(DoorScript)) as DoorScript[];
        System.Array.Sort(doors, (DoorScript x, DoorScript y) => { return x.GetID().CompareTo(y.GetID()); });

        audioSources = this.GetComponents<AudioSource>();
    }

    // Update is called once per frame
    /// <summary>
    /// Checks each frame for inputs to open the photograph collection menu and to exit the game
    /// </summary>
    void Update()
    {
        //Open/Close Menu
        if( ( (Input.GetKeyDown(KeyCode.Tab)) && !Read.isReading && !PadlockPuzzle.keypadisUp && !Endings.isUsingKnife && (!menuActive || pauseMenu.activeSelf == true)) || (Input.GetKeyDown(KeyCode.Tab) && menuActive && pauseMenu.activeSelf == true))
        {
            TogglePause();
            pauseMenu.SetActive(paused);
            //sounds for book
            if(!menuActive)
            {
                //open
                audioSources[0].Play();
            }
            else
            {
                //close
                audioSources[1].Play();

                //Tutorial first close scrapbook
                if(Dialogue.holding)
                {
                    Tutorial.instance.OnFirstCloseScrapbook();
                }
            }
            menuActive = paused;
        }

    }

    public void ChangeMenu(Menu newMenu)
    {
        for(int i = 0; i < pauseMenu_menus.Length + 1; i++)
        {
            if (i == (int)newMenu)
            {
                Debug.Log(i + "  is active");
                pauseMenu_menus[i].SetActive(true);
            }
            else
            {
                Debug.Log(i + "  is inactive");
                pauseMenu_menus[i].SetActive(false);
            }
                
        }
    }

    public static void QuitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Toggles the Game Pausing (Not the menu)
    /// </summary>
    public static void TogglePause()
    {
        paused = !paused;

        if (paused)
        { //Paused
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Player.EnableControls(false);
            Time.timeScale = 0;
        }
        else
        { //Unpaused
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Player.EnableControls(true);
            Time.timeScale = 1;
        }
    }



    private static void UpdateSaveSlotInfo(int saveSlot, string newDate, float newPlayTime)
    {
        _instance.playTime = newPlayTime;
        saveSlots[saveSlot - 1].transform.Find("SaveStats").Find("Date").GetComponent<TMP_Text>().text = "Date: " + newDate;

        int hours, minutes , seconds;
        newPlayTime -= (hours = (int)(newPlayTime / 3600)) * 3600;
        newPlayTime -= (minutes = (int)(newPlayTime / 60)) * 60;
        seconds = (int) newPlayTime;

        saveSlots[saveSlot - 1].transform.Find("SaveStats").Find("PlayTime").GetComponent<TMP_Text>().text = string.Format("Playtime: {0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
        saveSlots_delete[saveSlot - 1].SetActive(true);
    }

    private static void DeleteSaveSlotInfo(int saveSlot)
    {
        saveSlots[saveSlot - 1].transform.Find("SaveStats").Find("Date").GetComponent<TMP_Text>().text = "Date: N/A";
        saveSlots[saveSlot - 1].transform.Find("SaveStats").Find("PlayTime").GetComponent<TMP_Text>().text = "Playtime: --:--:--";
        saveSlots_delete[saveSlot - 1].SetActive(false);
    }


    bool waitForInput = false;
    bool confirmationResponse = false;
    private IEnumerator Confirmation(int saveSlot, string message, UnityEngine.Events.UnityAction action)
    {
        if (!SaveSystem.SaveExists(saveSlot))
        {
            Log.AddEntry("No Save Data in Slot " + saveSlot);
            yield break;
        }

        waitForInput = true;
        loadMenu_confirmation.SetActive(true);

        //Setup confirmation pop-up window
        loadMenu_confirmation_message.text = message;
        loadMenu_options[0].GetComponentInChildren<TMP_Text>().text = "Load Save Slot " + saveSlot;

        UnityEngine.Events.UnityAction confirm = () => { waitForInput = false; confirmationResponse = true; };
        UnityEngine.Events.UnityAction cancel = () => { waitForInput = false; confirmationResponse = false; };
        loadMenu_options[0].onClick.AddListener(confirm);
        loadMenu_options[1].onClick.AddListener(cancel);

        while (waitForInput)
            yield return null;

        //Clear confirmation listeners
        loadMenu_confirmation.SetActive(false);
        loadMenu_options[0].onClick.RemoveListener(confirm);
        loadMenu_options[1].onClick.RemoveListener(cancel);


        if (confirmationResponse == true)
        {
            while (Player.GetPossessionInProgress())
                yield return null;

            action.Invoke();
        }
    }

    public static bool initialTVTransition = true;
    private IEnumerator InitializeGame()
    {
        Debug.Log("Running Initialize");

        while (MainMenu._instance.TVs.Length == 0 || SaveSystem.loading || Player.GetPossessionInProgress() || !MainMenu._instance.loadComplete)
        {
            Debug.Log("TVS: " + MainMenu._instance.TVs.Length + " | Loading: " + SaveSystem.loading + " | PossessionInProgress: " + Player.GetPossessionInProgress() + " | MainmenuLoad: " + (MainMenu._instance.loadComplete == false));
            yield return null;
        }
        Debug.Log("!SaveExists: " + !SaveSystem.SaveExists(SaveSystem.currentSaveSlot) + " | CurrentTV: " + MainMenu._instance.GetCurrentTV() == null);
        if (!SaveSystem.SaveExists(SaveSystem.currentSaveSlot) || MainMenu._instance.GetCurrentTV() == null)
        {
            Debug.Log("Running initial tv settup");
            float shortestDist = Mathf.Infinity;
            int shortestIndex = -1;
            for (int i = 0; i < MainMenu._instance.TVs.Length; i++)
            {
                float temp;
                if ((temp = Vector3.Distance(MainMenu._instance.TVs[i].transform.Find("CamPoint").position, playerSpawn.transform.position)) < shortestDist)
                {
                    shortestDist = temp;
                    shortestIndex = i;
                }
            }
            Television startTV = MainMenu._instance.TVs[shortestIndex];
            MainMenu._instance.SetCurrentTV(startTV);

            //Vector3 target = transform.forward;
        }

        Debug.Log("CanLook: " + Player.canLook + " | CanMove: " + Player.canMove + " | Paused: " + GameController.paused);

        soul.transform.position = MainMenu._instance.GetCurrentTV().transform.Find("CamPoint").position;
        soul.transform.rotation = MainMenu._instance.GetCurrentTV().transform.Find("CamPoint").rotation;
        Debug.Log("Player Moved to Spawn");

        yield return new WaitForFixedUpdate();

        Debug.Log("Updating ranges");
        MainMenu.UpdateTVRanges();

        Debug.Log("Trying to enter tv");
        if (SaveSystem.enterTVOnThisLoad == true)
        {
            MainMenu.TriggerMainMenu();
        }
        else
        {
            SaveSystem.Save(SaveSystem.currentSaveSlot);
        }
    }

    public void ChangeNotepad(int pageNumber)
    {
        if(pageNumber == 1)
        {
            NotepadPage1.SetActive(true);
            NotepadPage2.SetActive(false);
            NotepadPage3.SetActive(false);
            NotepadPage4.SetActive(false);
        }
        if (pageNumber == 2)
        {
            NotepadPage1.SetActive(false);
            NotepadPage2.SetActive(true);
            NotepadPage3.SetActive(false);
            NotepadPage4.SetActive(false);
        }
        if (pageNumber == 3)
        {
            NotepadPage1.SetActive(false);
            NotepadPage2.SetActive(false);
            NotepadPage3.SetActive(true);
            NotepadPage4.SetActive(false);
        }
        if (pageNumber == 4)
        {
            NotepadPage1.SetActive(false);
            NotepadPage2.SetActive(false);
            NotepadPage3.SetActive(false);
            NotepadPage4.SetActive(true);
        }
    }

}
