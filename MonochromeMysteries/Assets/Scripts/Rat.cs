using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rat : Possessable
{
    private float camOffsetForward = -0.5f;

    private CharacterController cc;

    protected override void Awake()
    {
        base.Awake();
    }

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
        //None
    }

    private void OnDisable()
    {
    }

}
