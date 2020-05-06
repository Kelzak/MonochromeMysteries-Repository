using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class randLetter : MonoBehaviour
{
    TMP_Text text;
    string st = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
    // Start is called before the first frame update
    void Start()
    {
        char c = st[Random.Range(0, st.Length)];
        text = GetComponent<TextMeshProUGUI>();
        text.text = c.ToString();
        InvokeRepeating("ChangeLetter", 0f, .5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void ChangeLetter()
    {
        char c = st[Random.Range(0, st.Length)];
        text.text = c.ToString();
    }
}
