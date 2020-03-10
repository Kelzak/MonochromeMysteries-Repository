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

    // Use this for initialization
    void Start()
    {
        _animator = transform.Find("Door").GetComponent<Animator>();

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
        if (other.tag == "Person")
        {
            _isInsideTrigger = false;
            _animator.SetBool("open", false);
            //OpenPanel.SetActive(false);
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

        if (isPlayer && _isInsideTrigger && !isLocked)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                //OpenPanel.SetActive(false);
                _animator.SetBool("open", true);
            }
        }

        if (_isInsideTrigger && !isPlayer && !isLocked)
        {
            _animator.SetBool("open", true);
        }
    }
}
