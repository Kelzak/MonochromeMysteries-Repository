using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{

    private Animator _animator;

    public GameObject OpenPanel = null;

    private bool _isInsideTrigger = false;

    private bool isPlayer = false;
    public bool isLocked = false;

    public GameObject key;

    public AudioClip[] openDoor;
    public AudioClip[] closeDoor;
    public AudioClip[] unlockDoor;
    public AudioClip[] lockedDoor;
    private AudioSource audioSource;

    private int rand;
    private AudioClip sound;

    private bool isOpen;

    // Use this for initialization
    void Start()
    {
        _animator = transform.Find("Hinge").GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Person")
        {
            if(other.GetComponent<Player>())
            {
                _isInsideTrigger = true;
                //OpenPanel.SetActive(true);
                isPlayer = true;
            }
            else
            {
                _isInsideTrigger = true;
                isPlayer = false;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Person" && isOpen)
        {
            _isInsideTrigger = false;
            _animator.SetBool("open", false);
            Invoke("DoorShut", 1.5f);
            //OpenPanel.SetActive(false);
            isOpen = false;
        }
    }

    private bool IsOpenPanelActive
    {
        get
        {
            return OpenPanel.activeInHierarchy;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayer && _isInsideTrigger && !isLocked && !isOpen)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                //OpenPanel.SetActive(false);
                _animator.SetBool("open", true);

                int rand = Random.Range(0, openDoor.Length);
                AudioClip sound = openDoor[rand];
                audioSource.PlayOneShot(sound);

                isOpen = true;
            }
        }
        if (isPlayer && _isInsideTrigger && isLocked)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if(Player.keys.Contains(key))
                {
                    rand = Random.Range(0, unlockDoor.Length);
                    sound = unlockDoor[rand];
                    audioSource.PlayOneShot(sound);
                    Unlock();
                }
                else
                    rand = Random.Range(0, lockedDoor.Length);
                    sound = lockedDoor[rand];
                    audioSource.PlayOneShot(sound);
            }
        }

        if (_isInsideTrigger && !isPlayer && !isLocked && !isOpen)
        {
            _animator.SetBool("open", true);
            rand = Random.Range(0, openDoor.Length);
            sound = openDoor[rand];
            audioSource.PlayOneShot(sound);
            isOpen = true;

        }
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
