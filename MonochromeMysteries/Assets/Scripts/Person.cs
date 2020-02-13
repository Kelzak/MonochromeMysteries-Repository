using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Person : Possessable
{
    private CharacterController cc;

    protected override void Awake()
    {
        base.Awake();
    }

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
