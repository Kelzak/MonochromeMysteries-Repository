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

    //public Text pickUpInstructions;
    //public Text possessionInstructions;
    //public Text pictureTakingInstructions;
    //public Text itemSpecificInstructions;
    public bool hasPossessedForTheFirstTime;

    //UI Images and Texts
    public Sprite ghostImage;
    public Text ghostName;
    public Sprite photographerImage;
    public Sprite ratImage;
    public Image characterImage;
    public Text characterName;
    public Sprite cameraImage;
    public Image itemImage;
    public Text itemName;
    public GameObject keyImage;
    public Image reticle;
    public float reticleDist;
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

        camOffset = new Vector3(0, 0.25f, 0);

        if (mainPlayer == null)
            mainPlayer = gameObject;

        if(cam == null)
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
           
        if (gameObject.GetComponent<Photographer>() || gameObject.GetComponent<Rat>() || gameObject.GetComponent<Character>())
        {
            ppvToggle.Toggle(false);
        }
        else
        {
            ppvToggle.Toggle(true);
        }
        /*
        if (!hasPossessedForTheFirstTime)
        {
            possessionInstructions.gameObject.SetActive(true);
        }
        else
        {
            possessionInstructions.gameObject.SetActive(false);
        }*/

        if (gameObject.GetComponent<Photographer>())
        {
            itemImage.sprite = cameraImage;
            itemName.text = "Camera";
            characterImage.sprite = photographerImage;
            characterName.text = "\"The Photographer\"";
            isRat = false;
        }
        else if (gameObject.GetComponent<Rat>())
        {
            itemImage.transform.parent.gameObject.SetActive(false);
            characterImage.sprite = ratImage;
            characterName.text = "\"The Rat\"";
            isRat = true;
        }
        else
        {
            itemImage.transform.parent.gameObject.SetActive(false);
            itemName.text = "";
            characterImage.sprite = ghostImage;
            characterName.text = "\"The Spirit\"";
            isRat = false;
        }

        PickUp();

        Debug.Log(isAtTheFirstSafe);
        Debug.Log(isAtTheSecondSafe);
        Debug.Log(isAtTheThirdSafe);
    }

    public bool isLookingAtSafe1;
    public bool isLookingAtSafe2;
    public bool isLookingAtSafe3;

    public GameObject keypadPanel;

    //PICKING UP OBJECTS
    private void OnTriggerStay(Collider other)
    {
        Ray safeCheck = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(safeCheck, out hit))
        {
            Debug.Log(hit.collider.name);
            if(hit.collider.name == "LockedSafe1")
            {
                isLookingAtSafe1 = true;
            }
            else if (hit.collider.name == "LockedSafe2")
            {
                isLookingAtSafe2 = true;
            }
            else if (hit.collider.name == "LockedSafe3")
            {
                isLookingAtSafe3 = true;
            }
            else
            {
                isLookingAtSafe1 = false;
                isLookingAtSafe2 = false;
                isLookingAtSafe3 = false;
            }

        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            if (other.gameObject.name == "LockedSafe1" && isLookingAtSafe1)
            {
                keypadPanel.SetActive(true);
                isAtTheFirstSafe = true;
                isAtTheSecondSafe = false;
                isAtTheThirdSafe = false;
            }
            else if (other.gameObject.name == "LockedSafe2" && isLookingAtSafe2)
            {
                keypadPanel.SetActive(true);
                isAtTheFirstSafe = false;
                isAtTheSecondSafe = true;
                isAtTheThirdSafe = false;
            }
            else if (other.gameObject.name == "LockedSafe3" && isLookingAtSafe3)
            {
                keypadPanel.SetActive(true);
                isAtTheFirstSafe = false;
                isAtTheSecondSafe = false;
                isAtTheThirdSafe = true;
            }
            else
            {
                isAtTheFirstSafe = false;
                isAtTheSecondSafe = false;
                isAtTheThirdSafe = false;
            }
        }
        

        ////kevs stuff below
        ///
        //if (other.gameObject.tag == "Selectable")
        //{
        //    if (other.gameObject.GetComponent<Item>().itemName == "Camera")
        //    {
        //        Debug.Log("Text should appear");
        //        //pickUpInstructions.gameObject.SetActive(true);
        //    }
        //    canPickup = true;
        //    Debug.Log("CanPickUp = " + canPickup);

        //    if (Input.GetKeyDown(KeyCode.F) && canPickup)
        //    {
        //        if (gameObject.tag == "Photographer" && other.gameObject.GetComponent<Item>().itemName == "Camera")
        //        {
        //            Debug.Log("Picking up Camera...");
        //            hasCamera = true;
        //            photographer.ToggleHUD(true);
        //            Destroy(other.gameObject);
        //            //pictureTakingInstructions.gameObject.SetActive(true);
        //            //pickUpInstructions.gameObject.SetActive(false);
        //            //itemSpecificInstructions.gameObject.SetActive(false);
        //        }
        //        else if (other.gameObject.GetComponent<Item>().itemName == "Key" && !(gameObject.GetComponent<Rat>()))
        //        {
        //            hasKey = true;
        //            keyImage.SetActive(true);
        //            Debug.Log("You picked up a key!");
        //            Destroy(other.gameObject);
        //        }
        //    }
        //}

        //if (other.gameObject.tag == "LockedDoor")
        //{
        //    if (Input.GetKeyDown(KeyCode.F) && hasKey)
        //    {
        //        Debug.Log("You unlocked the door!");
        //        Destroy(other.gameObject);
        //        hasKey = false;
        //        keyImage.SetActive(false);
        //        Debug.Log("You dont have the key anymore");
        //    }
        //}
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
        if (this.gameObject.CompareTag("Rat") || this.gameObject.CompareTag("Player"))
            canPickup = false;
        else
            canPickup = true;

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
                    if (Input.GetKeyDown(KeyCode.F) && selection.gameObject.CompareTag("Key") && canPickup)
                    {
                        //Debug.Log("Added Key");
                        audioSource.PlayOneShot(obtainClip);
                        keys.Add(selection.gameObject);
                        Destroy(selection.gameObject);
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
        //Scan area directly in front for targets
        RaycastHit[] //hit = Physics.RaycastAll(cam.transform.position, cam.transform.forward, possess_Distance);
        hit = Physics.BoxCastAll(cam.transform.position, new Vector3(0.25f, 0.25f, 0.25f), cam.transform.forward, Quaternion.identity, possess_Distance);
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
        targetTransform = target.GetComponent<Photographer>() ? target.transform.Find("CamPoint") : target.transform;
        targetTransform = target.GetComponent<Character>() ? target.transform.Find("CamPoint") : target.transform;

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
        if(target.GetComponent<Rat>() && Dialogue.holding)
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
        for (int i = 0; i < multiplierOptions.Length && safeExitPoint == false; i++) //Diagonals
        {
            for (int j = multiplierOptions.Length - 1; j > 0 && safeExitPoint == false; j--) //Cardinal Directions
            {
                //Set temp to point being tested, account for size of mesh to make sure it doesn't clip through wall
                temp = GetComponent<MeshRenderer>().bounds.center + (transform.forward * checkRadius * multiplierOptions[j]) + (transform.right * checkRadius * multiplierOptions[i]);
                temp.y -= GetComponent<MeshRenderer>().bounds.extents.y;
                //point is deemed safe until proven not
                safeExitPoint = true;

                //Make sure point is visible
                RaycastHit visibleHit;
                if(Physics.Linecast(cam.transform.position, temp, out visibleHit) && visibleHit.collider.gameObject != gameObject)
                    safeExitPoint = false;

                //Make sure there's enough space around the point
                Collider[] hit = Physics.OverlapSphere(temp, mainPlayerMaxExtents);
                for (int k = 0; k < hit.Length; k++)
                {
                    if (hit[k].gameObject.tag != "Floor" && hit[k].gameObject != gameObject)
                        safeExitPoint = false;
                    else
                        closestPointOnFloor = hit[k].ClosestPoint(temp);
                }
            }
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

        //Zoom out
        while (camComp.fieldOfView < maxFOV)
        {
            camComp.fieldOfView = Mathf.Lerp(minFOV, maxFOV, currentTime / transitionTime);
            cam.transform.position = Vector3.Lerp(transform.position, mainPlayer.transform.position + camOffset, Mathf.SmoothStep(0f, 1f, currentTime / transitionTime));
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
            if(Input.GetAxis("Horizontal") > 0 || Input.GetAxis("Vertical") > 0 || Input.GetAxis("Horizontal") < 0 || Input.GetAxis("Vertical") < 0)
            {
                audioSource.volume = stepVolume;
                int rand;
                if(isRat)
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
    
}
