/* Name: Player.cs
 * Author: Matt Kirchoff
 * Description: This script handles hover UI text 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
public class HoverText : MonoBehaviour
{
    [Multiline]
    public string myString;
    public TextMeshProUGUI text;
    public float fadeTime;
    public bool displayWhenGhost;
    [HideInInspector]
    public bool display;
    public bool isStatic;
    public Player player;
    private float dist;
    public float displayDist = 5f;
    public TMP_FontAsset font;

    //used for displaying the UI when rat carries key or not
    public bool UIstop;

    // Start is called before the first frame update
    void Start()
    {
        //text = GameObject.Find("Text").GetComponent<TextMeshProUGUI>();
        text = GetComponentInChildren<TextMeshProUGUI>();
        text.SetText(myString);
        text.font = font;
        //text.color = Color.white;
        player = GameObject.FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if(GetComponentInParent<onlyGhostSees>() && !StateChecker.isGhost)
        {
            //Debug.Log("UI stop on guide");
            UIstop = true;
        }
        else if (GetComponentInParent<onlyGhostSees>()&& StateChecker.isGhost)
        {
            UIstop = false;
        }

        //display = false;
        
        dist = Vector3.Distance(this.transform.position, player.transform.position);

        if (dist < displayDist)
        {
            display = true;

        }
        else
        {
            display = false;
        }
        Display();
        LookAt();
    }

    void Display()
    {
        if (UIstop)
        {
            text.color = Color.Lerp(text.color, Color.clear, fadeTime * Time.deltaTime);
            return;
        }
        if(displayWhenGhost)
        {
            if (display)
            {
                //text.enabled = true;
                text.color = Color.Lerp(text.color, Color.white, fadeTime * Time.deltaTime);
            }
            else
            {
                text.color = Color.Lerp(text.color, Color.clear, fadeTime * Time.deltaTime);
                //text.enabled = false;
            }
        }
        else if (!displayWhenGhost)
        {
            if(!StateChecker.isGhost)
            {
                if (display)
                {
                    //text.enabled = true;
                    text.color = Color.Lerp(text.color, Color.white, fadeTime * Time.deltaTime);
                }
                else
                {

                    text.color = Color.Lerp(text.color, Color.clear, fadeTime * Time.deltaTime);
                    //text.enabled = false;
                }
            }
            else
            {
                text.color = Color.Lerp(text.color, Color.clear, fadeTime * Time.deltaTime);
                //text.enabled = false;
            }
        }
        else
        {
            text.color = Color.Lerp(text.color, Color.clear, fadeTime * Time.deltaTime);
            ///text.enabled = false;
        }
    }

    void LookAt()
    {
        if(!isStatic)
        {
            Transform look = FindObjectOfType<Camera>().GetComponent<Transform>();
            transform.LookAt(look);
        }
    }
}
