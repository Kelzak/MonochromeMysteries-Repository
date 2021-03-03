using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Suitcase : ItemAbs
{
    public int slot1;
    public int slot2;
    public int slot3;
    private static string correctOrder = "369";
    private static string inputtedCode = "";


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Activate()
    {
        throw new System.NotImplementedException();
    }

    public override void SetItemUI()
    {
        throw new System.NotImplementedException();
    }
}
