﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public static MainMenu _instance;

    private Camera cam;

    [Header("MainMenu")]
    private int lastTvIndex = 0;
    private bool playerInTVRange = false;
    private GameObject currentMenu;
    private Television currentTV;

    public delegate void MainMenuEvent(bool isActive);
    public static event MainMenuEvent OnMainMenuTriggered;

    public Television[] TVs;

    public GameObject playerPortrait;


    //Says whether the MAIN MENU ONLY is active
    public static bool active = false;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += Begin;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= Begin;
    }

    private void Awake()
    {
            _instance = this;
            //DontDestroyOnLoad(_instance.transform.parent.gameObject);

    }

    bool initialized = false;
    // Start is called before the first frame update
    void Begin(Scene scene, LoadSceneMode loadSceneMode)
    {
        Begin();
    }

    void Begin()
    {
        active = false;
        cam = Camera.main;

        TVs = FindObjectsOfType<Television>();

        playerPortrait = GameObject.Find("CharacterPortrait");
        initialized = true;
    }

    int frameCount = 0, triggerFrame = 20;
    // Update is called once per frame
    void Update()
    {
        //TV UPDATE
        //Check each tv to see if player is in range
        if (frameCount % triggerFrame == 0)
            UpdateTVRanges();
        else if (frameCount % triggerFrame == triggerFrame)
            frameCount = 0;
        frameCount++;

        //Enter/Exit Main Menu
        if (Input.GetKeyDown(KeyCode.F) || (Input.GetKeyDown(KeyCode.Escape) && MainMenu.GetCurrentMenu() == "MainMenu" && GameController.menuActive))
        {
            TriggerMainMenu();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && GameController.menuActive)
        {
            TriggerSwitchMenu("MainMenu");
        }

        //Just Save
        if (Input.GetKeyDown(KeyCode.X) && MainMenu.IsInRange())
        {
            //Save code
        }
    }

    public static int GetLastTVIndex()
    {
        return _instance.lastTvIndex;
    }

    public static string GetCurrentMenu()
    {
        if(_instance.currentMenu)
            return _instance.currentMenu.name;
        return "";
    }

    public static void TriggerMainMenu()
    {
        if (!MainMenu._instance.tvTransitionInProgress && MainMenu.IsInRange() && (!GameController.menuActive || MainMenu.active))
        {
            MainMenu._instance.StartCoroutine(MainMenu._instance.TriggerTV());
        }
        else
        {
            Debug.Log("tvTransitition: " + !MainMenu._instance.tvTransitionInProgress + " | In Range: " + IsInRange() + " | Etc: " + (!GameController.menuActive || MainMenu.active));

        }
    }

    public void TriggerSwitchMenu(string menu)
    {
        if (!menuTransitionInProgress)
            StartCoroutine(SwitchMenu(menu));
    }

    public static bool IsInRange()
    {
        return _instance.playerInTVRange;
    }

    public static void ChangeFromInitialOptions()
    {
        foreach(Television instance in _instance.TVs)
        {
            //Turn new game button off and resume on
            instance.SwapButtons(false, Television.ButtonName.Continue);
            instance.SwapButtons(true, Television.ButtonName.Resume);
        }
    }

    public void SetCurrentTV(Television set)
    {
        currentTV = set;
    }

    public Television GetCurrentTV()
    {
        return currentTV;
    }

    public static void UpdateTVRanges()
    {
        _instance.playerInTVRange = false;
        for (int i = 0; i < _instance.TVs.Length; i++)
        {
            if (_instance.TVs[i].CheckForPlayerInRange())
            {
                _instance.playerInTVRange = true;
                _instance.lastTvIndex = i;
                _instance.currentTV = _instance.TVs[i];
            }
        }

        if (!_instance.playerInTVRange)
            _instance.lastTvIndex = -1;
    }

    //Store the player transform to return after player exits the menu
    Transform tempPlayerStorage = null;
    Quaternion tempRotationStorage = Quaternion.identity;
    public bool tvTransitionInProgress;
    float tvTransitionTime = 0.75f;
    /// <summary>
    /// Handles the flow into the TV main menu;
    /// </summary>
    /// <returns></returns>
    public IEnumerator TriggerTV()
    {

        tvTransitionInProgress = true;
        Player.EnableControls(false);

        while (Player.GetPossessionInProgress())
            yield return null;

        //Changes 'paused' to true if this pauses the game, false if the game was already paused and is now unpausing
        GameController.TogglePause();
        //If true, this function handles the transition INTO the main menu, if false, this function handles the exit
        active = GameController._instance.paused;
        //This variable is static and lets the game know that there is now a menu up (So another menu shouldn't be able to pop up on top of it)
        if(active == true)
        GameController.menuActive = GameController._instance.paused;
        Transform targetTransform;
        Quaternion targetRotation;

        //Take away player controls if moving into main menu
        if (active)
        {
            Player.EnableControls(false);
            OnMainMenuTriggered?.Invoke(true);
        }
        //Exiting menu so return to static
        else
        {
            currentTV.tvStatic.SetActive(true);
            currentTV.mainMenu.SetActive(false);
            currentTV.saveSelect.SetActive(false);
            currentTV.howToPlay.SetActive(false);
            OnMainMenuTriggered?.Invoke(false);
        }

        Debug.Log("Cam is in " + cam.transform.parent.name);
        //Transitioning into TV, store player transform so camera can be returned properly
        if (cam.GetComponentInParent<Player>())
        {
            tempPlayerStorage = cam.transform.parent;
            tempRotationStorage = cam.transform.localRotation;

            targetTransform = currentTV.transform.Find("CamPoint");
            targetRotation = Quaternion.identity;

            Photographer photographerX;
            if((photographerX = cam.GetComponentInParent<Photographer>()) && photographerX.CameraLensActive)
            {
                photographerX.CameraLensActive = false;
            }
        }
        //Transitioning out of TV back into player
        else
        {
            targetTransform = tempPlayerStorage;
            targetRotation = tempRotationStorage;
        }

        var tempRotation = cam.transform.rotation;
        cam.transform.parent = targetTransform;
        cam.transform.rotation = tempRotation;


        //Progress toward target transform's position and rotation
        Vector3 startPos = cam.transform.localPosition;
        Debug.Log(targetTransform.name);
        Vector3 targetPos = targetTransform.GetComponent<Player>() ? Vector3.zero + targetTransform.GetComponent<Player>().camOffset : Vector3.zero;
        Quaternion startRot = cam.transform.localRotation;

        float currentTime = 0;

        while (currentTime <= tvTransitionTime)
        {
            currentTime += Time.unscaledDeltaTime;
            cam.transform.localPosition = Vector3.Lerp(startPos, targetPos, Mathf.SmoothStep(0f, 1f, currentTime / tvTransitionTime));
            cam.transform.localRotation = Quaternion.Lerp(startRot, targetRotation, Mathf.SmoothStep(0f, 1f, currentTime / tvTransitionTime));
            yield return null;
        }

        //Return player controls if moving out of main menu AFTER TRANSITION
        if (!active)
        {
            Player.EnableControls(true);
            GameController.menuActive = GameController._instance.paused;
        }
        //Entering menu so turn menu on and static off
        else
        {
            currentTV.tvStatic.SetActive(false);
            currentTV.mainMenu.SetActive(true);
            currentMenu = currentTV.mainMenu;

            if (Time.timeSinceLevelLoad > 5)
                SaveSystem.Save(SaveSystem.currentSaveSlot);
        }

        Photographer photographer;
        if ((photographer = cam.GetComponentInParent<Photographer>()) && photographer.CameraLensActive == false)
        {
            photographer.CameraLensActive = true;
        }

        tvTransitionInProgress = false;
    }

    float menuTransitionTime = 0.5f;
    bool menuTransitionInProgress = false;
    private IEnumerator SwitchMenu(string menuNameToSwitchTo)
    {
        menuTransitionInProgress = true;
        if(currentMenu != null)
            currentMenu.SetActive(false);

        while (currentTV == null)
        {
            yield return null;
        }

        currentTV.tvStatic.SetActive(true);

        yield return new WaitForSecondsRealtime(menuTransitionTime);

        currentMenu = currentTV.transform.Find("Screen").Find(menuNameToSwitchTo).gameObject;
        currentMenu.SetActive(true);
        currentTV.tvStatic.SetActive(false);
        menuTransitionInProgress = false;
    }

    public static void TriggerLoad(Data.MainMenuData mainMenuData)
    {
        _instance.StartCoroutine(_instance.Load(mainMenuData));
    }

    public IEnumerator Load(Data.MainMenuData mainMenuData)
    {
        while (!_instance.initialized)
        {
            yield return null;
        }

        var tvPos = new Vector3(mainMenuData.currentTV_pos[0], mainMenuData.currentTV_pos[1], mainMenuData.currentTV_pos[2]);
        Collider[] hit = Physics.OverlapSphere(tvPos, 1f);
        Television result = null;
        foreach (Collider x in hit)
        {
            if (x.GetComponent<Television>())
                result = x.GetComponent<Television>();

        }
        if (result.screen != null)
        {
            _instance.SetCurrentTV(result);
            GameController._instance.playerSpawn.transform.position = result.transform.Find("CamPoint").position;
            GameController._instance.playerSpawn.transform.rotation = result.transform.Find("CamPoint").rotation;
        }
    }

    private Television GetDefaultTV()
    {
        return null;
    }
}
