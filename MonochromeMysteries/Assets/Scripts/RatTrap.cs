//Made by matt kirchoff

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatTrap : MonoBehaviour
{ 
    public AudioClip disableTrap;
    private AudioSource audioSource;

    private AudioClip sound;

    public bool isActive
    {
        get { return active; }
        set
        {
            gameObject.SetActive(value);
            active = value;
        }
    }

    private float id;
    private bool active = true;

    private void Awake()
    {
        id = (1000 * transform.position.x) + transform.position.y + (0.001f * transform.position.z);
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Activate()
    {
        audioSource.PlayOneShot(disableTrap);
        Invoke("Disable", .1f);
    }

    public float GetID()
    {
        return id;
    }
    void Disable()
    {
        isActive = false;

    }

    public void Load(Data.RatTrapData data)
    {
        for(int i = 0; i < data.trapIDs.Length; i++)
        {
            if(data.trapIDs[i] == this.id)
            {
                this.isActive = data.active[i];
                return;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerEnter(Collider other)
    {
        
    }

    void OnTriggerExit(Collider other)
    {
        
    }
}
