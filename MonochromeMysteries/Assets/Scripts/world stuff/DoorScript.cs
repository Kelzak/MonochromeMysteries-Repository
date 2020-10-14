/* Name: Player.cs
 * Author: Matt Kirchoff
 * Description: This script handles door interactions
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : ItemAbs
{
    [Header("Door information")]
    public Animator _animator;
    private float id;
    private bool _isInsideTrigger = false;
    private bool isPlayer;
    public Player player;

    [Header("Lock and Key")]
    public bool isLocked = false;
    public GameObject key;


    [Header("Door Settings")]
    [HideInInspector]
    public bool isOpen;
    //public bool autoClose = true;
    public bool stayOpen;
    public bool brokendoor = false;
    private bool repairing = false;
    public bool personalDoor;
    //match this with the name of the game object for the character whos door it is
    public GameObject whoDoor;
    private bool unlocking;
    [HideInInspector]
    public bool doorSaveOpen = false;

    [Header("UI Settings")]
    public Sprite keyIcon;
    public Sprite keyIconFlip;
    public Sprite repairIcon;
    public Sprite repairIconFlip;

    [Header("Audio Settings")]
    public AudioClip[] openDoor;
    public AudioClip[] closeDoor;
    public AudioClip[] unlockDoor;
    public AudioClip[] lockedDoor;
    public AudioClip repairClip;
    private AudioSource audioSource;

    private int rand;
    private AudioClip sound;


    private bool waitHitbox;

    private void Awake()
    {
        id = (1000 * transform.position.x) + transform.position.y + (0.001f * transform.position.z);
    }

    // Use this for initialization
    void Start()
    {
        //get icons from sprite manager
        keyIcon = FindObjectOfType<UIspriteManager>().keySprite;
        keyIconFlip = FindObjectOfType<UIspriteManager>().keySpriteFlip;
        repairIcon = FindObjectOfType<UIspriteManager>().repairSprite;
        repairIconFlip = FindObjectOfType<UIspriteManager>().repairSpriteFlip;

        //stayOpen = false;
        //_animator = transform.parent.Find("Hinge").GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if(!isLocked && !personalDoor)
        {
            doorSaveOpen = true;
        }
        if(doorSaveOpen)
        {
            isLocked = false;
            personalDoor = false;
        }

        //set values to not contradict each other - commented out for save testing purposes
        //if(key != null)
        //{
        //    isLocked = true;
        //    personalDoor = false;
        //}
        //if (whoDoor != null)
        //{
        //    isLocked = false;
        //    personalDoor = true;
        //}
    }

    // Update is called once per frame
    void Update()
    {
        player = FindObjectOfType<Player>();

        //attempting to disable hitboxes
        if (waitHitbox == true)
            GetComponent<MeshRenderer>().GetComponent<MeshCollider>().enabled = false;
        else
            GetComponent<MeshRenderer>().GetComponent<MeshCollider>().enabled = true;
    }

    // old way for personal door detection
    //void OnTriggerStay(Collider other)
    //{
    //    //Debug.Log("by door");
    //    if (personalDoor)
    //    {
    //        Debug.Log("by manager door");

    //        if (other.gameObject.name.Equals(whoDoor))
    //        {
    //            Debug.Log("Should work");

    //            isPlayer = true;
    //        }
    //        else
    //        {
    //             isPlayer = false;
    //        }
    //    }
        
    //}

    IEnumerator WaitForSound()
    {
        
        //Wait Until Sound has finished playing

        yield return new WaitForSeconds(3);

        //Auidio has finished playing
        Debug.Log("should open");
        //isLocked = false;
        personalDoor = false;
        repairing = false;
        Open();
        
    }
    public override void Activate()
    {
        //if player is possessing the correct person for the door, then open 
        if (personalDoor && !isOpen)
        {
            if(player.gameObject.name.Equals(whoDoor.name))
            {
                //Debug.Log("personal door activate");
                if (whoDoor.name.Equals("Mechanic") && !repairing) //mechanic branch
                {
                    Log.AddEntry("Repairing Door...");
                    repairing = true;
                    audioSource.PlayOneShot(repairClip);
                    StartCoroutine(WaitForSound());
                }
                else if (whoDoor.Equals("Manager"))//manager branch
                {
                    Debug.Log("manager open");
                    Log.AddEntry("Unlocked Office");
                    //isLocked = false;
                    personalDoor = false;
                    Open();
                }
                else
                {
                    personalDoor = false;
                    Open();
                }
            }
            else
            {
                rand = Random.Range(0, lockedDoor.Length);
                sound = lockedDoor[rand];
                audioSource.PlayOneShot(sound);
                Log.AddEntry("The Door requires the " + whoDoor.name);

            }


        }
        //regular open
        else if (!isLocked && !isOpen && !personalDoor)
        {
            Open();
        }
        //locked open
        else if (isLocked && !isOpen && Player.keys.Contains(key))
        {
            Log.AddEntry("Used: " + key.GetComponent<Item>().itemName);
            //isLocked = false;

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
            if (key != null)
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
                //if door is not open, and saved data is, open it
                if(!this.isOpen & data.open[i])
                {
                    Open();
                }
                this.isOpen = data.open[i];
                //if door save data is open, set restrictive bools off
                if(data.saveOpen[i])
                {
                    this.isLocked = false;
                    this.personalDoor = false;
                }
                this.doorSaveOpen = data.saveOpen[i];
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
        unlocking = false;
        isLocked = false;
        isOpen = true;
        personalDoor = false;

        doorSaveOpen = true;
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

    //UI managment
    public override void SetItemUI()
    {
        if(!StateChecker.isGhost && !player.GetComponent<Rat>())
        {
            GetComponent<Item>().dontChangeReticleColor = false;
            //lock and key
            if (isLocked)
            {
                if (key != null)
                {
                    if (Player.keys.Contains(key))
                        GetComponent<Item>().SetUI(keyIcon, "Press F to Unlock", "Use Key", "", false);
                    else
                        GetComponent<Item>().SetUI(keyIcon, "Press F to Unlock", "Needs Key", "", false);
                }
                else if (unlocking)
                {
                    GetComponent<Item>().SetUI(keyIconFlip, "Unlocking", "Used Key", "", false);
                }
                else
                {
                    GetComponent<Item>().SetUI(null, null, null, "Press F to Open", true);
                }
            }
            else if (personalDoor)
            {
                if (whoDoor.name.Equals("Mechanic"))
                {
                    GetComponent<Item>().SetUI(repairIcon, "Press F to Repair", "Needs " + whoDoor.name, "", false);
                }
                else
                {
                    GetComponent<Item>().SetUI(keyIcon, "Press F to Open", "Needs " + whoDoor.name, "", false);
                }
            }
            else if (repairing)
            {
                if (whoDoor.name.Equals("Mechanic"))
                {
                    GetComponent<Item>().SetUI(repairIconFlip, "Repairing", "Needs " + whoDoor.name, "", false);
                }
                else
                {
                    GetComponent<Item>().SetUI(keyIcon, "Press F to Open", "Needs " + whoDoor.name, "", false);
                }
            }
            //default open and close
            else if (isOpen)
                GetComponent<Item>().SetUI(null, null, null, "Press F to Close", true);
            else
                GetComponent<Item>().SetUI(null, null, null, "Press F to Open", true);
        }
        //clear UI changes for when rat or ghost
        else
        {
            GetComponent<Item>().dontChangeReticleColor = true;
            GetComponent<Item>().SetUI(null, null, null, "", true);

        }


    }
}
