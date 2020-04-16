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
    public AudioSource stepsSource;
    public AudioSource squeakSource;
    public AudioClip[] squeaks;
    public AudioClip obtainClip;

    private bool hold;
    private GameObject target;

    // Start is called before the first frame update
    protected override void Start()
    {
        int rand = Random.Range(squeakInterval / 2, squeakInterval * 2);
        InvokeRepeating("Squeak", 5f, squeakInterval);
        base.Start();
        verticalClamp = 30f;

        canMove = true;

        cc = GetComponent<CharacterController>();
    }

    protected override void Update()
    {
        base.Update();
        if(hasKey)
        {
            
        }
        if (hold)
        {
            target.gameObject.GetComponentInChildren<HoverText>().UIstop = true;
            //Debug.Log("Rat should be picking up key or knife");
            target.transform.SetParent(pickupDestination);
            target.transform.position = pickupDestination.position;
            target.transform.rotation = pickupDestination.rotation;
            target.gameObject.GetComponent<Rigidbody>().useGravity = false;
            target.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            hasKey = true;
        }
        else
        {
            try
            {
                target.gameObject.GetComponentInChildren<HoverText>().UIstop = false;
                //Debug.Log("Let go");
                target.transform.SetParent(null);
                target.gameObject.GetComponent<Rigidbody>().useGravity = true;
                target.gameObject.GetComponent<Rigidbody>().isKinematic = false;
                hasKey = false;
            }
            catch (System.Exception)
            {

                //throw;
            }
            
            
        }
    }

    public override void Ability()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if(Input.GetButtonDown("PickUp") && (other.gameObject.tag == "Key" || other.gameObject.tag == "letter" || other.gameObject.tag == "Knife") && !hasKey)
        {
            squeakSource.PlayOneShot(obtainClip);
            target = other.gameObject;
            hold = true;
        }
        else if ((Input.GetButtonDown("PickUp") && (other.gameObject.tag == "Key" || other.gameObject.tag == "letter" || other.gameObject.tag == "Knife") && hasKey) || Input.GetKeyDown(KeyCode.Q) && other.gameObject.tag == "Key" && hasKey)
        {
            squeakSource.PlayOneShot(obtainClip);
            target = other.gameObject;
            hold = false;
        }
        //else
        //    target = null;


        //if (Input.GetButtonDown("PickUp")) //which is "F"
        //{
        //    if (other.gameObject.tag == "Key" && !hasKey)
        //    {
        //        Debug.Log("Rat should be picking up key");
        //        other.transform.SetParent(pickupDestination);
        //        other.transform.position = pickupDestination.position;
        //        other.transform.rotation = pickupDestination.rotation;
        //        other.gameObject.GetComponent<Rigidbody>().useGravity = false;
        //        other.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        //        hasKey = true;
        //    }
        //    else if (other.gameObject.tag == "Key" && hasKey)
        //    {
        //        Debug.Log("Let go");
        //        other.transform.SetParent(null);
        //        other.gameObject.GetComponent<Rigidbody>().useGravity = true;
        //        other.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        //        hasKey = false;
        //    }
        //}

    }

   


    private void OnDisable()
    {

    }
    void Squeak()
    {
        int rand = Random.Range(0, squeaks.Length);
        squeakSource.volume = squeakVolume;
        squeakSource.PlayOneShot(squeaks[rand]);
        SqueakRand();
    }

    void SqueakRand()
    {
        int rand = Random.Range(squeakInterval / 2, squeakInterval * 2);
        Invoke("Squeak", rand);
    }

}
