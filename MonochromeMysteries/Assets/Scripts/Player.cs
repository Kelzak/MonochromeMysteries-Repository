using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [HideInInspector]
    public GameObject mainPlayer = null;

    [Header("Aiming")]
    public float lookSensitivity = 5f;
    [HideInInspector]
    public bool canLook = true;

    [HideInInspector]
    public float lookHorizontal;
    [HideInInspector]
    public float lookVertical;
    [HideInInspector]
    public float verticalClamp = 60;
    private GameObject cam;

    [Header("Movement")]
    public float moveSpeed = 10f;
    [HideInInspector]
    public bool canMove = true;

    float xMovement, yMovement;
    CharacterController character;

    [Header("Possession")]
    public float possess_Distance = 3;
    public GameObject possess_Vignette;

    // Start is called before the first frame update
    void Start()
    {
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
        if(canLook)
            Look();
        if(canMove)
            Movement();

        PossessionCheck();

        if (Input.GetKeyDown(KeyCode.Q) && gameObject != mainPlayer && !possessionInProgress)
            StartCoroutine(ExitPossession());

    }

    //PUBLIC FUNCTIONS
    public void EnableControls(bool on)
    {
        canMove = canLook = on;
    }


    //PRIVATE FUNCTIONS
    private void Look()
    {
        lookHorizontal += Input.GetAxis("Mouse X") * lookSensitivity;
        lookVertical = Mathf.Clamp(lookVertical - Input.GetAxis("Mouse Y") * lookSensitivity, -verticalClamp, verticalClamp);
        transform.localRotation = Quaternion.Euler(0, lookHorizontal, 0);
        cam.transform.localRotation = Quaternion.Euler(lookVertical, 0, 0);
    }

    private void LookAt(GameObject obj)
    {
        Quaternion LookAtRot = Quaternion.LookRotation(obj.transform.position - transform.position);
        transform.rotation = Quaternion.Euler(0, lookHorizontal = LookAtRot.eulerAngles.y, 0);
        cam.transform.localRotation = Quaternion.Euler(LookAtRot.eulerAngles.x, 0, lookVertical = LookAtRot.eulerAngles.z);

    }


    private void Movement()
    {
        xMovement = Input.GetAxis("Horizontal");
        yMovement = Input.GetAxis("Vertical");

        Vector3 velocity = (transform.right * xMovement * moveSpeed) + (transform.forward * yMovement * moveSpeed);
        if(!character.isGrounded)
        {
            velocity += Physics.gravity;
        }

        character.Move(velocity * Time.deltaTime);
    }

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
        if(target == null && Possessable.GetHighlightedObject() != null)
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
    private IEnumerator Possess(GameObject target)
    {
        possessionInProgress = true;

        //No Target 
        if (target == null)
            yield break;

        if(!target.GetComponent<CharacterController>())
        target.AddComponent<CharacterController>();

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

        possess_Vignette.SetActive(true);
        currentTime = 0;
        target.transform.rotation = cam.transform.rotation;
        cam.transform.SetParent(target.transform);
        cam.transform.localPosition = Vector3.zero;

        //Zoom out
        while (camComp.fieldOfView < maxFOV)
        {
            camComp.fieldOfView = Mathf.Lerp(minFOV, maxFOV, currentTime / (transitionTime /4f));
            Color goalColor = mat.color;
            goalColor.a = Mathf.Lerp(minAlpha, maxAlpha, currentTime / (transitionTime /4f));
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
        else if(gameObject != mainPlayer)
        {
            Destroy(GetComponent<Player>());
        }

        possessionInProgress = false;
    }

    private IEnumerator ExitPossession()
    {
        possessionInProgress = true;

        //Find a point with enough space to exit to once possession ends
        Vector3 exitPoint = gameObject.transform.position;

        float mainPlayerMaxExtents = Mathf.Max(mainPlayer.GetComponent<MeshRenderer>().bounds.extents.x, mainPlayer.GetComponent<MeshRenderer>().bounds.extents.z);
        float thisMaxExtents = Mathf.Max(gameObject.GetComponent<MeshRenderer>().bounds.extents.x, gameObject.GetComponent<MeshRenderer>().bounds.extents.z);
        float checkRadius = thisMaxExtents + mainPlayerMaxExtents * 2;
        bool safeExitPoint = false;

        int[] multiplierOptions = { 0, -1, 1};
        Vector3 temp = exitPoint;
        for (int i = 0; i < multiplierOptions.Length && safeExitPoint == false; i++) //Diagonals
        {
            for(int j = multiplierOptions.Length - 1; j > 0 && safeExitPoint == false; j--) //Cardinal Directions
            {
                //temp = new Vector3(exitPoint.x + checkRadius * multiplierOptions[i], exitPoint.y, exitPoint.z + checkRadius * multiplierOptions[j]);
                temp = transform.position + ( transform.forward * checkRadius * multiplierOptions[j] ) + ( transform.right * checkRadius * multiplierOptions[i] );
                Collider[] hit = Physics.OverlapSphere(temp, mainPlayerMaxExtents);
                safeExitPoint = true;
                foreach(Collider x in hit)
                {
                    if (x.gameObject.tag != "Floor")
                        safeExitPoint = false;
                }
            }
        }

        if(safeExitPoint) //Safe exit point with no collisions found
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
        float maxFOV = 175;
        float minFOV = camComp.fieldOfView;
        float transitionTime = 0.5f;
        float currentTime = 0;

        //Zoom out
        while (camComp.fieldOfView < maxFOV)
        {
            camComp.fieldOfView = Mathf.Lerp(minFOV, maxFOV, currentTime / transitionTime);
            currentTime += Time.deltaTime;
            yield return null;
        }

        //Reactivate the Player, 
        mainPlayer.SetActive(true);
        mainPlayer.GetComponent<Player>().EnableControls(false);
        mainPlayer.transform.position = exitPoint;
        cam.transform.position = mainPlayer.transform.position;
        cam.transform.SetParent(mainPlayer.transform);
        mainPlayer.GetComponent<Player>().LookAt(gameObject);
        currentTime = 0;
        possess_Vignette.SetActive(false);

        //Zoom in
        while (camComp.fieldOfView > minFOV)
        {
            camComp.fieldOfView = Mathf.Lerp(maxFOV, minFOV, currentTime / transitionTime);
            currentTime += Time.deltaTime;
            yield return null;
        }

        //System.Type type = this.GetType();
        //System.Reflection.FieldInfo[] fields = type.GetFields();
        //foreach (System.Reflection.FieldInfo field in fields)
        //{
        //    field.SetValue(mainPlayer.GetComponent<Player>(), field.GetValue(this));
        //}

        mainPlayer.GetComponent<Player>().EnableControls(true);

        if(gameObject != mainPlayer)
        {
            Destroy(GetComponent<Player>());
        }

        possessionInProgress = false;
    }
}
