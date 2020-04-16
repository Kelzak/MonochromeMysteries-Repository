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

public class GameController : MonoBehaviour
{
    public static GameController _instance;
    public static Canvas mainHUD;
    public static Camera cam;
    public static GameObject soul;
    //Says whether ANY menu is active
    public static bool menuActive = false;
    public GameObject playerSpawn;

    public bool paused = false;

    public GameObject tabs;
    private GameObject pauseMenu;
    private GameObject[] pauseMenu_tabs;
    private GameObject[] pauseMenu_menus;
    public enum Menu { Scrapbook, LoadGame, Options };
    private Menu pauseMenu_activeMenu;

    private AudioSource[] audioSources;

    // Start is called before the first frame update
    void Start()
    {
        _instance = this;

        //Assign Menus
        mainHUD = GameObject.Find("HUD").GetComponent<Canvas>();
        pauseMenu = mainHUD.transform.Find("Menu").gameObject;
        tabs = pauseMenu.transform.Find("Tabs").gameObject;
        pauseMenu_tabs = new GameObject[] { pauseMenu.transform.Find("Tabs").Find("Scrapbook").gameObject,
                                            pauseMenu.transform.Find("Tabs").Find("LoadGame").gameObject,
                                            pauseMenu.transform.Find("Tabs").Find("Options").gameObject };
        pauseMenu_menus = new GameObject[3];
        pauseMenu_menus[(int) Menu.Scrapbook] = pauseMenu.transform.Find("PhotoCollection").gameObject;
        pauseMenu_menus[(int) Menu.LoadGame] = pauseMenu.transform.Find("LoadGame").gameObject;
        pauseMenu_menus[(int) Menu.Options] = pauseMenu.transform.Find("Options").gameObject;

        //Add Listeners to tabs;
        pauseMenu_tabs[(int)Menu.Scrapbook].GetComponent<Button>().onClick.AddListener(() => { ChangeMenu(Menu.Scrapbook); });
        pauseMenu_tabs[(int)Menu.LoadGame].GetComponent<Button>().onClick.AddListener(() => { ChangeMenu(Menu.LoadGame); });
        pauseMenu_tabs[(int)Menu.Options].GetComponent<Button>().onClick.AddListener(() => { ChangeMenu(Menu.Options); });

        pauseMenu.SetActive(false);

        cam = Camera.main;
        soul = GameObject.Find("Player");
        soul.transform.position = playerSpawn.transform.position;
        //Initialize Game
        StartCoroutine(InitializeGame());
        
        audioSources = this.GetComponents<AudioSource>();
    }



    // Update is called once per frame
    /// <summary>
    /// Checks each frame for inputs to open the photograph collection menu and to exit the game
    /// </summary>
    void Update()
    {
        //Open/Close Menu
        if( (Input.GetKeyDown(KeyCode.Tab) && !Readables.isReadingLetter && (!menuActive || pauseMenu.activeSelf == true)) || (Input.GetKeyDown(KeyCode.Escape) && menuActive && pauseMenu.activeSelf == true))
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

        //Enter/Exit Menu
        if(Input.GetKeyDown(KeyCode.F) || (Input.GetKeyDown(KeyCode.Escape) && menuActive))
        {
            TriggerMainMenu();
        }
        //Just Save
        if(Input.GetKeyDown(KeyCode.X) && MainMenu.IsInRange())
        {
            //Save code
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

    public void TriggerMainMenu()
    {
        if (!MainMenu._instance.tvTransitionInProgress && MainMenu.IsInRange() && (!menuActive || MainMenu.active))
            MainMenu._instance.StartCoroutine(MainMenu._instance.TriggerTV());
    }

    public void QuitGame()
    {
        Application.Quit();
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

    private IEnumerator InitializeGame()
    {
        while(MainMenu._instance.TVs.Length == 0)
        {
            yield return null;
        }

        Television startTV = MainMenu._instance.TVs[0];
        MainMenu._instance.SetCurrentTV(startTV);

        //Vector3 target = transform.forward;
        //target.z += soul.GetComponent<MeshRenderer>().bounds.size.z * 400;
        soul.transform.position = playerSpawn.transform.position;
        soul.transform.rotation = startTV.transform.Find("CamPoint").rotation;

        yield return new WaitForFixedUpdate();

        MainMenu._instance.UpdateTVRanges();
        TriggerMainMenu();
    }

}
