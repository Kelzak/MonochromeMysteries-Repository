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

    public AudioClip openDoor;
    public AudioClip closeDoor;
    private AudioSource audioSource;

    private bool isOpen;

    // Use this for initialization
    void Start()
    {
        _animator = transform.Find("Door").GetComponent<Animator>();
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
            if (Input.GetKeyDown(KeyCode.E))
            {
                //OpenPanel.SetActive(false);
                _animator.SetBool("open", true);
                audioSource.PlayOneShot(openDoor);
                isOpen = true;
            }
        }

        if (_isInsideTrigger && !isPlayer && !isLocked && !isOpen)
        {
            _animator.SetBool("open", true);
            audioSource.PlayOneShot(openDoor);
            isOpen = true;

        }
    }
    void DoorShut()
    {
        audioSource.PlayOneShot(closeDoor);
    }

    public void Unlock()
    {
        isLocked = false;
    }
}
