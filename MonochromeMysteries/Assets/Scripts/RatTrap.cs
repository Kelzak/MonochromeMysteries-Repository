using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatTrap : MonoBehaviour
{
    public GameObject OpenPanel = null;

    private bool _isInsideTrigger = false;

    private bool isPlayer = false;

    public AudioClip[] disableTrap;
    private AudioSource audioSource;

    private int rand;
    private AudioClip sound;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayer && _isInsideTrigger)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {

                int rand = Random.Range(0, disableTrap.Length);
                AudioClip sound = disableTrap[rand];
                audioSource.PlayOneShot(sound);

                Destroy(this.gameObject);
            }
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Person" && other.gameObject.name.Equals("Exterminator"))
        {
            if (other.GetComponent<Player>())
            {
                _isInsideTrigger = true;
                //OpenPanel.SetActive(true);
                isPlayer = true;
            }
            else
            {
                _isInsideTrigger = true;
                isPlayer = false;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Person" && other.gameObject.name.Equals("Exterminator"))
        {
            _isInsideTrigger = false;
            //OpenPanel.SetActive(false);
        }
        _isInsideTrigger = false;
    }
}
