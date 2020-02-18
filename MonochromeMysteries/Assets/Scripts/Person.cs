/* Name: Person.cs
 * Author: Zackary Seiple
 * Description: Handles the basic behaviour of humanoid possessable GameObjects
 * Last Updated: 2/18/2020 (Zackary Seiple)
 * Changes: Added Header
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Person : Possessable
{
    private CharacterController cc;

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

}
