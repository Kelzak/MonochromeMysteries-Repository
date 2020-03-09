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

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        canMove = true;

        cc = GetComponent<CharacterController>();
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void Ability()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if(Input.GetButtonDown("PickUp"))
        {
            if (other.gameObject.tag == "Selectable" && other.gameObject.GetComponent<Item>().itemName == "Key")
            {
                Debug.Log("Rat should be picking up key");
                other.transform.SetParent(pickupDestination);
            }
        }
        if (Input.GetButtonUp("PickUp"))
        {
            other.transform.SetParent(null);
        }
    }

    private void OnDisable()
    {

    }

}
