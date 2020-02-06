using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rat : Possessable
{
    private float camOffsetForward = -0.5f;

    private CharacterController cc;

    // Start is called before the first frame update
    void Start()
    {
        canMove = true;



        cc = GetComponent<CharacterController>();
    }

    private void Update()
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
