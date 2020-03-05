/* Name: Player.cs
 * Author: Zackary Seiple
 * Description: This script handles the player's ghost behaviours and their ability to posses 
 * Last Updated: 2/18/2020 (Zackary Seiple)
 * Changes: Added header
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    [HideInInspector]
    public GameObject mainPlayer = null;

    [Header("Aiming")]
    public float lookSensitivity = 5f;
    [HideInInspector]
    public static bool canLook = true;

    [HideInInspector]
    public float lookHorizontal;
    [HideInInspector]
    public float lookVertical;
    [HideInInspector]
    public float verticalClamp = 60;
    [HideInInspector]
    public GameObject cam;

    [Header("Movement")]
    public float moveSpeed = 10f;
    [HideInInspector]
    public static bool canMove = true;

    float xMovement, yMovement;
    CharacterController character;

    [Header("Possession")]
    public float possess_Distance = 3;

    //KEVON'S ADDITION TO CODE//
    bool canPickup;
    bool hasKey;
    public PPSettings ppvToggle;
    public static bool hasCamera;
    public Photographer photographer;

    public Text pickUpInstructions;
    public Text possessionInstructions;
    public Text pictureTakingInstructions;
    public Text itemSpecificInstructions;
    public bool hasPossessedForTheFirstTime;

    private void Awake()
    {
        //ppvToggle.Toggle(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        hasKey = false;
        canPickup = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (mainPlayer == null)
            mainPlayer = gameObject;

        cam = transform.GetChild(0).gameObject;
        character = GetComponent<CharacterController>();
    }



    // Update is called once per frame
    void Update()
    {
        if (canLook)
            Look();
        if (canMove)
            Movement();

        if (!possessionInProgress)
            PossessionCheck();

        if (Input.GetKeyDown(KeyCode.Q) && gameObject != mainPlayer && !possessionInProgress)
            StartCoroutine(ExitPossession());

        if (gameObject.GetComponent<Photographer>())
        {
            ppvToggle.Toggle(false);
        }
        else
        {
            ppvToggle.Toggle(true);
        }

        if (!hasPossessedForTheFirstTime)
        {
            possessionInstructions.gameObject.SetActive(true);
        }
        else
        {
            possessionInstructions.gameObject.SetActive(false);
        }

    }

    //PICKING UP OBJECTS
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Selectable")
        {
            if (other.gameObject.GetComponent<Item>().itemName == "Camera")
            {
                Debug.Log("Text should appear");
                pickUpInstructions.gameObject.SetActive(true);
            }
            canPickup = true;
            Debug.Log("CanPickUp = " + canPickup);

            if (Input.GetKeyDown(KeyCode.F) && canPickup)
            {
                if (gameObject.tag == "Photographer" && other.gameObject.GetComponent<Item>().itemName == "Camera")
                {
                    Debug.Log("Picking up Camera...");
                    hasCamera = true;
                    photographer.ToggleHUD(true);
                    Destroy(other.gameObject);
                    pictureTakingInstructions.gameObject.SetActive(true);
                    pickUpInstructions.gameObject.SetActive(false);
                    itemSpecificInstructions.gameObject.SetActive(false);
                }
                else if (gameObject.tag == "Manager" && other.gameObject.GetComponent<Item>().itemName == "Key")
                {
                    hasKey = true;
                    Destroy(other.gameObject);
                }
            }
        }

        if (other.gameObject.tag == "LockedDoor")
        {
            if (Input.GetKeyDown(KeyCode.F) && hasKey && gameObject.tag == "Manager")
            {
                Debug.Log("You unlocked the door!");
                Destroy(other.gameObject);
                hasKey = false;
                Debug.Log("You dont have the key anymore");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        canPickup = false;
        pickUpInstructions.gameObject.SetActive(false);
    }

    //PUBLIC FUNCTIONS
    public static void EnableControls(bool on)
    {
        canMove = canLook = on;
    }

    public GameObject GetCam()
    {
        return cam;
    }


    //PRIVATE FUNCTIONS
    /// <summary>
    /// Handles the aiming movement with the mouse
    /// </summary>
    private void Look()
    {
        lookHorizontal += Input.GetAxis("Mouse X") * lookSensitivity;
        lookVertical = Mathf.Clamp(lookVertical - Input.GetAxis("Mouse Y") * lookSensitivity, -verticalClamp, verticalClamp);
        transform.localRotation = Quaternion.Euler(0, lookHorizontal, 0);
        cam.transform.localRotation = Quaternion.Euler(lookVertical, 0, 0);
    }

    /// <summary>
    /// Function that can be called, forces the Player GameObject to look at a given GameObject
    /// </summary>
    /// <param name="obj">The GameObject to be looked at</param>
    private void LookAt(GameObject obj)
    {
        Quaternion LookAtRot = Quaternion.LookRotation(obj.transform.position - transform.position);
        transform.rotation = Quaternion.Euler(0, lookHorizontal = LookAtRot.eulerAngles.y, 0);
        cam.transform.localRotation = Quaternion.Euler(LookAtRot.eulerAngles.x, 0, LookAtRot.eulerAngles.z);

    }

    /// <summary>
    /// Handles the movement of the player
    /// </summary>
    private void Movement()
    {
        xMovement = Input.GetAxis("Horizontal");
        yMovement = Input.GetAxis("Vertical");

        Vector3 velocity = (transform.right * xMovement * moveSpeed) + (transform.forward * yMovement * moveSpeed);

        {
            velocity += Physics.gravity;
        }

        character.Move(velocity * Time.deltaTime);
    }

    /// <summary>
    /// Called Every Frame in Update. Checks in front of the player for set distance in order to see if any objects
    /// are possessable and should be highlighted
    /// </summary>
    private void PossessionCheck()
    {
        GameObject target = null;
        float targetDist = 0;
        //Scan area directly in front for targets
        RaycastHit[] hit = Physics.RaycastAll(cam.transform.position, cam.transform.forward, possess_Distance);
        foreach (RaycastHit x in hit)
        {
            //If a possessable target is found and its not the gameObject that this is on
            if (x.collider.gameObject.GetComponent<Possessable>() != null && x.collider.gameObject != gameObject)
            {
                //Set target to first found possessable entity and then stop looking, and only one thing can be targeted
                target = x.collider.gameObject;
                targetDist = x.distance;
            }
        }

        if (target != null) //Check to make sure nothing else got hit before target
        {
            foreach (RaycastHit x in hit)
            {
                if (x.distance < targetDist && x.collider.gameObject != target) //Hit something else first
                {
                    target = null;
                    break;
                }
            }
        }

        //If there are no targets, clear the highlighted one
        if (target == null && Possessable.GetHighlightedObject() != null)
        {
            Possessable.GetHighlightedObject().TriggerHighlight();
        }

        //If target was found and it isn't highlighted, start highlighting it
        if (target != null && !target.GetComponent<Possessable>().IsHighlighted())
            target.GetComponent<Possessable>().TriggerHighlight();

        //If target is still in range and button is pressed, start the possession
        if (Input.GetKeyDown(KeyCode.E) && !possessionInProgress && target != null)
            StartCoroutine(Possess(target));
    }

    bool possessionInProgress = false;
    /// <summary>
    /// Handles the possession transition into another GameObject
    /// </summary>
    /// <param name="target">The target GameObject to possess</param>
    /// <returns></returns>
    private IEnumerator Possess(GameObject target)
    {
        hasPossessedForTheFirstTime = true;
        itemSpecificInstructions.gameObject.SetActive(true);
        possessionInProgress = true;

        //No Target 
        if (target == null)
            yield break;

        //Cam Shift & Alpha fade
        EnableControls(false);
        LookAt(target);

        //VARIABLES
        float minFOV = 5, maxFOV;
        float minAlpha = 0, maxAlpha = 1;
        float transitionTime = 0.5f, currentTime = 0;

        Camera camComp = cam.GetComponent<Camera>();
        maxFOV = camComp.fieldOfView;
        Material mat = target.GetComponent<MeshRenderer>().material;

        //Zoom in
        while (camComp.fieldOfView > minFOV && mat.color.a > minAlpha)
        {
            camComp.fieldOfView = Mathf.Lerp(maxFOV, minFOV, currentTime / transitionTime);
            Color goalColor = mat.color;
            goalColor.a = Mathf.Lerp(maxAlpha, minAlpha, currentTime / transitionTime);
            mat.color = goalColor;
            cam.transform.position = Vector3.Lerp(gameObject.transform.position, target.transform.position, currentTime / transitionTime);
            currentTime += Time.deltaTime;
            yield return null;
        }

        currentTime = 0;
        target.transform.rotation = cam.transform.rotation;
        cam.transform.SetParent(target.transform);
        cam.transform.localPosition = Vector3.zero;
        //Get Rid of Effects of currently Possessed Objects
        if (gameObject != mainPlayer)
            gameObject.GetComponent<Possessable>().TriggerOnPossession(false);
        //Start Effect of What is Being Possessed
        target.GetComponent<Possessable>().TriggerOnPossession(true);

        //Zoom out
        while (camComp.fieldOfView < maxFOV)
        {
            camComp.fieldOfView = Mathf.Lerp(minFOV, maxFOV, currentTime / (transitionTime / 4f));
            Color goalColor = mat.color;
            goalColor.a = Mathf.Lerp(minAlpha, maxAlpha, currentTime / (transitionTime / 4f));
            mat.color = goalColor;
            currentTime += Time.deltaTime;
            yield return null;
        }

        EnableControls(true);
        //End Camera Shift & Alpha Fade

        //Copy Player Script and all its public fields
        System.Type type = this.GetType();
        Component copy = target.AddComponent(type);
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(this));
        }



        //If it's the main player (Ghost) then make it "disappear"
        if (gameObject == mainPlayer)
            gameObject.SetActive(false);
        //If it's not the main player, remove player script
        else if (gameObject != mainPlayer)
        {

            Destroy(GetComponent<Player>());
        }

        possessionInProgress = false;
    }

    /// <summary>
    /// Handles the exiting of a possessed Object
    /// </summary>
    /// <returns></returns>
    private IEnumerator ExitPossession()
    {
        possessionInProgress = true;

        GetComponent<Possessable>().TriggerHighlight();

        //Find a point with enough space to exit to once possession ends
        Vector3 exitPoint = gameObject.transform.position;

        float mainPlayerMaxExtents = Mathf.Max(mainPlayer.GetComponent<MeshRenderer>().bounds.extents.x, mainPlayer.GetComponent<MeshRenderer>().bounds.extents.z);
        float thisMaxExtents = Mathf.Max(gameObject.GetComponent<MeshRenderer>().bounds.extents.x, gameObject.GetComponent<MeshRenderer>().bounds.extents.z);
        float checkRadius = thisMaxExtents + mainPlayerMaxExtents * 2;
        bool safeExitPoint = false;

        int[] multiplierOptions = { 0, 1, -1 };
        Vector3 temp = exitPoint;
        for (int i = 0; i < multiplierOptions.Length && safeExitPoint == false; i++) //Diagonals
        {
            for (int j = multiplierOptions.Length - 1; j > 0 && safeExitPoint == false; j--) //Cardinal Directions
            {
                //temp = new Vector3(exitPoint.x + checkRadius * multiplierOptions[i], exitPoint.y, exitPoint.z + checkRadius * multiplierOptions[j]);
                temp = transform.position + (transform.forward * checkRadius * multiplierOptions[j]) + (transform.right * checkRadius * multiplierOptions[i]);
                Collider[] hit = Physics.OverlapSphere(temp, mainPlayerMaxExtents);
                safeExitPoint = true;
                foreach (Collider x in hit)
                {
                    if (x.gameObject.tag != "Floor")
                        safeExitPoint = false;
                }
            }
        }

        if (safeExitPoint) //Safe exit point with no collisions found
        {
            exitPoint = temp;
        }
        else //No possible exit points found, can't unpossess here
        {
            Debug.Log("Player.cs: Cannot Unpossess Here");
            possessionInProgress = false;
            yield break;
        }

        //START TRANSITION TO EXIT POSSESSION
        EnableControls(false);

        //Camera Variables
        Camera camComp = cam.GetComponent<Camera>();
        float maxFOV = 120;
        float minFOV = camComp.fieldOfView;
        float transitionTime = 0.5f;
        float currentTime = 0;



        //Reactivate the Player, 
        mainPlayer.SetActive(true);
        mainPlayer.GetComponent<Player>().possessionInProgress = true;
        mainPlayer.transform.position = exitPoint;
        mainPlayer.transform.rotation = gameObject.transform.rotation;

        if (gameObject != mainPlayer)
            gameObject.GetComponent<Possessable>().TriggerOnPossession(false);

        //Zoom out
        while (camComp.fieldOfView < maxFOV)
        {
            camComp.fieldOfView = Mathf.Lerp(minFOV, maxFOV, currentTime / transitionTime);
            cam.transform.position = Vector3.Lerp(transform.position, mainPlayer.transform.position, currentTime / transitionTime);

            currentTime += Time.deltaTime;
            yield return null;
        }


        cam.transform.position = mainPlayer.transform.position;
        cam.transform.SetParent(mainPlayer.transform);

        currentTime = 0;

        //Zoom in
        while (camComp.fieldOfView > minFOV)
        {
            camComp.fieldOfView = Mathf.Lerp(maxFOV, minFOV, currentTime / transitionTime);
            currentTime += Time.deltaTime;
            yield return null;
        }

        //Make sure the player is looking in the same direction as they were in their body so that the transition isn't jarring
        mainPlayer.GetComponent<Player>().lookHorizontal = this.lookHorizontal;
        mainPlayer.GetComponent<Player>().lookVertical = this.lookVertical;

        //Transition complete, return control to player
        EnableControls(true);
        mainPlayer.GetComponent<Player>().possessionInProgress = false;

        //If this is not the main player (Ghost) then fire any events related to leaving a host and get rid of player script
        if (gameObject != mainPlayer)
        {
            Destroy(GetComponent<Player>());
        }

        possessionInProgress = false;
    }
}
