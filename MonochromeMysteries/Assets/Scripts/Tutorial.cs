using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public static Tutorial instance;

    public delegate void TutorialEvent();

    public event TutorialEvent onFirstMovement;
    public event TutorialEvent onFirstRatPossession;
    public event TutorialEvent onPhotographerEnter;
    public event TutorialEvent onFirstPhoto;
    public event TutorialEvent onFirstCloseScrapbook;

    bool photographerEntered = false;

    private IEnumerator DialogueScript()
    {
        yield return new WaitForSeconds(5f);
        Dialogue.AddLine(Dialogue.Character.Pete, "Hey you, you’re finally awake. It’s about time. I know you’re new to the whole ‘trapped soul’ gig but you’ve been dead for hours." ,
                         "Just look at your body (by moving your mouse) partner, it's been beaten to a bloody pulp!");
        Dialogue.AddLine(Dialogue.Character.Pete, true, "Take a second to get used to your soul legs (by using the W, A, S, and D keys), I know moving around was a mighty big task when I first became a soul.");
        Dialogue.AddLine(Dialogue.Character.Pete, "I’m sure you’re itching to get on with your afterlife. But if you don’t want to be stuck in this here motel like good ole ____, you’ll need to solve your murder.", "You can’t be findin’ peace with unresolved business, now can ya?");
        Dialogue.AddLine(Dialogue.Character.Pete, "Now I know I know, all this sounds harder than a day ole biscuit! But worry not there’s a couple perks you freshly decreased youngins’ get, unlike poor ole ____ who's been here longer than he can remember.", "First, you can see the aura of important objects around you. Objects that can provide important clues about your murder, like your body, have a ____ aura and items you may need later have a ____ aura.");
        Dialogue.AddLine(Dialogue.Character.Pete, "Another perk of being newly dead is being able to possess living creatures, which will be a mighty powerful tool in solving your murder.");
        Dialogue.AddLine(Dialogue.Character.Pete, true, "You see that rat over there? You can possess him (by pressing E and depossess him by pressing Q) to get into spaces tighter than my grandpappy’s trousers after Thanksgivin’ dinner.");
        Dialogue.AddLine(Dialogue.Character.Pete, "Creatures you possess can also pick up items with the F key. Don’t be afraid to use your new abilities to get to where the evidence be hidin’.");
        Dialogue.AddLine(Dialogue.Character.Pete, true, "If you’re gonna start solvin’ your murder, you’ll need to have a way to document evidence.Find a way next door to the Photographer’s room and bring him back here, would ya?");
        OnPhotographerEnter();
        Dialogue.AddLine(Dialogue.Character.Pete, true, "Good job on getting that there photographer in here. Now remember what I was sayin’ about those objects with a ____ aura ? Go and take some photographs of those right quick(by clicking the left mouse button).");
        Dialogue.AddLine(Dialogue.Character.Pete, true, "Great you’ve collected some evidence. Take a look at those photographs in your scrapbook (by pressing tab). You’ll be able to examine them for more clues there.");
        Dialogue.AddLine(Dialogue.Character.Pete, "Well good luck out there youngin! I’ll be rootin’ for ya, but you’re on your own from here on out. I’m not as young a soul as I used to be, all this here excitement has gone and tuckered me out");
    }

    Collider[] hit;
    private IEnumerator WaitForPhotographerEnter()
    {
        while (!photographerEntered)
        {
            hit = Physics.OverlapBox(new Vector3(92.82f, 7.397f, 157), new Vector3(3.15f / 2, 4.59f / 2, 1.35f / 2), Quaternion.identity);
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

        StartCoroutine(DialogueScript());
        StartCoroutine(WaitForPhotographerEnter());
    }

    // Update is called once per frame
    void Update()
    {

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
        StartCoroutine(FirstPhoto());
    }

    private IEnumerator FirstPhoto()
    {
        while (!Dialogue.holding || onFirstMovement != null || onFirstRatPossession != null || onPhotographerEnter != null)
        {
            yield return null;
        }

        Dialogue.ContinueDialogue();
        onFirstPhoto -= TriggerFirstPhoto;
    }

    //CLOSE SCRAPBOOK
    private void TriggerFirstCloseScrapbook()
    {
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

}
