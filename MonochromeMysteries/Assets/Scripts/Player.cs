/* Name: Player.cs
 * Author: Zackary Seiple
 * Description: This script handles the player's ghost behaviours and their ability to posses 
 * Last Updated: 2/18/2020 (Zackary Seiple)
 * Changes: Added header
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    private AudioSource[] audioSources;
    private AudioSource audioSource;

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
    public Vector3 camOffset;

    //KEVON'S ADDITION TO CODE//
    bool canPickup;
    public bool hasKey = false;
    public PPSettings ppvToggle;
    public static bool hasCamera;
    public Photographer photographer;
    public static bool isAtTheFirstSafe;
    public static bool isAtTheSecondSafe;
    public static bool isAtTheThirdSafe;
    
    public bool hasPossessedForTheFirstTime;

    //UI Images and Texts
    public Sprite ghostImage;
    public Text ghostName;
    public Sprite photographerImage;
    public Sprite ratImage;
    public Sprite managerImage;
    public Sprite mechanicImage;
    public Sprite exterminatorImage;
    public Sprite hunterImage;
    public Image characterImage;
    public Text characterName;
    public Text characterRole;
    public static string characterRoleForEnding;
    public Sprite cameraImage;
    public Image itemImage;
    public Text itemName;
    public GameObject keyImage;
    public Image reticle;
    public static float reticleDist = 7;
    public AudioClip obtainClip;
    public AudioClip possessClip;
    public AudioClip depossessClip;


    //sound stuff
    public StateChecker stateChecker;
    private AudioClip step;
    public bool isFemale;
    public AudioClip[] maleSteps;
    public AudioClip[] femaleSteps;
    public AudioClip[] indoorSteps;
    public AudioClip[] ratSteps;
    public float stepVolume;
    public float walkSoundInterval = .5f;
    private bool isRat;

    //pick up stuff
    public static List<GameObject> keys = new List<GameObject>();

    //Puzzle Stuff
    public PadlockPuzzle safeManager;
    Ray safeCheck;
    public bool isLookingAtSafe1;
    public bool isLookingAtSafe2;
    public bool isLookingAtSafe3;
    public static string safeName;
    
    //Ending Stuff
    public static bool hasKnife;
    public Endings endingManager;

    //Reading letters stuff
    public Readables readables;
    

    private void Awake()
    {
        //ppvToggle.Toggle(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        canPickup = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        MainMenu.onMainMenuTriggered += HideReticle;

        camOffset = new Vector3(0, 0.25f, 0);

        if (mainPlayer == null)
            mainPlayer = gameObject;

        if (cam == null)
            cam = transform.Find("Main Camera").gameObject;
        character = GetComponent<CharacterController>();

        InvokeRepeating("WalkAudio", 0f, walkSoundInterval);
    }



    // Update is called once per frame
    void Update()
    {
        audioSource = GetComponent<AudioSource>();
        if (canLook)
            Look();
        if (canMove)
            Movement();

        if (!possessionInProgress)
            PossessionCheck();

        if (Input.GetKeyDown(KeyCode.Q) && gameObject != mainPlayer && !possessionInProgress)
        {
            canPickup = false;
            audioSource.PlayOneShot(depossessClip);
            StartCoroutine(ExitPossession());
        }
        
        GrayscaleToggle();
        DisplayCharacterInfo(); //Displays character portrait, name, and role
        InteractWithSafe();
        PickUp();
        
    }

    private void GrayscaleToggle()
    {
        if (!StateChecker.isGhost)
        {
            ppvToggle.Toggle(false);
        }
        else
        {
            ppvToggle.Toggle(true);
        }
    }

    public void InteractWithSafe()
    {
        Ray safeRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(safeRay, out hit))
        {
            //Debug.Log("hitting " + hit.collider.name + " + " + hit.collider.tag);
            if (hit.collider.tag == "safe" && hit.distance < reticleDist)
            {
                if (gameObject.GetComponent<Photographer>() || gameObject.GetComponent<Character>())
                {
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        safeName = hit.collider.name;
                        if (!safeManager.safe1Open && safeName == "LockedSafe1")
                        {
                            if (gameObject.GetComponent<Photographer>())
                            {
                                photographer.CameraLensActive = false;
                            }
                            safeManager.ShowKeypad();
                        }
                        else if (!safeManager.safe3Open && safeName == "LockedSafe3")
                        {
                            if (gameObject.GetComponent<Photographer>())
                            {
                                photographer.CameraLensActive = false;
                            }
                            safeManager.ShowKeypad();
                        }
                        else if (!safeManager.safe4Open && safeName == "LockedSafe4")
                        {
                            if (gameObject.GetComponent<Photographer>())
                            {
                                photographer.CameraLensActive = false;
                            }
                            safeManager.ShowKeypad();
                        }
                    }
                }
                else if (StateChecker.isGhost)
                {
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        safeName = hit.collider.name;
                        if (!safeManager.safe2Open && safeName == "LockedSafe2")
                        {
                            safeManager.ShowKeypad();
                        }
                    }
                }
            }
        }
    }
   


    //Ray safeCheck = Camera.main.ViewportPointToRay(new Vector3(0, 1, 0));

    public bool hasEnteredRatRoom = false;
    //PICKING UP OBJECTS
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "RatRoomEnterTrigger" && !hasEnteredRatRoom)
        {
            hasEnteredRatRoom = true;
            StartCoroutine(endingManager.KnifeScript());
        }
    }

    public void DisplayCharacterInfo()
    {
        if (gameObject.GetComponent<Photographer>())
        {
            itemImage.sprite = cameraImage;
            itemName.text = "Camera";
            characterImage.sprite = photographerImage;
            characterRole.text = "\"The Photographer\"";
            characterName.text = "Norman Adler";
            isRat = false;
        }
        else if (gameObject.GetComponent<Rat>())
        {
            itemImage.transform.parent.gameObject.SetActive(false);
            characterImage.sprite = ratImage;
            characterRole.text = "\"The Rat\"";
            characterName.text = "";
            isRat = true;
        }
        else if (gameObject.name == "Manager")
        {
            itemImage.transform.parent.gameObject.SetActive(false);
            characterImage.sprite = managerImage;
            characterRole.text = "\"The Manager\"";
            characterName.text = "Camille Bastet";
            isRat = false;
        }
        else if (gameObject.name == "Exterminator")
        {
            itemImage.transform.parent.gameObject.SetActive(false);
            characterImage.sprite = exterminatorImage;
            characterRole.text = "\"The Exterminator\"";
            characterName.text = "Jonathan Abberdasky";
            isRat = false;
        }
        else if (gameObject.name == "Mechanic")
        {
            itemImage.transform.parent.gameObject.SetActive(false);
            characterImage.sprite = mechanicImage;
            characterRole.text = "\"The Mechanic\"";
            characterName.text = "Janet Bastet";
            isRat = false;
        }
        else if (gameObject.name == "Hunter")
        {
            itemImage.transform.parent.gameObject.SetActive(false);
            characterImage.sprite = hunterImage;
            characterRole.text = "\"The Hunter\"";
            characterName.text = "Ahab Sergei";
            isRat = false;
        }
        else
        {
            itemImage.transform.parent.gameObject.SetActive(false);
            itemName.text = "";
            characterImage.sprite = ghostImage;
            characterRole.text = "\"The Spirit\"";
            //characterRoleForEnding = "The Spirit";
            characterName.text = "";
            isRat = false;
        }
    }

    public bool IsInside()
    {
        bool isInside;
        Vector3 fwd = new Vector3(0, 4, 0);
        if (StateChecker.isGhost)
        {
            fwd = new Vector3(0, 1, 0);
        }

        Ray indoorCheck = new Ray(GameObject.FindObjectOfType<Player>().transform.position + fwd, transform.up);
        //Debug.DrawLine(indoorCheck.origin, hit.transform.position);


        if (Physics.Raycast(indoorCheck, out hit))
        {
            //Debug.Log(hit.collider.gameObject.tag);
            if (hit.collider.CompareTag("balcony"))
            {
                isInside = false;
            }
            else
                isInside = true;
        }
        else
        {
            isInside = false;
        }
        //Debug.Log("Is inside: " + isInside);
        //Debug.Log(hit.collider.gameObject.name);
        return isInside;
    }

    void PickUp()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.distance < reticleDist)
            {
                //hoverUI
                var selection = hit.transform;
                if (selection.gameObject.GetComponent<Outline>())
                {
                    //Debug.Log("I'm looking at " + hit.transform.name);
                    //Debug.Log("Outline spotted");
                    reticle.color = selection.gameObject.GetComponent<Outline>().OutlineColor;

                    //pickup
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        if (selection.gameObject.CompareTag("Key") && !StateChecker.isGhost && !gameObject.GetComponent<Rat>())
                        {
                            Log.AddEntry("Picked up: " + selection.gameObject.name);
                            audioSource.PlayOneShot(obtainClip);
                            keys.Add(selection.gameObject);
                            Destroy(selection.gameObject);
                        }
                        else if (selection.gameObject.CompareTag("Knife") && StateChecker.isGhost)
                        {
                            hasKnife = true;
                            Log.AddEntry("Picked up Knife");
                            audioSource.PlayOneShot(obtainClip);
                            endingManager.ShowKnifeInstructions();
                            Destroy(selection.gameObject);
                        }
                    }
                }
                //else if (selection.gameObject.CompareTag("Person"))
                //{
                //    reticle.color = new Color32(254, 224, 0, 100);
                //}
                else
                {
                    reticle.color = new Color32(0, 255, 255, 100);
                }

                if (selection.gameObject.GetComponent<HoverText>())
                {
                    Debug.Log("hover text");
                    selection.gameObject.GetComponent<HoverText>().display = true;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //canPickup = false;
        //pickUpInstructions.gameObject.SetActive(false);
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
        transform.rotation = Quaternion.Euler(0, LookAtRot.eulerAngles.y, 0);
        cam.transform.localRotation = Quaternion.identity;//Quaternion.Euler(LookAtRot.eulerAngles.x, 0, LookAtRot.eulerAngles.z);

    }

    /// <summary>
    /// Handles the movement of the player
    /// </summary>
    private void Movement()
    {
        xMovement = Input.GetAxis("Horizontal");
        yMovement = Input.GetAxis("Vertical");

        //Tutorial Bit
        if ((xMovement != 0 || yMovement != 0) && Dialogue.holding)
        {
            Tutorial.instance.OnFirstMovement();
        }

        //Movement
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
        //The distance forward to go forward
        Vector3 targetPos = cam.transform.position + gameObject.transform.forward * possess_Distance;
        //using targetDist temporarily to find raycast distance
        targetDist = Vector3.Distance(targetPos, cam.transform.position) / Mathf.Cos((-cam.transform.rotation.eulerAngles.x) * Mathf.Deg2Rad);
        //Scan area directly in front for targets
        RaycastHit[] hit = Physics.BoxCastAll(cam.transform.position, new Vector3(0.25f, 0.25f, 0.25f), cam.transform.forward, Quaternion.identity, targetDist);
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

        //Check to make sure that the target is within line of sight (No targeting through walls)
        if (target != null)
        {
            hit = Physics.RaycastAll(cam.transform.position, (target.transform.position - cam.transform.position).normalized + target.GetComponent<Possessable>().GetCameraOffset(), possess_Distance);
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
        {
            audioSource.PlayOneShot(possessClip);

            StartCoroutine(Possess(target));
        }

    }

    bool possessionInProgress = false;
    private RaycastHit hit;


    /// <summary>
    /// Handles the possession transition into another GameObject
    /// </summary>
    /// <param name="target">The target GameObject to possess</param>
    /// <returns></returns>
    private IEnumerator Possess(GameObject target)
    {
        canPickup = true;
        hasPossessedForTheFirstTime = true;
        //itemSpecificInstructions.gameObject.SetActive(true);
        possessionInProgress = true;
        //No Target 
        if (target == null)
            yield break;

        if (target.GetComponent<NavPerson>())
        {
            target.GetComponent<NavPerson>().enabled = false;
        }

        Transform targetTransform;
        if (target.transform.Find("CamPoint"))
            targetTransform = target.transform.Find("CamPoint");
        else
            targetTransform = target.transform;


        //Cam Shift & Alpha fade
        EnableControls(false);

        //Look at what is about to be possessed
        cam.transform.parent = null;
        Vector3 direction = (targetTransform.position + target.GetComponent<Possessable>().GetCameraOffset() - transform.position);
        cam.transform.rotation = Quaternion.LookRotation(direction);
        direction.y = 0;
        transform.rotation = Quaternion.LookRotation(direction);

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
            camComp.fieldOfView = Mathf.Lerp(maxFOV, minFOV, Mathf.SmoothStep(0f, 1f, currentTime / transitionTime));
            Color goalColor = mat.color;
            goalColor.a = Mathf.Lerp(maxAlpha, minAlpha, Mathf.SmoothStep(0f, 1f, currentTime / transitionTime));
            mat.color = goalColor;
            cam.transform.position = Vector3.Lerp(gameObject.transform.position, targetTransform.position + target.GetComponent<Possessable>().GetCameraOffset()
                                                    , Mathf.SmoothStep(0f, 1f, currentTime / transitionTime));
            currentTime += Time.deltaTime;
            yield return null;
        }

        currentTime = 0;
        target.transform.rotation = gameObject.transform.rotation;
        cam.transform.SetParent(targetTransform);
        cam.transform.localPosition = Vector3.zero + target.GetComponent<Possessable>().GetCameraOffset();
        //Get Rid of Effects of currently Possessed Objects
        if (gameObject != mainPlayer)
            gameObject.GetComponent<Possessable>().TriggerOnPossession(false);
        //Start Effect of What is Being Possessed
        target.GetComponent<Possessable>().TriggerOnPossession(true);

        Quaternion startRot = cam.transform.localRotation;
        //Zoom out
        while (camComp.fieldOfView < maxFOV)
        {
            camComp.fieldOfView = Mathf.Lerp(minFOV, maxFOV, Mathf.SmoothStep(0f, 1f, currentTime / transitionTime));
            Color goalColor = mat.color;
            goalColor.a = Mathf.Lerp(minAlpha, maxAlpha, Mathf.SmoothStep(0f, 1f, currentTime / transitionTime));
            cam.transform.localRotation = Quaternion.Lerp(startRot, Quaternion.identity, Mathf.SmoothStep(0f, 1f, currentTime / transitionTime));
            mat.color = goalColor;
            currentTime += Time.deltaTime;
            yield return null;
        }

        lookHorizontal = cam.transform.rotation.eulerAngles.y;
        lookVertical = cam.transform.rotation.eulerAngles.x;

        //End Camera Shift & Alpha Fade

        //Copy Player Script and all its public fields
        System.Type type = this.GetType();
        Component copy = target.AddComponent(type);
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(this));
        }

        EnableControls(true);

        //First Rat possession
        if (target.GetComponent<Rat>() && Dialogue.holding)
        {
            Tutorial.instance.OnFirstRatPossession();
        }

        //If it's the main player (Ghost) then make it "disappear"
        if (gameObject == mainPlayer)
            gameObject.SetActive(false);
        //If it's not the main player, remove player script
        else if (gameObject != mainPlayer)
        {
            //if(GetComponent<Rat>())


            if (GetComponent<NavPerson>())
                GetComponent<NavPerson>().enabled = true;
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
        canPickup = false;
        possessionInProgress = true;

        GetComponent<Possessable>().TriggerHighlight();

        //Find a point with enough space to exit to once possession ends
        Vector3 exitPoint = gameObject.transform.position;

        float mainPlayerMaxExtents = Mathf.Max(mainPlayer.GetComponent<MeshRenderer>().bounds.extents.x, mainPlayer.GetComponent<MeshRenderer>().bounds.extents.z);
        float thisMaxExtents = Mathf.Max(gameObject.GetComponent<MeshRenderer>().bounds.extents.x, gameObject.GetComponent<MeshRenderer>().bounds.extents.z);
        float checkRadius = thisMaxExtents + mainPlayerMaxExtents * 2;
        Vector3 closestPointOnFloor = Vector3.zero;
        bool safeExitPoint = false;

        //Test points around player like a coordinate plane (ex (0,0), (0, 1), (0,2), ..., etc)
        int[] multiplierOptions = { 0, 1, -1 };
        Vector3 temp = exitPoint;
        for (int i = 0; i < multiplierOptions.Length; i++) //Diagonals
        {
            for (int j = multiplierOptions.Length - 1; j > 0; j--) //Cardinal Directions
            {
                //Set temp to point being tested, account for size of mesh to make sure it doesn't clip through wall
                temp = GetComponent<MeshRenderer>().bounds.center + (transform.forward * checkRadius * multiplierOptions[j]) + (transform.right * checkRadius * multiplierOptions[i]);
                temp.y -= GetComponent<MeshRenderer>().bounds.extents.y;
                //point is deemed safe until proven not
                safeExitPoint = true;



                //In case there are multiple floors, get the highest one
                List<int> floorIndexes = new List<int>();

                //Make sure there's enough space around the point
                Collider[] hit = Physics.OverlapSphere(temp, mainPlayerMaxExtents / 2);
                for (int k = 0; k < hit.Length; k++)
                {
                    if (hit[k].gameObject.tag != "Floor" && hit[k].gameObject != gameObject)
                    {
                        safeExitPoint = false;
                        break;
                    }
                    else if (hit[k].gameObject.tag == "Floor")
                        floorIndexes.Add(k);
                }

                float maxHeight = Mathf.NegativeInfinity;
                int highestIndex = -1;
                //In case there are multiple floors, find the highest one to avoid clipping
                foreach (int x in floorIndexes)
                {
                    if (hit[x].bounds.center.y > maxHeight)
                        maxHeight = hit[x].transform.position.y;
                    highestIndex = x;
                }
                Vector3 heightModified = temp;
                heightModified.y += mainPlayer.GetComponent<MeshRenderer>().bounds.extents.y;
                if (highestIndex != -1)
                {
                    closestPointOnFloor = hit[highestIndex].ClosestPointOnBounds(heightModified);
                    heightModified.y = closestPointOnFloor.y + mainPlayer.GetComponent<MeshRenderer>().bounds.extents.y;
                }

                //Make sure point is visible
                RaycastHit[] visibleHit;
                visibleHit = Physics.BoxCastAll(cam.transform.position, new Vector3(0.1f, 0.1f, 0.1f), heightModified - cam.transform.position, cam.transform.rotation, Vector3.Distance(heightModified, cam.transform.position), ~0, QueryTriggerInteraction.Ignore);
                foreach (RaycastHit x in visibleHit)
                {
                    if (x.collider.gameObject != gameObject)
                    {
                        safeExitPoint = false;
                        continue;
                    }
                }


                if (safeExitPoint)
                    break;
            }

            if (safeExitPoint)
                break;
        }

        if (safeExitPoint) //Safe exit point with no collisions found
        {
            temp.y = closestPointOnFloor.y + mainPlayer.GetComponent<MeshRenderer>().bounds.extents.y;
            exitPoint = temp;
        }
        else //No possible exit points found, can't unpossess here
        {
            Log.AddEntry("No Room to De-Possess Here");
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
        Vector3 direction = (transform.position - exitPoint).normalized;
        direction.y = 0;

        mainPlayer.transform.rotation = Quaternion.LookRotation(direction);

        if (gameObject != mainPlayer)
            gameObject.GetComponent<Possessable>().TriggerOnPossession(false);

        Vector3 startPoint = transform.position;
        if (transform.Find("CamPoint"))
            startPoint = transform.Find("CamPoint").position + camOffset;

        //Zoom out
        while (camComp.fieldOfView < maxFOV)
        {
            camComp.fieldOfView = Mathf.Lerp(minFOV, maxFOV, currentTime / transitionTime);
            cam.transform.position = Vector3.Lerp(startPoint, mainPlayer.transform.position + camOffset, Mathf.SmoothStep(0f, 1f, currentTime / transitionTime));
            cam.transform.rotation = Quaternion.Lerp(transform.rotation, mainPlayer.transform.rotation, Mathf.SmoothStep(0f, 1f, currentTime / transitionTime));

            currentTime += Time.deltaTime;
            yield return null;
        }


        //cam.transform.position = mainPlayer.transform.position + camOffset;
        cam.transform.SetParent(mainPlayer.transform);
        cam.transform.localPosition = Vector3.zero + camOffset;
        cam.transform.localRotation = Quaternion.identity;

        currentTime = 0;

        //Zoom in
        while (camComp.fieldOfView > minFOV)
        {
            camComp.fieldOfView = Mathf.Lerp(maxFOV, minFOV, Mathf.SmoothStep(0f, 1f, currentTime / transitionTime));
            currentTime += Time.deltaTime;
            yield return null;
        }



        //Make sure the player is looking in the same direction as they were in their body so that the transition isn't jarring
        mainPlayer.GetComponent<Player>().lookHorizontal = cam.transform.rotation.eulerAngles.y;
        mainPlayer.GetComponent<Player>().lookVertical = cam.transform.rotation.eulerAngles.x;

        //Transition complete, return control to player
        EnableControls(true);
        mainPlayer.GetComponent<Player>().possessionInProgress = false;

        //If this is not the main player (Ghost) then fire any events related to leaving a host and get rid of player script
        if (gameObject != mainPlayer)
        {
            if (GetComponent<NavPerson>())
            {
                GetComponent<NavPerson>().enabled = true;
            }

            Destroy(GetComponent<Player>());
        }

        possessionInProgress = false;
    }

    public void WalkAudio()
    {
        if (!StateChecker.isGhost)
        {
            if (Input.GetAxis("Horizontal") > 0 || Input.GetAxis("Vertical") > 0 || Input.GetAxis("Horizontal") < 0 || Input.GetAxis("Vertical") < 0)
            {
                audioSource.volume = stepVolume;
                int rand;
                if (isRat)
                {
                    rand = Random.Range(0, ratSteps.Length);
                    step = ratSteps[rand];
                    audioSource.PlayOneShot(step);
                }
                else if (IsInside() == true)
                {
                    rand = Random.Range(0, indoorSteps.Length);
                    step = indoorSteps[rand];
                    audioSource.PlayOneShot(step);
                }
                else if (isFemale)
                {
                    rand = Random.Range(0, femaleSteps.Length);
                    step = femaleSteps[rand];
                    audioSource.PlayOneShot(step);
                }
                else
                {
                    rand = Random.Range(0, maleSteps.Length);
                    step = maleSteps[rand];
                    audioSource.PlayOneShot(step);
                }
            }
        }

    }

    private void HideReticle(bool shouldHide)
    {
        if (shouldHide)
        {
            reticle.gameObject.SetActive(false);
        }
        else
        {
            reticle.gameObject.SetActive(true);
        }
    }

    private void OnDisable()
    {
        MainMenu.onMainMenuTriggered -= HideReticle;
    }

}
