using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Person : Possessable
{
    private CharacterController cc;

    void Start()
    {
        canMove = true;

        cc = GetComponent<CharacterController>();
    }

}
