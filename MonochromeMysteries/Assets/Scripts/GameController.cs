/* Name: GameController.cs
 * Author: Zackary Seiple
 * Description: Handles the basic functions of the game including pulling up menus and pausing
 * Last Updated: 2/18/2020 (Zackary Seiple)
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
    public Canvas mainHUD;
    public Camera cam;
    public static GameObject soul;
    //Says whether ANY menu is active
    public static bool menuActive = false;
    public GameObject playerSpawn;

    public PadlockPuzzle padlocks;


    public bool paused = false;

    [Header("Game Stats")]
    public float playTime = 0;
    public float lastSaveTime = 0;

    [Header("Pause Menus")]
    public GameObject tabs;
    private GameObject pauseMenu;
    private GameObject[] pauseMenu_tabs;
    private GameObject[] pauseMenu_menus;
    public enum Menu { Scrapbook, LoadGame, Options, Notepad};
    private Menu pauseMenu_activeMenu;

    [Header("Load Game"), HideInInspector]
    public static GameObject[] saveSlots;
    private static GameObject[] saveSlots_delete;
    private GameObject loadMenu_confirmation;
    private TMP_Text loadMenu_confirmation_message;
    private Button[] loadMenu_options;

    private AudioSource[] audioSources;


    // Start is called before the first frame update
    private void OnEnable()
    {
        SceneManager.sceneLoaded += Begin;

        SaveSystem.OnUpdatedSaveStats += UpdateSaveSlotInfo;
        SaveSystem.OnDeleteSave += DeleteSaveSlotInfo;
    }

    private void OnDisable()
    {
       SceneManager.sceneLoaded -= Begin;

       SaveSystem.OnUpdatedSaveStats -= UpdateSaveSlotInfo;
       SaveSystem.OnDeleteSave -= DeleteSaveSlotInfo;
    }

    private void Awake()
    {
        //_instance = this;
            _instance = this;

    }

    static bool initialLoad = false;
    void Begin(Scene scene, LoadSceneMode loadSceneMode)
    {
        menuActive = false;
        //if (initialLoad == false)
        //{
        //    initialLoad = true;
        //    SaveSystem.Load(SaveSystem.currentSaveSlot);
        //}
        //Assign Menus
        mainHUD = GameObject.Find("HUD").GetComponent<Canvas>();
        pauseMenu = mainHUD.transform.Find("Menu").gameObject;
        tabs = pauseMenu.transform.Find("Tabs").gameObject;
        pauseMenu_tabs = new GameObject[] { tabs.transform.Find("Scrapbook").gameObject,
                                            tabs.transform.Find("LoadGame").gameObject,
                                            tabs.transform.Find("Options").gameObject,
                                            tabs.transform.Find("Notepad").gameObject };
        pauseMenu_menus = new GameObject[4];
        pauseMenu_menus[(int) Menu.Scrapbook] = pauseMenu.transform.Find("PhotoCollection").gameObject;
        pauseMenu_menus[(int) Menu.LoadGame] = pauseMenu.transform.Find("LoadGame").gameObject;
        pauseMenu_menus[(int) Menu.Options] = pauseMenu.transform.Find("Options").gameObject;
        pauseMenu_menus[(int)Menu.Notepad] = pauseMenu.transform.Find("NotepadGroup").gameObject;

        //Add Listeners to tabs;
        pauseMenu_tabs[(int)Menu.Scrapbook].GetComponent<Button>().onClick.AddListener(() => { ChangeMenu(Menu.Scrapbook); });
        pauseMenu_tabs[(int)Menu.LoadGame].GetComponent<Button>().onClick.AddListener(() => { ChangeMenu(Menu.LoadGame); });
        pauseMenu_tabs[(int)Menu.Options].GetComponent<Button>().onClick.AddListener(() => { ChangeMenu(Menu.Options); });
        pauseMenu_tabs[(int)Menu.Notepad].GetComponent<Button>().onClick.AddListener(() => { ChangeMenu(Menu.Notepad); });

        pauseMenu.SetActive(false);

        //Load Menu
        loadMenu_confirmation = pauseMenu_menus[(int)Menu.LoadGame].transform.Find("Confirmation").gameObject;
        loadMenu_confirmation_message = loadMenu_confirmation.transform.Find("Message").GetComponent<TMP_Text>();
        loadMenu_options = new Button[2];
        loadMenu_options[0] = loadMenu_confirmation.transform.Find("Options").Find("Confirm").GetComponent<Button>();
        loadMenu_options[1] = loadMenu_confirmation.transform.Find("Options").Find("Cancel").GetComponent<Button>();

        saveSlots = new GameObject[SaveSystem.MAX_SAVE_SLOTS];
        saveSlots_delete = new GameObject[SaveSystem.MAX_SAVE_SLOTS];
        for(int i = 1; i <= SaveSystem.MAX_SAVE_SLOTS; i++)
        {
            int temp = i;
            saveSlots[i - 1] = pauseMenu_menus[(int)Menu.LoadGame].transform.Find("SaveSlots").Find("Slot " + temp).gameObject;
            saveSlots[i - 1].transform.GetComponent<Button>().onClick.AddListener(() => { if(!loadMenu_confirmation.activeSelf) StartCoroutine(LoadGame(temp)); });
            saveSlots_delete[i - 1] = pauseMenu_menus[(int)Menu.LoadGame].transform.Find("SaveSlots").Find("Delete " + temp).gameObject;
            saveSlots_delete[i - 1].GetComponent<Button>().onClick.AddListener(() => { SaveSystem.DeleteSave(temp); });
            if(!SaveSystem.SaveExists(temp))
                saveSlots_delete[i - 1].SetActive(false);
        }


        cam = Camera.main;
        soul = GameObject.Find("Player");

        
        audioSources = this.GetComponents<AudioSource>();


        if (initialLoad == false)
        {
            initialLoad = true;
            SaveSystem.Load(SaveSystem.currentSaveSlot);
        }


        //Initialize Game
        StartCoroutine(InitializeGame());

    }

    // Update is called once per frame
    /// <summary>
    /// Checks each frame for inputs to open the photograph collection menu and to exit the game
    /// </summary>
    void Update()
    {
        //Open/Close Menu
        if( (Input.GetKeyDown(KeyCode.Tab) && !Readables.isReadingLetter && !padlocks.keypadisUp && (!menuActive || pauseMenu.activeSelf == true)) || (Input.GetKeyDown(KeyCode.Escape) && menuActive && pauseMenu.activeSelf == true))
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
        for(int i = 0; i < pauseMenu_menus.Length; i++)
        {
            if (i == (int) newMenu)
                pauseMenu_menus[i].SetActive(true);
            else
                pauseMenu_menus[i].SetActive(false);
        }
    }

    public static void QuitGame()
    {
        Application.Quit();
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

    /// <summary>
    /// Toggles the Game Pausing (Not the menu)
    /// </summary>
    public static void TogglePause()
    {
        _instance.paused = !_instance.paused;

        if (_instance.paused)
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


    bool waitForInput = false;
    bool confirmationResponse = false;
    public IEnumerator LoadGame(int saveSlot)
    {
        if(!SaveSystem.SaveExists(saveSlot))
        {
            Log.AddEntry("No Save Data in Slot " + saveSlot);
            yield break;
        }

        waitForInput = true;
        loadMenu_confirmation.SetActive(true);

        //Setup confirmation pop-up window
        loadMenu_confirmation_message.text = "Are you sure you'd like to load the game in Slot " + saveSlot + "?";
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

            SaveSystem.Load(saveSlot);
            //SaveSystem.Save(saveSlot);
            if (paused)
                TogglePause();
            pauseMenu.SetActive(false);
            menuActive = false;
            StartCoroutine(InitializeGame());
        }
        
       
    }

    public static bool playerInPlace = false;
    private IEnumerator InitializeGame()
    {
        while (SaveSystem.loading || Player.GetPossessionInProgress())
            yield return null;


        while (MainMenu._instance.TVs.Length == 0)
        {
            yield return null;
        }

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


        soul.transform.position = MainMenu._instance.GetCurrentTV().transform.Find("CamPoint").position;
        soul.transform.rotation = MainMenu._instance.GetCurrentTV().transform.Find("CamPoint").rotation;
        Debug.Log(MainMenu._instance.GetCurrentTV().transform.parent.name);
        playerInPlace = true;

        yield return new WaitForEndOfFrame();

        MainMenu.UpdateTVRanges();
        MainMenu.TriggerMainMenu();
    }

}
