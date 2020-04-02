using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
public class HoverText : MonoBehaviour
{
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
        text = GameObject.Find("Text").GetComponent<TextMeshProUGUI>();
        text = GetComponentInChildren<TextMeshProUGUI>();
        text.SetText(myString);
        //text.color = Color.white;
    }

    // Update is called once per frame
    void Update()
    {
        //display = false;
        player = GameObject.FindObjectOfType<Player>();
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
