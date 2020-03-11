/* Name: Rat.cs
 * Author: Zackary Seiple
 * Description: Handles the basic functions of the rat character including offsetting it's smaller height to work with the camera.
 * Last Updated: 2/18/2020 (Zackary Seiple)
 * Changes: Added Header
 */
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rat : Possessable
{
    private readonly float camOffsetForward = -0.5f;

    private CharacterController cc;

    //Kevon's Additions
    public Transform pickupDestination;
    public bool hasKey = false;

    public int squeakInterval;
    public float squeakVolume;
    private AudioSource audioSource;
    public AudioClip[] squeaks;

    // Start is called before the first frame update
    protected override void Start()
    {
        int rand = Random.Range(squeakInterval / 2, squeakInterval * 2);
        audioSource = this.GetComponent<AudioSource>();
        InvokeRepeating("Squeak", 5f, squeakInterval);
        base.Start();

        canMove = true;

        cc = GetComponent<CharacterController>();
    }

    protected override void Update()
    {
        base.Update();
        if(hasKey)
        {
            
        }
    }

    public override void Ability()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if(Input.GetButtonDown("PickUp")) //which is "F"
        {
            if (other.gameObject.tag == "Selectable" && other.gameObject.GetComponent<Item>().itemName == "Key")
            {
                hasKey = true;
                Debug.Log("Rat should be picking up key");
                other.transform.SetParent(pickupDestination);
                other.transform.position = pickupDestination.position;
                other.transform.rotation = pickupDestination.rotation;
                other.gameObject.GetComponent<Rigidbody>().useGravity = false;
                other.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            }
        }
        if (Input.GetButtonUp("PickUp"))
        {
            other.transform.SetParent(null);
            other.gameObject.GetComponent<Rigidbody>().useGravity = true;
            other.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    private void OnDisable()
    {

    }
    void Squeak()
    {
        int rand = Random.Range(0, squeaks.Length);
        audioSource.volume = squeakVolume;
        audioSource.PlayOneShot(squeaks[rand]);
        SqueakRand();
    }

    void SqueakRand()
    {
        int rand = Random.Range(squeakInterval / 2, squeakInterval * 2);
        Invoke("Squeak", rand);
    }

}
