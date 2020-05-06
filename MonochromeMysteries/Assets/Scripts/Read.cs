//Made by matt kirchoff

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class Read : MonoBehaviour
{
    private Photographer photographer;
    private Endings ending;
    private Player player;

    public GameObject[] toRead;
    private GameObject Background;
    private GameObject pressEscToCloseText;


    public GameObject flipLeftIcon;
    public GameObject flipRightIcon;

    [HideInInspector]
    public bool readTime;

    private bool closeTime;

    private bool isOpen;
    public static bool isReading;
    private int index;

    private AudioSource audioSource;
    public AudioClip[] flipClip;
    public AudioClip openClip;
    public AudioClip closeClip;

    [Range(0.0f, 1.0f)]
    public float soundVolume = .5f;

    private void Awake()
    {
        flipLeftIcon = GameObject.FindGameObjectWithTag("leftFlip");
        flipRightIcon = GameObject.FindGameObjectWithTag("rightFlip");
    }
    // Start is called before the first frame update
    void Start()
    {
        //flipLeftIcon = GameController.mainHUD.transform.Find("LeftMouseIcon").GetComponent<GameObject>();
        //flipRightIcon = GameController.mainHUD.transform.Find("RightMouseIcon").GetComponent<GameObject>();
        photographer = FindObjectOfType<Photographer>();
        ending = FindObjectOfType<Endings>();
        player = FindObjectOfType<Player>();
        Background = player.darkBackground;
        pressEscToCloseText = GameObject.Find("HUD").transform.Find("PressEscToClose").gameObject;
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = soundVolume;

        audioSource.rolloffMode = AudioRolloffMode.Custom;
        audioSource.minDistance = 0f;
        audioSource.maxDistance = 5f;
        audioSource.spatialBlend = .5f;

        //Debug.Log(flipLeftIcon.name);
        flipLeftIcon.GetComponent<Image>().enabled = false;
        flipLeftIcon.GetComponentInChildren<TMP_Text>().enabled = false;
        flipRightIcon.GetComponent<Image>().enabled = false;
        flipRightIcon.GetComponentInChildren<TMP_Text>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        //fliping
        if(isOpen)
        {
            //close readable
            if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.F) && readTime))
            {
                Debug.Log("close");
                Close();
            }

            //flipping
            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                toRead[index].SetActive(false);
                if (index >= toRead.Length - 1)
                {
                    foreach (GameObject page in toRead)
                    {
                        page.SetActive(false);
                    }
                    index = toRead.Length - 1;
                    toRead[index].SetActive(true);
                }
                else
                {
                    audioSource.PlayOneShot(flipClip[RandFlip()]);
                    index++;
                    toRead[index].SetActive(true);
                }
            }
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                toRead[index].SetActive(false);
                if (index <= 0)
                {
                    foreach (GameObject page in toRead)
                    {
                        page.SetActive(false);
                    }
                    index = 0;
                    toRead[index].SetActive(true);
                }
                else
                {
                    audioSource.PlayOneShot(flipClip[RandFlip()]);
                    index--;
                    toRead[index].SetActive(true);
                }
            }

            //show flip page icons
            if (toRead.Length >= 1 && index != 0 && isOpen)
            {
                flipLeftIcon.GetComponent<Image>().enabled = true;
                flipLeftIcon.GetComponentInChildren<TMP_Text>().enabled = true;

            }
            //else if(!isOpen)
            //{
            //    flipLeftIcon.GetComponent<Image>().enabled = false;
            //}
            if (toRead.Length >= 1 && index != toRead.Length-1 && isOpen)
            {
                flipRightIcon.GetComponent<Image>().enabled = true;
                flipRightIcon.GetComponentInChildren<TMP_Text>().enabled = true;

            }
            //else if(!isOpen)
            //{
            //    flipRightIcon.GetComponent<Image>().enabled = false;
            //}


        }
        else
        {
            if(toRead.Length > 1)
            {
                //flipLeftIcon.SetActive(false);
                //flipRightIcon.SetActive(false);
            }   
        }
    }

   

    public void Open()
    {
        if(!closeTime)
        {
            if(photographer.GetComponent<Player>())
            {
                photographer.CameraLensActive = false;
                photographer.canTakePhoto = false;
            }
            isReading = true;
            isOpen = true;
            index = 0;
            toRead[index].SetActive(true);
            //GameController.TogglePause();
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
            Player.EnableControls(false);
            Time.timeScale = 0;
            Background.SetActive(true);
            pressEscToCloseText.SetActive(true);
            StartCoroutine(ReadTime());

            if(toRead.Length > 1)
            {
                flipLeftIcon.GetComponent<Image>().enabled = true;
                flipRightIcon.GetComponent<Image>().enabled = true;
                flipLeftIcon.GetComponentInChildren<TMP_Text>().enabled = true;
                flipRightIcon.GetComponentInChildren<TMP_Text>().enabled = true;
            }
            audioSource.PlayOneShot(openClip);
        }
        
    }

    public void Close()
    {
        if(readTime)
        {
            foreach (GameObject page in toRead)
            {
                page.SetActive(false);
            }
            if (photographer.GetComponent<Player>())
            {
                photographer.CameraLensActive = true;
                StartCoroutine(WaitToTurnOnCamera());
            }
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Player.EnableControls(true);
            Time.timeScale = 1;
            Background.SetActive(false);
            pressEscToCloseText.SetActive(false);
            //GameController.TogglePause();
            isOpen = false;
            isReading = false;
            audioSource.PlayOneShot(closeClip);
            readTime = false;
            closeTime = true;
            flipLeftIcon.GetComponent<Image>().enabled = false;
            flipRightIcon.GetComponent<Image>().enabled = false;
            flipLeftIcon.GetComponentInChildren<TMP_Text>().enabled = false;
            flipRightIcon.GetComponentInChildren<TMP_Text>().enabled = false;

            StartCoroutine(CloseTime());
        }

    }
    public IEnumerator ReadTime()
    {
        yield return new WaitForSecondsRealtime(.5f);
        readTime = true;
    }
    public IEnumerator CloseTime()
    {
        yield return new WaitForSecondsRealtime(.5f);
        closeTime = false;
    }

    public IEnumerator WaitToTurnOnCamera()
    {
        yield return new WaitForSecondsRealtime(.5f);
        photographer.canTakePhoto = true;
    }

    //chooses random int for flipping page sound
    private int RandFlip()
    {
        int temp = Random.Range(0,flipClip.Length-1);
        return temp;
    }
}
