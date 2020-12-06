//PlayerHUD
/* Author: Matt Kirchoff
 * Controls player hud and interacting with objects
 * 
 * 
 * 
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [HideInInspector]
    public Player player;


    public float fadeTime = 3f;


    [Header("Player Display Objects / Text")]
    public TMP_Text topText;
    public TMP_Text bottomText;
    public TMP_Text singleText;

    public GameObject displayIcon;
    public GameObject displaySeperator;
    private Image displaySeperatorImage;
    private Image displayIconImage;


    [Header("Reticle Settings")]
    public Image reticle;
    public Color defaultReticleColor = new Color32(0, 255, 255, 100);

    [Header("location HUD")]
    public TMP_Text playerLocationText;
    public GameObject leftDash;
    public GameObject rightDash;

    [Header("Audio Settings")]
    public AudioSource hoverAudio;
    public AudioClip hoverClip;
    [Range(0, 1)]
    public float hoverAudioVolume = .5f;

    //hidden static used variables for updating UI assets
    [HideInInspector]
    public static bool showLocation;
    [HideInInspector]
    public static string locationString;
    [HideInInspector]
    public static string topString;
    [HideInInspector]
    public static string bottomString;
    [HideInInspector]
    public static string singleString;
    [HideInInspector]
    public static Sprite iconSprite;
    [HideInInspector]
    public static bool showDisplayUI;
    [HideInInspector]
    public static bool onlySingleText;
    [HideInInspector]
    public static Color reticleColor;
    [HideInInspector]
    public static bool noOutline;

    // Start is called before the first frame update
    void Start()
    {
        reticle = GameObject.FindGameObjectWithTag("Reticle").GetComponent<Image>();
        displayIconImage = displayIcon.GetComponent<Image>();
        displaySeperatorImage = displaySeperator.GetComponent<Image>();

        singleText.text = singleString;
        topText.text = topString;
        bottomText.text = bottomString;
        displayIconImage.sprite = iconSprite;

        //initalize hover settings
        hoverAudio = GetComponent<AudioSource>();
        hoverAudio.volume = 0f;
        hoverAudio.clip = hoverClip;

        player = FindObjectOfType<Player>();
    }


    // Update is called once per frame
    void Update()
    {
        LocationUI();

        if(!GameController.menuActive && !MainMenu.active)
        {
            topText.gameObject.SetActive(true);
            bottomText.gameObject.SetActive(true);
            singleText.gameObject.SetActive(true);
            displayIconImage.gameObject.SetActive(true);
            reticle.gameObject.SetActive(true);
            displaySeperatorImage.gameObject.SetActive(true);
            DisplayUI();

        }
        else
        {
            topText.gameObject.SetActive(false);
            bottomText.gameObject.SetActive(false);
            singleText.gameObject.SetActive(false);
            displayIconImage.gameObject.SetActive(false);
            reticle.gameObject.SetActive(false);
            displaySeperatorImage.gameObject.SetActive(false);

            //hoverAudio.volume = 0f;
        }

        

    }

    //manages location UI
    void LocationUI()
    {
        if (showLocation)
        {
            playerLocationText.text = locationString;
            playerLocationText.color = Color.Lerp(playerLocationText.color, Color.white, fadeTime * Time.deltaTime);
            //leftDash.GetComponent<Image>().color = Color.Lerp(playerLocationText.color, Color.white, fadeTime * Time.deltaTime);
            //rightDash.GetComponent<Image>().color = Color.Lerp(playerLocationText.color, Color.white, fadeTime * Time.deltaTime);
            //print(playerLocationText.textBounds);

        }
        else
        {
            playerLocationText.color = Color.Lerp(playerLocationText.color, Color.clear, fadeTime * Time.deltaTime);
            //leftDash.GetComponent<Image>().color = Color.Lerp(playerLocationText.color, Color.clear, fadeTime * Time.deltaTime);
            //rightDash.GetComponent<Image>().color = Color.Lerp(playerLocationText.color, Color.clear, fadeTime * Time.deltaTime);

        }
    }

    //manges display UI
    public void DisplayUI()
    {
        //print("hover volume: " + hoverAudio.volume);

        //if showing, read values and display
        if (showDisplayUI)
        {
            //set reticle color, reads from glow of object
            reticle.color = Color.Lerp(reticle.color, reticleColor, fadeTime * Time.deltaTime);

            if(noOutline)
            {
                hoverAudio.volume = Mathf.Lerp(hoverAudio.volume, 0f, fadeTime * Time.deltaTime);
            }
            else
            {
                hoverAudio.volume = Mathf.Lerp(hoverAudio.volume, hoverAudioVolume, fadeTime * Time.deltaTime);

            }

            //set string values to text mesh assets
            singleText.text =  singleString;
            topText.text = topString;
            bottomText.text = bottomString;
            displayIconImage.sprite = iconSprite;

            //fade in display and icons, activate hover audio
            //handle only single text displays 
            if (onlySingleText)
            {
                singleText.color = Color.Lerp(singleText.color, Color.white, fadeTime * Time.deltaTime);
                topText.color = Color.Lerp(topText.color, Color.clear, fadeTime * Time.deltaTime);
                bottomText.color = Color.Lerp(bottomText.color, Color.clear, fadeTime * Time.deltaTime);
                displayIconImage.color = Color.Lerp(displayIconImage.color, Color.clear, fadeTime * Time.deltaTime);
                displaySeperatorImage.color = Color.Lerp(displayIconImage.color, Color.clear, fadeTime * Time.deltaTime);
            }
            else
            {
                topText.color = Color.Lerp(topText.color, Color.white, fadeTime * Time.deltaTime);
                bottomText.color = Color.Lerp(bottomText.color, Color.white, fadeTime * Time.deltaTime);
                displayIconImage.color = Color.Lerp(displayIconImage.color, Color.white, fadeTime * Time.deltaTime);
                displaySeperatorImage.color = Color.Lerp(displayIconImage.color, Color.white, fadeTime * Time.deltaTime);
                singleText.color = Color.Lerp(singleText.color, Color.clear, fadeTime * Time.deltaTime);

            }
        }
        //else, default case
        else
        { 
            //fade out display and icons, deactivate hover audio, default reticle
            topText.color = Color.Lerp(topText.color, Color.clear, fadeTime * Time.deltaTime);
            bottomText.color = Color.Lerp(bottomText.color, Color.clear, fadeTime * Time.deltaTime);
            singleText.color = Color.Lerp(singleText.color, Color.clear, fadeTime * Time.deltaTime);
            displayIconImage.color = Color.Lerp(displayIconImage.color, Color.clear, fadeTime * Time.deltaTime);
            reticle.color = Color.Lerp(reticle.color, defaultReticleColor, fadeTime * Time.deltaTime);
            displaySeperatorImage.color = Color.Lerp(displayIconImage.color, Color.clear, fadeTime * Time.deltaTime);
            hoverAudio.volume = Mathf.Lerp(hoverAudio.volume, 0f, fadeTime * Time.deltaTime);
            return;
        }


    }

    /*
    void Raycast()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit[] hit;
        if ((hit = Physics.RaycastAll(ray, Player.reticleDist)).Length > 0)
        {
            GameObject target = null;
            float shortestDistance = Mathf.Infinity;
            for (int i = 0; i < hit.Length; i++)
            {
                if (hit[i].distance < shortestDistance && hit[i].collider.gameObject != gameObject)
                {
                    target = hit[i].collider.gameObject;
                    shortestDistance = hit[i].distance;
                }
            }

            if (target == null)
            {
                ClearReticleDisplay();
                return;
            }


            //default glowing objects
            if (target.GetComponent<Outline>())
            {
                Outline outline = target.GetComponent<Outline>();
                outline.enabled = true;

                UpdateReticleDisplay(cameraIcon, "Take photo", "Needs Photographer");
            }

            //possessable
            if (target.GetComponent<Possessable>())
            {
                UpdateSingleText("Press E to Possess");
            }

            //rat draging
            if (GetComponent<Rat>())
            {
                if (target.GetComponent<Item>().isPickup)
                {
                    //is holding item
                    if (Rat.hold)
                    {
                        UpdateReticleDisplay(cameraIcon, "Press F to Drop", "");
                    }
                    else
                    {
                        UpdateReticleDisplay(cameraIcon, "Press F to Drag", "");
                    }
                }
            }

            //reading
            if (target.GetComponent<Read>())
            {
                //spirit read
                if(StateChecker.isGhost)
                {
                    //UpdateReticleDisplay(cameraIcon, "Spirit can't read", "Take photo");
                    UpdateSingleText("Spirit can't read");
                }
                else if (GetComponent<Rat>())
                {
                    //UpdateReticleDisplay(cameraIcon, "Rat can't read", "Take photo");
                }
                else
                {
                    UpdateReticleDisplay(cameraIcon, "Press F to Read", "Take photo");

                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        target.GetComponent<Read>().Open();
                    }
                }
            }

            if (target.CompareTag("door"))
            {
                DoorScript doorScript = target.GetComponentInParent<DoorScript>();

                //mechanic door
                if (doorScript.whoDoor.Equals("Mechanic") && target.GetComponentInParent<DoorScript>().personalDoor)
                {
                    if (doorScript.repairing)
                    {
                        UpdateReticleDisplay(toolIcon, "Repairing", "Needs Mechanic");
                    }
                    else
                    {
                        UpdateReticleDisplay(toolIcon, "Press F to Repair", "Needs Mechanic");

                        if (GetComponent<Player>().gameObject.name.Equals("Mechanic") && target.GetComponentInParent<DoorScript>().personalDoor)
                        {
                            if (Input.GetKeyDown(KeyCode.F) && !StateChecker.isGhost && !GetComponent<Rat>())
                            {
                                target.GetComponentInParent<DoorScript>().Activate();
                            }
                        }
                    }
                }
                //manager door
                if (doorScript.whoDoor.Equals("Manager") && target.GetComponentInParent<DoorScript>().personalDoor)
                {
                    if (doorScript.repairing)
                    {
                        UpdateReticleDisplay(keyIcon, "Unlocking", "Needs Manager");
                    }
                    else
                    {
                        UpdateReticleDisplay(keyIcon, "Press F to Open", "Needs Manager");

                        if (GetComponent<Player>().gameObject.name.Equals("Manager") && target.GetComponentInParent<DoorScript>().personalDoor)
                        {
                            if (Input.GetKeyDown(KeyCode.F) && !StateChecker.isGhost && !GetComponent<Rat>())
                            {
                                target.GetComponentInParent<DoorScript>().Activate();
                            }
                        }
                    }
                }
                //locked doors
                if (target.GetComponentInParent<DoorScript>().hasKey && target.GetComponentInParent<DoorScript>().isLocked)
                {
                    UpdateReticleDisplay(keyIcon, "Press F to Open", "Needs Key");

                    if (target.GetComponentInParent<DoorScript>().unlocking)
                    {
                        //flip
                        UpdateReticleDisplay(keyIcon, "Press F to Open", "Used Key");
                    }
                    else
                    {
                        UpdateReticleDisplay(keyIcon, "Press F to Open", "Needs Key");
                        if (Input.GetKeyDown(KeyCode.F) && !StateChecker.isGhost && !GetComponent<Rat>())
                        {
                            target.GetComponentInParent<DoorScript>().Activate();
                        }
                    }
                }
                if (target.GetComponentInParent<DoorScript>().isOpen)
                {
                    UpdateReticleDisplay(keyIcon, "Press F to Close", "");

                    //open
                    if (Input.GetKeyDown(KeyCode.F) && !StateChecker.isGhost && !GetComponent<Rat>())
                    {
                        target.GetComponentInParent<DoorScript>().Activate();
                    }
                }
                else
                {
                    UpdateReticleDisplay(keyIcon, "Press F to Open", "");

                    //open
                    if (Input.GetKeyDown(KeyCode.F) && !StateChecker.isGhost && !GetComponent<Rat>())
                    {
                        target.GetComponentInParent<DoorScript>().Activate();
                    }

                }
            }

   

            //default clear display
            else
            {
                ClearReticleDisplay();
            }
        }
    }

    void UpdateReticleDisplay(Sprite displayIcon, string interactText, string requireText)
    {
        playerInteractText.text = interactText;
        playerRequireText.text = requireText;
        playerDisplayIcon.sprite = displayIcon;

        playerInteractText.color = Color.Lerp(playerLocationText.color, Color.white, fadeTime * Time.deltaTime);
        playerRequireText.color = Color.Lerp(playerLocationText.color, Color.white, fadeTime * Time.deltaTime);
        playerDisplayIcon.color = Color.Lerp(playerLocationText.color, Color.white, fadeTime * Time.deltaTime);

        hoverAudio.volume = Mathf.Lerp(hoverAudio.volume, hoverAudioVolume, fadeTime * Time.deltaTime);
    }
    void UpdateSingleText(string text)
    {
        singleText.text = text;

        singleText.color = Color.Lerp(playerLocationText.color, Color.white, fadeTime * Time.deltaTime);

        hoverAudio.volume = Mathf.Lerp(hoverAudio.volume, hoverAudioVolume, fadeTime * Time.deltaTime);
    }

    void ClearReticleDisplay()
    {
        playerInteractText.color = Color.Lerp(playerLocationText.color, Color.clear, fadeTime * Time.deltaTime);
        playerRequireText.color = Color.Lerp(playerLocationText.color, Color.clear, fadeTime * Time.deltaTime);
        playerDisplayIcon.color = Color.Lerp(playerLocationText.color, Color.clear, fadeTime * Time.deltaTime);
        singleText.color = Color.Lerp(playerLocationText.color, Color.clear, fadeTime * Time.deltaTime);


        hoverAudio.volume = Mathf.Lerp(hoverAudio.volume, 0f, fadeTime * Time.deltaTime);
    }
    */

    void Door()
    {

    }
    private void OnTriggerStay(Collider other)
    {

        //if(other.gameObject.CompareTag("Location"))
        //{
        //    playerLocationText.text = other.gameObject.name;
        //    //print(playerLocationText);
        //    //leftDash.GetComponent<Image>().color = Color.Lerp(playerLocationText.color, Color.white, fadeTime * Time.deltaTime);
        //    //rightDash.GetComponent<Image>().color = Color.Lerp(playerLocationText.color, Color.white, fadeTime * Time.deltaTime);
        //    //print(playerLocationText.textBounds);
        //    showLocation = true;
        //}
        //else
        //{
        //    showLocation = false;
        //}
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Location"))
        {
            //leftDash.GetComponent<Image>().color = Color.Lerp(playerLocationText.color, Color.clear, fadeTime * Time.deltaTime);
            //rightDash.GetComponent<Image>().color = Color.Lerp(playerLocationText.color, Color.clear, fadeTime * Time.deltaTime);
        }
    }

}
