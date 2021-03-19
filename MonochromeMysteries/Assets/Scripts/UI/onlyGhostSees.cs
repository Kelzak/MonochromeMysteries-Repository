/* Name: Player.cs
 * Author: Matt Kirchoff
 * Description: This script turns off mesh when player is not ghost (mostly used for spirit guide)
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class onlyGhostSees : MonoBehaviour
{

    public bool onlyGhostSee;

    private new MeshRenderer renderer;

    private Material mat;

    private HoverText hoverText;
    // Start is called before the first frame update
    void Start()
    {
        mat = gameObject.GetComponent<MeshRenderer>().material;
        renderer = GetComponent<MeshRenderer>();
        if(GetComponentInChildren<HoverText>())
        {
            hoverText = GetComponentInChildren<HoverText>();
        }
        else
        {
            hoverText = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (hoverText != null)
        {
            if (!StateChecker.isGhost)
            {
                if (onlyGhostSee)
                    hoverText.UIstop = true;
            }
            else
            {
                hoverText.UIstop = false;
            }
        }
        if (!StateChecker.isGhost)
        {
            if(onlyGhostSee)
                renderer.enabled = false;
        }
        else
        {
            renderer.enabled = true;
        }
    }
}