using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPortrait : MonoBehaviour
{
    public Sprite photographerImage;
    public Sprite ratImage;
    public Image characterImage;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.GetComponent<Player>())
        {
            if (gameObject.GetComponent<Photographer>())
            {
                characterImage.sprite = photographerImage;
            }
            else if (gameObject.GetComponent<Rat>())
            {
                characterImage.sprite = ratImage;
            }
            else
            {
                characterImage.sprite = null;
            }

        }
    }
}
