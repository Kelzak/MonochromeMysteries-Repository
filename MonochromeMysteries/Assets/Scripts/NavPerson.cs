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
    private Player player;
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
    public float lookInterval = 3f;
    private Quaternion _lookRotation;
    //int used for pathing
    private int count = 0;
    //speed person turns to look at player when stopped
    public float turnSpeed = 3f;
    //distance until person stops when player gets close
    public float playerDistStop = 5f;

    public bool personStops = true;
    public bool canSeeGhost;

    public AudioSource stepSources;
    private AudioClip step;
    public bool isFemale;
    public float walkSoundInterval = .5f;
    public AudioClip[] maleSteps;
    public AudioClip[] femaleSteps;
    public AudioClip[] indoorSteps;
    public float stepVolume = .5f;

    private bool isPossessed = false;
    private RaycastHit hit;

    void Start()
    {
        //player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        agent.isStopped = true;

        //calculates the random times based off wait time
        randWaitTimeMin = waitTime / 2;
        randWaitTimeMax = waitTime * 2;

        stepSources = this.GetComponent<AudioSource>();
        InvokeRepeating("WalkAudio", 0f, walkSoundInterval);

        if (!canSeeGhost)
            InvokeRepeating("LookAround", 0f, lookInterval);
    }

    [System.Obsolete]
    private void Update()
    {
        player = GameObject.FindObjectOfType<Player>();
        if (this.gameObject.GetComponent<Player>())
        {
            isPossessed = true;
        }
        else
        {
            isPossessed = false;
        }
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
            if (StateChecker.isGhost && personStops)
                agent.Stop();
            else if (!StateChecker.isGhost)
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

        //fixes rain sounds when possessing
        RainFix();

        if(StateChecker.isGhost && canSeeGhost)
        {
            //Debug.Log("is looking ghost");
            lookTowards();
        }
        else if (!StateChecker.isGhost)
        {
            //Debug.Log("is looking");
            lookTowards();
        }

    }

    public bool IsInside()
    {
        bool isInside;

        Ray indoorCheck;
        indoorCheck = new Ray(transform.position, transform.up);
        //Debug.DrawLine(indoorCheck.origin, transform.up, Color.green);

        if (Physics.Raycast(indoorCheck, out hit))
        {
            if (hit.collider.CompareTag("balcony"))
            {
                isInside = false;
            }
            else
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
        if(!agent.isStopped && !isPossessed)
        {
            stepSources.volume = stepVolume;
            int rand;
            if (IsInside() == true)
            {
                rand = Random.Range(0, indoorSteps.Length);
                step = indoorSteps[rand];
                stepSources.volume = .5f;
                stepSources.PlayOneShot(step);
            }
            else if (isFemale)
            {
                rand = Random.Range(0, femaleSteps.Length);
                step = femaleSteps[rand];
                stepSources.volume = .4f;
                stepSources.PlayOneShot(step);
            }
            else
            {
                rand = Random.Range(0, maleSteps.Length);
                step = maleSteps[rand];
                stepSources.volume = .4f;
                stepSources.PlayOneShot(step);
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
            NavMeshPath path = new NavMeshPath();
            agent.CalculatePath(targetLocation.position, path);
            Debug.Log(agent.CalculatePath(targetLocation.position, path));
            //if nav agent can not make it to the desired destination
            if(path.status == NavMeshPathStatus.PathPartial)
            {
                Debug.Log("Invalid path");
                count += 1;
                Move();
                return;
            }
            else
            {
                agent.destination = targetLocation.position;
                count += 1;
            }
            
            
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
    void LookAround()
    {
        if (agent.isStopped == true && StateChecker.isGhost)
        {
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            _lookRotation = Quaternion.LookRotation((randomDirection - transform.position).normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * turnSpeed);
        }
    }

    private void OnDisable()
    {
        isPossessed = true;
        GetComponent<NavMeshAgent>().isStopped = true;
    }

    private void OnEnable()
    {
        isPossessed = false;
        GetComponent<NavMeshAgent>().isStopped = false;
    }

    void RainFix()
    {
        if(IsInside() && isPossessed)
        {
            RainController.navInside = true;
        }
        else
            RainController.navInside = false;

    }
}