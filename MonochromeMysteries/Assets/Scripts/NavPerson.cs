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
    [Header("Nav locations")]
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
    [Header("AI")]
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
    [Header("Sound")]
    public AudioSource stepSources;
    public AudioSource mumbleSource;
    
    private AudioClip step;
    public bool isFemale;
    public float walkSoundInterval = .5f;
    public AudioClip[] maleSteps;
    public AudioClip[] femaleSteps;
    public AudioClip[] indoorSteps;
    public AudioClip[] grassSteps;
    public AudioClip possessClip;
    public AudioClip depossessClip;
    public float stepVolume = .5f;

    private bool isPossessed = false;
    private RaycastHit hit;

    private bool waitAfterPossess;
    private bool isWaiting;
    public float waitAfterPossessTime = 20f;

    void Start()
    {
        //player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        agent.isStopped = true;

        //calculates the random times based off wait time
        randWaitTimeMin = waitTime / 2;
        randWaitTimeMax = waitTime * 2;

        stepSources = this.GetComponent<AudioSource>();
        mumbleSource = this.GetComponent<AudioSource>();
        InvokeRepeating("WalkAudio", 0f, walkSoundInterval);

        if (!canSeeGhost)
            InvokeRepeating("LookAround", 0f, lookInterval);

        mumbleSource.volume = .5f;

        
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
        if (agent.isStopped && move == true && !waitAfterPossess)
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
        if (dist < playerDistStop || waitAfterPossess)
        {
            if (StateChecker.isGhost && personStops || waitAfterPossess)
                agent.Stop();
            else if (!StateChecker.isGhost || waitAfterPossess)
                agent.Stop();
        }
        else
        {
            if(!waitAfterPossess)
                agent.Resume();
        }
        if(agent.remainingDistance == 0f)
        {
            agent.isStopped = true;
        }

        //fixes rain sounds when possessing
        //RainFix();

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
        Vector3 fwd = new Vector3(0, 2, 0);
        Ray indoorCheck = new Ray(transform.position + fwd, transform.up);
        //Debug.DrawLine(indoorCheck.origin, hit.transform.position);

        
        if (Physics.Raycast(indoorCheck, out hit))
        {
            //Debug.Log(hit.collider.gameObject.tag);
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
        //Debug.Log(hit.collider.gameObject.name);
        return isInside;
    }
    public bool OnGrass()
    {
        bool onGrass;
        Vector3 fwd = new Vector3(0, 5, 0);
        if (StateChecker.isGhost)
        {
            fwd = new Vector3(0, 2, 0);
        }

        Ray indoorCheck = new Ray(transform.position + fwd, Vector3.down);
        //Debug.DrawLine(indoorCheck.origin, hit.transform.position);

        RaycastHit[] hit;
        if ((hit = Physics.RaycastAll(indoorCheck, Player.reticleDist)).Length > 0)
        {
            GameObject target = null;
            float shortestDistance = Mathf.Infinity;
            for (int i = 0; i < hit.Length; i++)
            {
                if (hit[i].distance < shortestDistance && hit[i].collider.gameObject != gameObject)
                {
                    target = hit[i].collider.gameObject;
                    shortestDistance = hit[i].distance;
                }
            }


            //Debug.Log(hit.collider.gameObject.name);
            //Debug.Log(hit.collider.gameObject.tag);
            if (target.CompareTag("Person"))
            {
                //ignore collider
            }
            if (target.name.Equals("Bass"))
            {
                Debug.Log("nav person on Graass");
                onGrass = true;
            }
            else
                onGrass = false;
        }
        else
        {
            onGrass = false;
        }
        //Debug.Log("Is inside: " + isInside);
        //Debug.Log(hit.collider.gameObject.name);
        return onGrass;
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
            else if (OnGrass() == true)
            {
                rand = Random.Range(0, grassSteps.Length-1);
                step = grassSteps[rand];
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
            //Debug.Log(agent.CalculatePath(targetLocation.position, path));
            //Debug.Log("chek path");
            //if nav agent can not make it to the desired destination
            if (path.status != NavMeshPathStatus.PathComplete)
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

            NavMeshPath path = new NavMeshPath();
            agent.CalculatePath(targetLocation.position, path);
            //Debug.Log(agent.CalculatePath(targetLocation.position, path));
            //Debug.Log("chek path");

            //if nav agent can not make it to the desired destination
            if (path.status != NavMeshPathStatus.PathComplete)
            {
                //Debug.Log("Invalid path");
                Move();
                return;
            }
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

            if (gameObject.name == "guide")
                _lookRotation.eulerAngles = new Vector3(_lookRotation.eulerAngles.x, _lookRotation.eulerAngles.y + 180, _lookRotation.eulerAngles.z);


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
        mumbleSource.PlayOneShot(possessClip);
        isPossessed = true;
        GetComponent<NavMeshAgent>().isStopped = true;
        try
        {
            GetComponentInChildren<HoverText>().UIstop = true;
        }
        catch
        {
            //throw
        }
    }

    private void OnEnable()
    {
        mumbleSource.PlayOneShot(depossessClip);

        waitAfterPossess = true;
        isPossessed = false;
        GetComponent<NavMeshAgent>().isStopped = false;
        //Debug.Log("invoke");
        if(!isWaiting)
        {
            isWaiting = true;
            Invoke("PossessWait", waitAfterPossessTime);       
        }
        try
        {
            GetComponentInChildren<HoverText>().UIstop = false;
        }
        catch
        {
            //throw
        }
    }

    //void RainFix()
    //{
    //    if(IsInside() && isPossessed)
    //    {
    //        RainController.navInside = false;
    //    }
    //    else
    //        RainController.navInside = true;

    //}
    void PossessWait()
    {
        isWaiting = false;
        waitAfterPossess = false;
        //Debug.Log("pass");
    }
}