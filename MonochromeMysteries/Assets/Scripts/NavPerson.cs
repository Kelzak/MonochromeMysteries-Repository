using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavPerson : MonoBehaviour
{

    public Transform[] targetLocations;
    private GameObject player;
    private bool move;
    private NavMeshAgent agent;
    private Transform targetLocation;
    public float waitTime;
    private float dist;
    private bool setting;

    void Start()
    {
        SetLocation();
        move = true;
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        
    }

    private void Update()
    {
        
        if (player.activeInHierarchy)
        {
            move = true;
            Move();
        }
        else
        {
            move = false;
        }

        dist = Vector3.Distance(this.transform.position, targetLocation.position);
        //Debug.Log("Dist: " + dist);
        if (dist < 2)  
        {
            if (setting == false)
            {
                SetLocation();

                setting = true;
            }
                
            
        }
    }

    void Move()
    {
        if (move == true)
        {
            agent.destination = targetLocation.position;
        }
    }
    void SetLocation()
    {
        int temp = Random.Range(0, targetLocations.Length);

        targetLocation = targetLocations[temp];
        Move();
        Invoke("Setting", waitTime);
        //Debug.Log(setting);
    }
    void Setting()
    {
        setting = false;
    }
}

