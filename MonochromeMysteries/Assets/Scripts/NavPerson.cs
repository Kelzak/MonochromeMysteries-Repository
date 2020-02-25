using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavPerson : MonoBehaviour
{

    public Transform targetLocation;
    private GameObject player;
    private bool move;
    private NavMeshAgent agent;

    void Start()
    {
        move = true;
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        
    }

    private void Update()
    {
        
        if (player.activeInHierarchy)
        {
            move = true;
            MoveToRoom();
        }
        else
        {
            move = false;
        }
    }

    void MoveToRoom()
    {
        if (move == true)
        {
            agent.destination = targetLocation.position;
        }
    }
}

