/* Name: Player.cs
 * Author: Matt Kirchoff
 * Description: This script handles door interactions
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{

    private Animator _animator;

    public GameObject OpenPanel = null;

    private float id;

    private bool _isInsideTrigger = false;

    private bool isPlayer;
    public bool isLocked = false;

    public GameObject key;

    public AudioClip[] openDoor;
    public AudioClip[] closeDoor;
    public AudioClip[] unlockDoor;
    public AudioClip[] lockedDoor;
    public AudioClip repairClip;
    private AudioSource audioSource;

    private int rand;
    private AudioClip sound;

    public bool isOpen;
    public bool autoClose = true;
    public bool stayOpen;
    public bool brokendoor = false;
    public bool repairing = false;
    public bool unlocking = false;

    public bool personalDoor;
    //match this with the name of the game object for the character whos door it is
    public string whoDoor;
    public bool hasNotBeenRepaired;

    public bool hasKey = false;
    private bool waitHitbox;

    private void Awake()
    {
        id = (1000 * transform.position.x) + transform.position.y + (0.001f * transform.position.z);
    }

    // Use this for initialization
    void Start()
    {
        stayOpen = false;
        _animator = transform.Find("Hinge").GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        if(key != null)
        {
            hasKey = true;
        }
    }

    private void FixedUpdate()
    {

    }

    void OnTriggerStay(Collider other)
    {
        //Debug.Log("by door");
        if (personalDoor)
        {
            Debug.Log("by manager door");

            if (other.gameObject.name.Equals(whoDoor))
            {
                Debug.Log("Should work");
                //personalDoor = true;
                //_isInsideTrigger = true;
                //OpenPanel.SetActive(true);
                isPlayer = true;
            }
            else
            {
                //personalDoor = false;
                //_isInsideTrigger = true;
                 isPlayer = false;
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
    IEnumerator WaitForSound()
    {
        
        //Wait Until Sound has finished playing

        yield return new WaitForSeconds(3);

        //Auidio has finished playing
        Debug.Log("should open");
        isLocked = false;
        personalDoor = false;
        repairing = false;
        Open();
        
    }
    public void Activate()
    {
        if (personalDoor && isPlayer && !isOpen)
        {
            Debug.Log("personal door activate");
            if(whoDoor.Equals("Mechanic") && !repairing) //mechanic branch
            {
                Log.AddEntry("Repairing Door...");
                repairing = true;
                audioSource.PlayOneShot(repairClip);
                StartCoroutine(WaitForSound());
            }
            else if(whoDoor.Equals("Manager"))//manager branch
            {
                Debug.Log("manager open");
                Log.AddEntry("Unlocked Office");
                isLocked = false;
                personalDoor = false;
                Open();
            }

        }
        //regular open
        else if (!isLocked && !isOpen)
        {
            Open();
        }
        //locked open
        else if (isLocked && !isOpen && Player.keys.Contains(key))
        {
            Log.AddEntry("Used: " + key.name);
            isLocked = false;

            int rand = Random.Range(0, unlockDoor.Length);
            AudioClip sound = unlockDoor[rand];
            audioSource.PlayOneShot(sound);
            unlocking = true;
            Invoke("Open", .75f);
            //Open();
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
            if (hasKey)
            {
                Log.AddEntry("The Door needs a Key");
            }
            else if (personalDoor)
            {
                if(whoDoor.Equals("Mechanic"))
                {
                    Log.AddEntry("The Door needs Repairs");
                }
                else
                {
                    Log.AddEntry("The Door needs the " + whoDoor);
                }
            }
            else
            {
                Log.AddEntry("The Door wont budge");
            }

        }

    }

    public void Load(Data.DoorData data)
    {
        for(int i = 0; i < GameController._instance.doors.Length; i++)
        {
            if(GameController._instance.doors[i].GetID() == this.id)
            {
                this.isLocked = data.locked[i];
                if(!isLocked)
                {
                    personalDoor = false;
                }
                return;
            }
        }
    }

    public float GetID()
    {
        return id;
    }

    private void Open()
    {
        _animator.SetBool("open", true);

        int rand = Random.Range(0, openDoor.Length);
        AudioClip sound = openDoor[rand];
        audioSource.PlayOneShot(sound);
        hasKey = false;
        unlocking = false;
        isOpen = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (waitHitbox == true)
            GetComponent<MeshRenderer>().GetComponent<MeshCollider>().enabled = false;
        else
            GetComponent<MeshRenderer>().GetComponent<MeshCollider>().enabled = true;
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
    IEnumerator Blah()
    {
        waitHitbox = true;
        yield return new WaitForSeconds(2);
        waitHitbox = false;
    }
}
