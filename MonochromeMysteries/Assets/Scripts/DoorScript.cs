using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{

    private Animator _animator;

    public GameObject OpenPanel = null;

    private bool _isInsideTrigger = false;

    private bool isPlayer;
    public bool isLocked = false;

    public GameObject key;

    public AudioClip[] openDoor;
    public AudioClip[] closeDoor;
    public AudioClip[] unlockDoor;
    public AudioClip[] lockedDoor;
    private AudioSource audioSource;

    private int rand;
    private AudioClip sound;

    public bool isOpen;
    public bool autoClose = true;
    public bool stayOpen;

    public bool personalDoor;
    //match this with the name of the game object for the character whos door it is
    public string whoDoor;

    // Use this for initialization
    void Start()
    {
        stayOpen = false;
        _animator = transform.Find("Hinge").GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        Debug.Log(isPlayer);
    }

    void OnTriggerStay(Collider other)
    {
        Debug.Log("by door");
        if (personalDoor)
        {
            Debug.Log("by manager door");

            if (other.gameObject.name.Equals(whoDoor))
            {
                Debug.Log("Should work");
                //personalDoor = true;
                _isInsideTrigger = true;
                //OpenPanel.SetActive(true);
                isPlayer = true;
            }
            else
            {
                //personalDoor = false;
                _isInsideTrigger = true;
                // isPlayer = false;
            }
        }
        /*
        else //if (other.tag == "Person")
        {
            if(other.GetComponent<Player>())
            {
                _isInsideTrigger = true;
                //OpenPanel.SetActive(true);
                isPlayer = true;
                personalDoor = false;

            }
            else
            {
                _isInsideTrigger = true;
                isPlayer = false;
                personalDoor = false;

            }
        }*/
    }

    private bool IsOpenPanelActive
    {
        get
        {
            return OpenPanel.activeInHierarchy;
        }
    }
    public void Activate()
    {
        if (personalDoor && isPlayer && !isOpen)
        {
            Debug.Log("should open");
            Open();
        }
        //regular open
        else if (!isLocked && !isOpen)
        {
            Open();
        }
        //locked open
        else if (isLocked && !isOpen && Player.keys.Contains(key))
        {
            isLocked = false;
            Open();
        }
        //close door if able
        else if (isOpen && !stayOpen)
        {
            _animator.SetBool("open", false);

            Invoke("DoorShut", 1.5f);

            isOpen = false;
        }
        //door locked
        else if (isLocked && !isOpen)
        {
            rand = Random.Range(0, lockedDoor.Length);
            sound = lockedDoor[rand];
            audioSource.PlayOneShot(sound);
            Log.AddEntry("The Door is Locked");
        }

    }
    private void Open()
    {
        _animator.SetBool("open", true);

        int rand = Random.Range(0, openDoor.Length);
        AudioClip sound = openDoor[rand];
        audioSource.PlayOneShot(sound);

        isOpen = true;
    }

    // Update is called once per frame
    void Update()
    {

    }
    void DoorShut()
    {
        rand = Random.Range(0, closeDoor.Length);
        sound = closeDoor[rand];
        audioSource.PlayOneShot(sound);
    }

    public void Unlock()
    {
        isLocked = false;
        //OpenPanel.SetActive(false);
        _animator.SetBool("open", true);
        rand = Random.Range(0, openDoor.Length);
        sound = openDoor[rand];
        audioSource.PlayOneShot(sound);
        isOpen = true;
    }
}
