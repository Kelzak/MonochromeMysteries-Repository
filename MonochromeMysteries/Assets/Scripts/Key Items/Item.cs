/*
 * Created by Matt Kirchoff
 * item script for outlining during ghost vision
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Outline))]
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(AudioSource))]

//abstract item class that all other items inherit. Items that have no activation, would simply leave that method empty when implementing an item
public abstract class ItemAbs : MonoBehaviour
{
    public abstract void Activate();

    public abstract void SetItemUI();

}

public class Item : MonoBehaviour
{
    private float playerDistance;
    private float playerDistGlow;
    private GameObject player;
    //public Image reticle;

    [Header("UI Settings")]
    public Sprite icon;
    public string topText;
    public string bottomText;
    public string singleText;
    public bool onlySingleText;

    [Header("Photo clue Settings")]
    public string itemName;
    public string itemDescription;


    //[Header("Item Settings")]
    //public bool isPickup;
    //public bool isReadable;

    [Header("Outline Settings")]
    public float glowWidth = 8f;
    public float fadeTime = 3f;
    public Color glowColor = Color.white;
    public bool glowOverride;

    private Outline outline;


    // Start is called before the first frame update
    void Start()
    {
        //default icon to camera
        if(icon == null)
        {
            icon = FindObjectOfType<UIspriteManager>().cameraSprite;
        }
        //outline stuff
        //gameObject.GetComponent<ItemAbs>().Activate();
        outline = GetComponent<Outline>();
        //set outline wifth to 0 to avoid outlines at start
        outline.OutlineWidth = 0f;
        outline.OutlineColor = glowColor;

        //set item nameif its null
        if(itemName == "" || itemName == null)
        {
            itemName = gameObject.name;
        }
        playerDistGlow = Player.reticleDist;

    }

    // Update is called once per frame
    void Update()
    {
        player = FindObjectOfType<Player>().gameObject;


        playerDistance = Vector3.Distance(this.transform.position, player.transform.position);

        // if() item is in the raycast hit array?

        //enables outline when player is ghost and nearby, or when player overrides glow
        if ((StateChecker.isGhost && playerDistance < playerDistGlow) || glowOverride)
        {
            //GetComponent<Outline>().enabled = true;
            outline.OutlineWidth = Mathf.Lerp(outline.OutlineWidth, glowWidth, fadeTime * Time.deltaTime);

        }
        //else outline is turned off
        else
        {
            outline.OutlineWidth = Mathf.Lerp(outline.OutlineWidth, 0f, fadeTime * Time.deltaTime);
            //GetComponent<Outline>().enabled = false;
        }
    }

    private void FixedUpdate()
    {
        
    }

    //sets ui, often called from an item abstract
    public void SetUI(Sprite icon, string topText, string bottomText, string singleText, bool onlySingleText)
    {
        this.icon = icon;
        this.topText = topText;
        this.bottomText = bottomText;
        this.singleText = singleText;
        this.onlySingleText = onlySingleText;
    }
    private void OnTriggerStay(Collider other)
    {
        if (Input.GetButton("PickUp") && other.gameObject.tag == "Rat")
        {
            GetComponent<Outline>().enabled = false;
        }
        if (Input.GetButtonUp("PickUp") && other.gameObject.tag == "Rat")
        {
            GetComponent<Outline>().enabled = true;
        }

    }
}

