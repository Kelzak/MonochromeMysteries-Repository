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
    //used to help determine movement
    private bool move = true;
    private NavMeshAgent agent;
    private Transform targetLocation;

    public float waitTime = 10f;
    //if checked the character will use random wait time inbetween the numbers of min and max
    public bool randWaitTime;
    private float randWaitTimeMin;
    private float randWaitTimeMax;

    //used for distance from player calculations
    private float dist;
    //if checked the person will follow the array consecutively instead of randomly 
    public bool isPath;
    //if checked character will go to random path
    public bool wander;
    public float wanderRadius = 10f;
    //int used for pathing
    private int count = 0;
    //speed person turns to look at player when stopped
    public float turnSpeed = 3f;
    //distance until person stops when player gets close
    public float playerDistStop = 5f;

    public AudioSource audioSource;
    private AudioClip step;
    public bool isFemale;
    public float walkSoundInterval = .5f;
    public AudioClip[] maleSteps;
    public AudioClip[] femaleSteps;
    public AudioClip[] indoorSteps;


    void Start()
    {

        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        agent.isStopped = true;

        //calculates the random times based off wait time
        randWaitTimeMin = waitTime / 2;
        randWaitTimeMax = waitTime * 2;

        audioSource = this.GetComponent<AudioSource>();
        audioSource.volume = .5f;
        InvokeRepeating("WalkAudio", 0f, walkSoundInterval);
    }

    [System.Obsolete]
    private void Update()
    {
        //if stopped, invoke movement over wait time
        //Debug.Log(agent.isStopped);
        if (agent.isStopped && move == true)
        {
            if(randWaitTime == true)
            {
                waitTime = Random.Range(randWaitTimeMin, randWaitTimeMax);
            }
            //Debug.Log("move");
            move = false;
            Invoke("Move", waitTime);
        }

        //if player comes close to character, character stops
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
        //makes character looks towards player
        lookTowards();

    }

    public bool IsInside()
    {
        bool isInside;

        Ray indoorCheck;
        indoorCheck = new Ray(transform.position, transform.up);
        //Debug.DrawLine(indoorCheck.origin, transform.up, Color.green);

        if (Physics.Raycast(indoorCheck, 100f))
        {
            isInside = true;
        }
        else
        {
            isInside = false;
        }
        //Debug.Log("Is inside: " + isInside);
        return isInside;
    }

    void WalkAudio()
    {
        if(!agent.isStopped)
        {
            audioSource.volume = .5f;
            int rand;
            if (IsInside() == true)
            {
                rand = Random.Range(0, indoorSteps.Length);
                step = indoorSteps[rand];
                audioSource.volume = .5f;
                audioSource.PlayOneShot(step);
            }
            else if (isFemale)
            {
                rand = Random.Range(0, femaleSteps.Length);
                step = femaleSteps[rand];
                audioSource.volume = .4f;
                audioSource.PlayOneShot(step);
            }
            else
            {
                rand = Random.Range(0, maleSteps.Length);
                step = maleSteps[rand];
                audioSource.volume = .4f;
                audioSource.PlayOneShot(step);
            }
            
        }

    }
    void Move()
    { 
        if (isPath == true)
        {
            
            if (count >= targetLocations.Length)
            {
                count = 0;
            }
            targetLocation = targetLocations[count];
            agent.destination = targetLocation.position;
            count += 1;
        }
        else if(wander == true)
        {
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1);
            Vector3 finalPosition = hit.position;
            agent.destination = finalPosition;
            
        }
        else
        {
            int temp = Random.Range(0, targetLocations.Length);
            targetLocation = targetLocations[temp];
            agent.destination = targetLocation.position;
        }
        agent.isStopped = false;
        move = true;
    }

    protected void lookTowards()
    {
        //look at player only if stopped moving
        if(agent.isStopped == true)
        {
            Vector3 playerPos = player.transform.position;
            Quaternion _lookRotation = Quaternion.LookRotation((playerPos - transform.position).normalized);

            transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * turnSpeed);
        }
       
    }
}