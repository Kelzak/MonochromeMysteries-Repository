//Made by matt kirchoff

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Read : MonoBehaviour
{
    private Photographer photographer;
    private Endings ending;
    private Player player;

    public GameObject[] toRead;
    private GameObject Background;

    public GameObject flipLeftIcon;
    public GameObject flipRightIcon;

    [HideInInspector]
    public bool readTime;

    private bool closeTime;

    private bool isOpen;
    private int index;

    private AudioSource audioSource;
    public AudioClip[] flipClip;
    public AudioClip openClip;
    public AudioClip closeClip;

    [Range(0.0f, 1.0f)]
    public float soundVolume = .5f;

    // Start is called before the first frame update
    void Start()
    {
        //flipLeftIcon = GameController.mainHUD.transform.Find("LeftMouseIcon").GetComponent<GameObject>();
        //flipRightIcon = GameController.mainHUD.transform.Find("RightMouseIcon").GetComponent<GameObject>();
        photographer = FindObjectOfType<Photographer>();
        ending = FindObjectOfType<Endings>();
        player = FindObjectOfType<Player>();
        Background = player.darkBackground;
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = soundVolume;

        audioSource.rolloffMode = AudioRolloffMode.Custom;
        audioSource.minDistance = 0f;
        audioSource.maxDistance = 5f;
        audioSource.spatialBlend = .5f;
    }

    // Update is called once per frame
    void Update()
    {
        //fliping
        if(isOpen)
        {
            //close readable
            if ((Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.F) && readTime))
            {
                Debug.Log("close");
                Close();
            }

            //flipping
            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Space))
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
            if (Input.GetMouseButtonDown(0))
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
            if (toRead.Length >= 1 && index != 0)
            {
                flipLeftIcon.SetActive(true);
            }
            else if(!isOpen)
            {
                flipLeftIcon.SetActive(false);
            }
            if (toRead.Length >= 1 && index != toRead.Length-1)
            {
                flipRightIcon.SetActive(true);
            }
            else if(!isOpen)
            {
                flipRightIcon.SetActive(false);
            }
        }
        else
        {
            if(toRead.Length > 1)
            {
                flipLeftIcon.SetActive(false);
                flipRightIcon.SetActive(false);
            }   
        }
    }

   

    public void Open()
    {
        if(!closeTime)
        {
            isOpen = true;
            index = 0;
            toRead[index].SetActive(true);

            if (player.GetComponent<Photographer>())
            {
                photographer.CameraLensActive = false;
                photographer.canTakePhoto = false;
            }

            GameController.TogglePause();
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            Player.EnableControls(false);
            Background.SetActive(true);
            StartCoroutine(ReadTime());


            audioSource.PlayOneShot(openClip);
        }
        
    }

    public void Close()
    {
        if(readTime)
        {
            if (player.GetComponent<Photographer>())
            {
                photographer.CameraLensActive = true;
                photographer.canTakePhoto = true;
            }

            foreach (GameObject page in toRead)
            {
                page.SetActive(false);
            }

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Player.EnableControls(true);
            Background.SetActive(false);
            GameController.TogglePause();
            isOpen = false;
            audioSource.PlayOneShot(closeClip);
            readTime = false;
            closeTime = true;
            flipLeftIcon.SetActive(false);
            flipRightIcon.SetActive(false);
            StartCoroutine(CloseTime());
        }

    }
    public IEnumerator ReadTime()
    {
        yield return new WaitForSecondsRealtime(.1f);
        readTime = true;
    }
    public IEnumerator CloseTime()
    {
        yield return new WaitForSecondsRealtime(.1f);
        closeTime = false;
    }
    //chooses random int for flipping page sound
    private int RandFlip()
    {
        int temp = Random.Range(0,flipClip.Length-1);
        return temp;
    }
}
