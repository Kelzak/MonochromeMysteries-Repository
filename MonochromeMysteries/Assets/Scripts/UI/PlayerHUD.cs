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

    [Header("Variables")]
    public float fadeTime = 3f;


    [Header("Player Display Objects / Text")]
    public TMP_Text playerLocationText;
    public TMP_Text playerInteractText;
    public TMP_Text playerRequireText;
    //text with no icon directly in the middle of the screen so it looks nicer
    public TMP_Text singleText;
    public Image playerDisplayIcon;
    private bool showLocation;

    [Header("Icons")]
    public Sprite cameraIcon;
    public Sprite toolIcon;
    public Sprite keyIcon;
    public Sprite readIcon;

    //location HUD
    public GameObject leftDash;
    public GameObject rightDash;

    [Header("Audio")]
    //other
    public AudioSource hoverAudio;
    [Range(0,1)]
    public float hoverAudioVolume = .5f;


    // Start is called before the first frame update
    void Start()
    {
        //hoverAudio.volume = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(showLocation)
        {
            playerLocationText.color = Color.Lerp(playerLocationText.color, Color.white, fadeTime * Time.deltaTime);

        }
        else
        {
            playerLocationText.color = Color.Lerp(playerLocationText.color, Color.clear, fadeTime * Time.deltaTime);

        }



        //Raycast();
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
        if(other.gameObject.CompareTag("Location"))
        {
            playerLocationText.text = other.gameObject.name;
            //print(playerLocationText);
            //leftDash.GetComponent<Image>().color = Color.Lerp(playerLocationText.color, Color.white, fadeTime * Time.deltaTime);
            //rightDash.GetComponent<Image>().color = Color.Lerp(playerLocationText.color, Color.white, fadeTime * Time.deltaTime);
            //print(playerLocationText.textBounds);
            showLocation = true;
        }
        else
        {
            showLocation = false;
        }
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
