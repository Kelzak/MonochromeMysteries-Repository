/*
 * Created by Matt Kirchoff
 * character movement AI
 */
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
    public bool isPath;
    private int count;
    public float turnSpeed;
    //distance until 
    public float playerDistStop;

    void Start()
    {
        turnSpeed = 3f;
        move = true;
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        count = 0;
        agent.isStopped = true;
        playerDistStop = 4f;
    }

    [System.Obsolete]
    private void Update()
    {
        Debug.Log(agent.isStopped);
        if (agent.isStopped && move == true)
        {
            Debug.Log("move");
            move = false;
            Invoke("Move", waitTime);
        }

        dist = Vector3.Distance(this.transform.position, player.transform.position);
        //Debug.Log("Dist: " + dist);
        if (dist < playerDistStop)
        {
            agent.Stop();
            
        }
        else
        {
            agent.Resume();
        }
        if(agent.remainingDistance == 0f)
        {
            agent.isStopped = true;
        }
        lookTowards();

    }

    void Move()
    {

        if (isPath == true)
        {
            count += 1;
            if (count > targetLocations.Length)
            {
                count = 0;
            }
            targetLocation = targetLocations[count];
        }
        else
        {
            int temp = Random.Range(0, targetLocations.Length);
            targetLocation = targetLocations[temp];
        }
        agent.destination = targetLocation.position;
        agent.isStopped = false;
        move = true;
    }

    protected void lookTowards()
    {
        if(agent.isStopped == true)
        {
            Vector3 playerPos = player.transform.position;
            Quaternion _lookRotation = Quaternion.LookRotation((playerPos - transform.position).normalized);

            transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * turnSpeed);
        }
       
    }
}