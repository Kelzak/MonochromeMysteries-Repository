/*
 * Created by Matt Kirchoff
 * item script for outlining during ghost vision
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//observer to state checker to display outline on item
[RequireComponent(typeof(Outline))]
[RequireComponent(typeof(MeshCollider))]

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

    private bool isGhost;
    private bool isClose;

    [Header("UI Settings")]
    public Image icon;
    public string topText;
    public string bottomText;
    public string singleText;
    public bool onlySingleText;

    [Header("Photo clue Settings")]
    public string itemName;
    public string itemDescription;


    [Header("Item Settings")]
    public bool isPickup;
    public bool isReadable;

    [Header("Audio Settings")]
    public AudioSource hoverAudio;
    [Range(0, 1)]
    public float hoverAudioVolume = .5f;
    public float audioFadeTime = 1f;

    [Header("Outline Settings")]
    public float glowWidth = 8f;
    public float fadeTime = 3f;
    public Color glowColor = Color.white;
    public bool glowOverride;

    private Outline outline;


    // Start is called before the first frame update
    void Start()
    {
        //gameObject.GetComponent<ItemAbs>().Activate();
        outline = GetComponent<Outline>();

        outline.OutlineWidth = glowWidth;

        //set item nameif its null
        itemName = itemName == null ? gameObject.name : this.itemName;

        //player = GameObject.FindObjectOfType<Player>();
        // adds this object to observer list in statechecker

        playerDistGlow = Player.reticleDist;
    }

    // Update is called once per frame
    void Update()
    {
        player = FindObjectOfType<Player>().gameObject;

        playerDistance = Vector3.Distance(this.transform.position, player.transform.position);

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

    //sets ui, often called from an item abstract
    public void SetUI(Image icon, string topText, string bottomText, string singleText, bool onlySingleText)
    {
        //if bool onlysingletext then update singletext line and null others 
        if(onlySingleText)
        {
            this.icon.color = Color.clear;
            this.onlySingleText = onlySingleText;
            this.singleText = singleText;
            this.icon = null;
            this.topText = null;
            this.bottomText = null;
            return;
        }
        //else update other and null single lines
        else
        {
            this.icon = icon;
            this.topText = topText;
            this.bottomText = bottomText;
            this.singleText = null;
            this.onlySingleText = onlySingleText;
            return;
        }
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

