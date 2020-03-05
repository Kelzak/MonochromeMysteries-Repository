using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public static MainMenu _instance;

    private Camera cam;

    [Header("MainMenu")]
    private int lastTvIndex = 0;
    private bool playerInTVRange = false;
    private GameObject currentMenu;
    private Television currentTV;

    public Television[] TVs;

    //Says whether the MAIN MENU ONLY is active
    public static bool active = false;

    private void Awake()
    {
        _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;

        TVs = FindObjectsOfType<Television>();
    }

    // Update is called once per frame
    void Update()
    {
        //TV UPDATE
        //Check each tv to see if player is in range
        UpdateTVRanges();
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

    public void SetCurrentTV(Television set)
    {
        currentTV = set;
    }

    public void UpdateTVRanges()
    {
        playerInTVRange = false;
        for (int i = 0; i < TVs.Length; i++)
        {
            if (TVs[i].CheckForPlayerInRange())
            {
                playerInTVRange = true;
                lastTvIndex = i;
                currentTV = TVs[i];
            }
        }

        if (!playerInTVRange)
            lastTvIndex = -1;
    }

    //Store the player transform to return after player exits the menu
    Transform tempPlayerStorage = null;
    Quaternion tempRotationStorage = Quaternion.identity;
    public bool tvTransitionInProgress = false;
    float tvTransitionTime = 0.75f;
    /// <summary>
    /// Handles the flow into the TV main menu;
    /// </summary>
    /// <returns></returns>
    public IEnumerator TriggerTV()
    {
        tvTransitionInProgress = true;
        Debug.Log("Starting TV Transition");
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
            Player.SetControlsActive(false);
        }
        //Exiting menu so return to static
        else
        {
            currentTV.tvStatic.SetActive(true);
            currentTV.mainMenu.SetActive(false);
        }

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

        cam.transform.parent = targetTransform;

        //Progress toward target transform's position and rotation
        Vector3 startPos = cam.transform.localPosition;
        Quaternion startRot = cam.transform.localRotation;
        float currentTime = 0;
        while (cam.transform.localPosition != Vector3.zero || cam.transform.localRotation != targetRotation && currentTime < tvTransitionTime)
        {
            cam.transform.localPosition = Vector3.Lerp(startPos, Vector3.zero, Mathf.SmoothStep(0f, 1f, currentTime / tvTransitionTime));
            cam.transform.localRotation = Quaternion.Lerp(startRot, targetRotation, Mathf.SmoothStep(0f, 1f, currentTime / tvTransitionTime));
            currentTime += Time.unscaledDeltaTime;
            yield return null;
        }

        //Return player controls if moving out of main menu
        if (!active)
        {
            Player.SetControlsActive(true);
            GameController.menuActive = GameController._instance.paused;
        }
        //Entering menu so turn menu on and static off
        else
        {
            currentTV.tvStatic.SetActive(false);
            currentTV.mainMenu.SetActive(true);
            currentMenu = currentTV.mainMenu;
        }

        Photographer photographer;
        if ((photographer = cam.GetComponentInParent<Photographer>()) && photographer.CameraLensActive == false)
            photographer.CameraLensActive = true;

        if (active == false)
            

        Debug.Log("Ending TV Transition");
        tvTransitionInProgress = false;
    }

    float menuTransitionTime = 0.5f;
    bool menuTransitionInProgress = false;
    private IEnumerator SwitchMenu(string menuNameToSwitchTo)
    {
        menuTransitionInProgress = true;
        if(currentMenu != null)
            currentMenu.SetActive(false);
        currentTV.tvStatic.SetActive(true);

        yield return new WaitForSecondsRealtime(menuTransitionTime);

        currentMenu = currentTV.transform.Find("Screen").Find(menuNameToSwitchTo).gameObject;
        currentMenu.SetActive(true);
        currentTV.tvStatic.SetActive(false);
        menuTransitionInProgress = false;
    }
}
