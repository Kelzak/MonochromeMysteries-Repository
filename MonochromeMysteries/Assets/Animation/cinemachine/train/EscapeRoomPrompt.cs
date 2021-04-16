using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EscapeRoomPrompt : MonoBehaviour
{

    public Image image;
    public NewCutsceneManager cutsceneManager;

    public TMP_Text text;
    public TMP_Text continueText;

    private bool activated = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!activated)
        {

            //Color newColor = new Color(0f, 0.5f, 1f, Mathf.PingPong(Time.deltaTime, 1));

            //Color color = Color.Lerp(Color.clear, Color.white, (Mathf.PingPong(Time.deltaTime, 1)));
            //continueText.color = newColor;

        }
        

        if (Input.GetKeyDown(KeyCode.Space))
        {
            print("activated escape room prompt");
            activated = true;
            image.color = Color.clear;
            text.color = Color.clear;
            continueText.color = Color.clear;
            cutsceneManager.StartCutscene();
        }
    }
}
