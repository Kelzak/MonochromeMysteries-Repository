using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tutorial : MonoBehaviour
{
    public static Tutorial instance;

    public delegate void TutorialEvent();

    public GameObject[] invisibleWalls;

    private Queue<string> objectives;
    private TMP_Text objectiveText;

    public event TutorialEvent onFirstMovement;
    public event TutorialEvent onFirstRatPossession;
    public event TutorialEvent onPhotographerEnter;
    public event TutorialEvent onFirstPhoto;
    public event TutorialEvent onFirstCloseScrapbook;
    public event TutorialEvent onTutorialEnd;

    bool photographerEntered = false;

    private IEnumerator DialogueScript()
    {
        yield return new WaitForSeconds(5f);
        Dialogue.AddLine(Dialogue.Character.Pete, "Hey you, you’re finally awake. It’s about time. I know you’re new to the whole ‘trapped soul’ gig but you’ve been dead for hours." ,
                         "Just look at your body (by moving your mouse) partner, it's been beaten to a bloody pulp!");
        Dialogue.AddLine(Dialogue.Character.Pete, true, "Take a second to get used to your soul legs (by using the W, A, S, and D keys), I know moving around was a mighty big task when I first became a soul.");
        objectives.Enqueue("Move around (Press W, A, S, or D)");
        Dialogue.AddLine(Dialogue.Character.Pete, "I’m sure you’re itching to get on with your afterlife. But if you don’t want to be stuck in this here motel like good ole Pete, you’ll need to solve your murder.", "You can’t be findin’ peace with unresolved business, now can ya?");
        Dialogue.AddLine(Dialogue.Character.Pete, "Now I know I know, all this sounds harder than a day ole biscuit! But worry not there’s a couple perks you freshly decreased youngins’ get!", "First, you can see the aura of important objects around you. Objects that can provide important clues about your murder, like your body, as well as items you may need later have a white aura.");
        Dialogue.AddLine(Dialogue.Character.Pete, "Another perk of being newly dead is being able to possess living creatures, which will be a mighty powerful tool in solving your murder.");
        Dialogue.AddLine(Dialogue.Character.Pete, true, "You see that rat over there? You can possess him by pressing [E] and depossess him by pressing [Q] to get into spaces tighter than my grandpappy’s trousers after Thanksgivin’ dinner.");
        objectives.Enqueue("Possess a rat (Press E to Possess)");
        Dialogue.AddLine(Dialogue.Character.Pete, "Creatures you possess can also pick up certain items with the F key, indicated by the words floatin' above 'em. Don’t be afraid to use your new abilities to get to where the evidence be hidin’.");
        Dialogue.AddLine(Dialogue.Character.Pete, true, "If you’re gonna start solvin’ your murder, you’ll need to have a way to document evidence. Find a way next door to the Photographer’s room and bring him back here, would ya?");
        OnPhotographerEnter();
        objectives.Enqueue("Find the Photographer and bring him to the crime scene");
        Dialogue.AddLine(Dialogue.Character.Pete, true, "Good job on getting that there photographer in here. Now remember what I was sayin’ about those objects with a white aura? Go and take some photographs of those right quick (by clicking the left mouse button).");
        objectives.Enqueue("Take a picture your dead body (Left-Click as the Photographer)");
        Dialogue.AddLine(Dialogue.Character.Pete, "Great, you’ve collected some evidence! You'll want to keep the photographer close since you'll find more evidence the more you look around. Take a look at those photographs in your scrapbook (by pressing tab). You’ll be able to examine them for more clues there.");
        Dialogue.AddLine(Dialogue.Character.Pete, true, "Now, take a look at those stunning photographs in your scrapbook by pressing [TAB]. You’ll be able to examine them for more clues there.");
        objectives.Enqueue("Open your scrapbook (Press Tab)");
        Dialogue.AddLine(Dialogue.Character.Pete, "Well good luck out there youngin! I’ll be rootin’ for ya, but you’re on your own from here on out. I’m not as young a soul as I used to be, all this here excitement has gone and tuckered me out");
        TriggerTutorialEnd();
    }

    Collider[] hit;
    private IEnumerator WaitForPhotographerEnter()
    {
        while (!photographerEntered)
        {
            hit = Physics.OverlapBox(new Vector3(77.57f, 13.64f, -72.79f), new Vector3(3.65f / 2, 5.92f / 2, 2.41f / 2), Quaternion.identity);
            foreach (Collider x in hit)
            {
                if (x.GetComponent<Photographer>())
                {
                    photographerEntered = true;
                }
                yield return null;
            }
        }
    }

    private void Awake()
    {
        instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        onFirstMovement += TriggerFirstMovement;
        onFirstRatPossession += TriggerFirstRatPossession;
        onPhotographerEnter += TriggerPhotographerEnter;
        onFirstPhoto += TriggerFirstPhoto;
        onFirstCloseScrapbook += TriggerFirstCloseScrapbook;

        objectives = new Queue<string>();
        objectiveText = GameController.mainHUD.transform.Find("Objective").GetComponent<TMP_Text>();

        StartCoroutine(DialogueScript());
        StartCoroutine(WaitForPhotographerEnter());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void UpdateObjective()
    {
        if(instance.objectives.Count > 0)
        {
            instance.objectiveText.text = instance.objectives.Dequeue();
        }
    }

    public static void ShowObjective(bool shouldShowObjective)
    {
        instance.objectiveText.gameObject.SetActive(shouldShowObjective);
    }

    public void OnFirstMovement()
    {
        onFirstMovement?.Invoke();
    }

    public void OnFirstRatPossession()
    {
        onFirstRatPossession?.Invoke();
    }

    public void OnPhotographerEnter()
    {
        onPhotographerEnter?.Invoke();
    }

    public void OnFirstPhoto()
    {
        onFirstPhoto?.Invoke();
    }

    public void OnFirstCloseScrapbook()
    {
        onFirstCloseScrapbook?.Invoke();
    }

    //FIRST MOVEMENT
    private void TriggerFirstMovement()
    {
        StartCoroutine(FirstMovement()); 
    }

    private IEnumerator FirstMovement()
    {
        while(!Dialogue.holding)
        {
            yield return null;
        }

        Dialogue.ContinueDialogue();
        onFirstMovement -= TriggerFirstMovement;

    }

    //RAT POSSESSION
    private void TriggerFirstRatPossession()
    {
        StartCoroutine(FirstRatPossession());
    }

    private IEnumerator FirstRatPossession()
    {
        while (!Dialogue.holding || onFirstMovement != null)
        {
            yield return null;
        }

        Dialogue.ContinueDialogue();
        onFirstRatPossession -= TriggerFirstRatPossession;
    }

    //PHOTOGRAPHER ENTER
    private void TriggerPhotographerEnter()
    {
        StartCoroutine(PhotographerEnter());
    }

    private IEnumerator PhotographerEnter()
    {
        while (!photographerEntered || !Dialogue.holding || onFirstMovement != null || onFirstRatPossession != null)
        {
            yield return null;
        }

        Dialogue.ContinueDialogue();
        onPhotographerEnter -= TriggerPhotographerEnter;
    }

    //FIRST PHOTO
    private void TriggerFirstPhoto()
    {
        if (onPhotographerEnter == null)
        {
            StartCoroutine(FirstPhoto());
        }
    }

    private IEnumerator FirstPhoto()
    {
        while (!Dialogue.holding || onFirstMovement != null || onFirstRatPossession != null || onPhotographerEnter != null)
        {
            yield return null;
        }
        //Debug.Log("Photo Conditions Met");

        Dialogue.ContinueDialogue();
        onFirstPhoto -= TriggerFirstPhoto;
    }

    //CLOSE SCRAPBOOK
    private void TriggerFirstCloseScrapbook()
    {
        if(onFirstPhoto == null)
            StartCoroutine(FirstCloseScrapbook());
    }

    private IEnumerator FirstCloseScrapbook()
    {
        while (!Dialogue.holding || onFirstMovement != null || onFirstRatPossession != null || onPhotographerEnter != null
                || onFirstPhoto != null)
        {
            yield return null;
        }

        Dialogue.ContinueDialogue();
        onFirstCloseScrapbook -= TriggerFirstCloseScrapbook;
    }

    //ON TUTORIAL END
    private void TriggerTutorialEnd()
    {
        StartCoroutine(TutorialEnd());
    }

    private IEnumerator TutorialEnd()
    {
        //Wait until other parts of the tutorial have been completed
        while(onFirstRatPossession != null || onPhotographerEnter != null
               || onFirstPhoto != null || onFirstCloseScrapbook != null || Dialogue.instance.dialogueQueue.Count > 0)
        {
            yield return null;
        }

        //INSERT CODE TO OCCUR AFTER TUTORIAL END HERE
        foreach(GameObject wall in invisibleWalls)
        {
            Destroy(wall);
        }

        objectives.Enqueue("Solve your murder");
        UpdateObjective();
        ShowObjective(true);
        yield return new WaitForSeconds(7f);
        ShowObjective(false);
        //Debug.Log("TutorialEnd");
    }
}
