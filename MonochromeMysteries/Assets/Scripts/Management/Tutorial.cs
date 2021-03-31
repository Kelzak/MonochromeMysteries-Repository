/* Name: Tutorial.cs
 * Primary Author: Zackary Seiple - Event System and Objective Framework, events for the tutorial, as well as loading functionality
 * Contributors: Kevon Long - event for entering TV
 *               Matthew Kirchoff - Invisible walls
 * Description: Handles the events and objectives used during the tutorial to guide the player through a set path in learning the game
 * Last Updated: 5/6/2020 (Zackary Seiple)
 * Changes: Added Header
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tutorial : MonoBehaviour
{
    public static Tutorial instance;
    public bool isCompleted = false;

    public delegate void TutorialEvent();

    //public GameObject[] invisibleWalls;

    private Queue<string> objectives;
    public TMP_Text objectiveText;

    public event TutorialEvent onFirstMovement;
    public event TutorialEvent onFirstTVEnter;
    public event TutorialEvent onFirstRatPossession;
    public event TutorialEvent onPhotographerEnter;
    public event TutorialEvent onFirstPhoto;
    public event TutorialEvent onFirstCloseScrapbook;
    public event TutorialEvent onTutorialEnd;

    public event TutorialEvent onFirstPossess;
    //public event TutorialEvent onRead;
    //public event TutorialEvent onPickupFuse;


    bool photographerEntered = false;

    public bool canEnterTV;
    public bool canEnterRat;
    public bool canBringMan;

    public bool canPickupFuse;
    public bool canPossess;

    /*private IEnumerator HotelDialogueScript()
    {

        yield return new WaitForSeconds(5f);
        Dialogue.AddLine(Dialogue.Character.Pete, "Ah, so you’ve awoken. I apologize about your body and its...current state. I know you must be lost in this afterlife, so let me guide you. You don’t have many other options.", "However, I assure you that I have your best interest at heart.");
        Dialogue.AddLine(Dialogue.Character.Pete, true, "Your being is unlike the one you came from. Walk around this room to get a feel for your new body.");
        objectives.Enqueue("Move around (Press W, A, S, and D)");
        Dialogue.AddLine(Dialogue.Character.Pete, "I can feel the growing chaos in your heart, for your life has been taken from you. To be at rest you must solve your murder to bring justice. It might be difficult since your memory has been lost.", "As a soul you are limited. You can not interact with the world around you. However, you can see things tied to your murder with a glowing aura. These objects may provide crucial information to solve the mystery.");
        Dialogue.AddLine(Dialogue.Character.Pete, true, "These “picture boxes” will act as a hub for you to save your progress and come back to where you left off if you ever need a break.");
        objectives.Enqueue("Possess a TV by pressing [F] in front of it. Press [F] again to exit.");
        Dialogue.AddLine(Dialogue.Character.Pete, true, "As a soul, you have a direct line into the hearts in every living being, allowing you to possess and control their bodies as if it was your own, including those rats crawling around. As a rat, you can pick up various smaller items like keys and paper balls.");
        objectives.Enqueue("Possess a rat by pressing [E]. To leave the body, press [Q].");
        Dialogue.AddLine(Dialogue.Character.Pete, true, "In order to solve your mystery, you’ll need help from the man in the next room. He possesses an object that will be necessary for analyzing information. Bring him here.");
        OnPhotographerEnter();
        objectives.Enqueue("Get the man next door into your room");
        Dialogue.AddLine(Dialogue.Character.Pete, true, "I see you’re growing accustomed to your newfound powers. Very good. Now, you’ll see the man is well versed in capturing visages. It’d be wise to take photos of objects with auras around them to inspect them further. Try this with your body.");
        objectives.Enqueue("Take a photo of your body as the Photographer with [LeftMouseClick]");
        Dialogue.AddLine(Dialogue.Character.Pete, "You can use photos to save information about the important objects you find, to inspect them for further information, or use later.");
        Dialogue.AddLine(Dialogue.Character.Pete, true, "Keep that man nearby, the photos will prove a vital source of information. At any time you may examine the scrapbook and review your photos.");
        objectives.Enqueue("Press [Tab] to open your scrapbook and examine photos");
        Dialogue.AddLine(Dialogue.Character.Pete, "I’ve given you all the information I can at this point. From here on out, you’ll be on your own.", "This journey will be fraught with twists that will challenge your intellect, but I trust you will be successful. The fate of your heart depends on it.");
        TriggerTutorialEnd();
    }*/

    private IEnumerator TrainDialogueScript()
    {

        yield return new WaitForSeconds(2f);
        Dialogue.AddLine(Dialogue.Character.Pete, "I see, you are finally awake. Behold, I am Osiris, Lord of the Underworld and Judge of the Dead. And you… you are deceased.");
        Dialogue.AddLine(Dialogue.Character.Pete, "I have granted you the chance to be rebirthed which is why you are in your current form, a form that is trapped between the living world, and the after life.");
        Dialogue.AddLine(Dialogue.Character.Pete, true,"In this Astral form, you are able to traverse the environment, but unable to interact with the living world in any way. Move around to get used to this new form.");
        objectives.Enqueue("Move around using [W], [A], [S], and [D]");
        Dialogue.AddLine(Dialogue.Character.Pete, true, "Although, this Astral form does come with its advantages, and that is by possessing the living and using them to fulfil your bidding. Find one now to get aquainted with your newfound power.");
        objectives.Enqueue("Possess a lifeform on the train using [E]");
        Dialogue.AddLine(Dialogue.Character.Pete, "You can use these lifeforms to pick items up, read, and find your way to hard to reach places depending on the life form you choose.","These life forms talents are also carried over, meaning if you possess someone or something with a skill, you can use that to your advantage");
        Dialogue.AddLine(Dialogue.Character.Pete, "With all of this information, you are probably wondering why I brought you back. You were murdered, and your killer has fled.","Once you find the truth behind your murder, hunt down your killer, and bring yourself to justice, only then can your soul be free to join the afterlife.");
        TriggerTutorialEnd();
    }

    Collider[] hit;
    /*private IEnumerator WaitForPhotographerEnter()
    {
        while (!photographerEntered && !isCompleted)
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
    }*/

    private void Awake()
    {
        instance = this;
        canPossess = false;

        isCompleted = false;
        objectives = new Queue<string>();
        //objectiveText = GameController._instance.mainHUD.transform.Find("Objective").GetComponent<TMP_Text>();
    }

    private void Start()
    {
        Debug.Log("canentertv is " + canEnterTV);
        //DontDestroyOnLoad(instance.transform.parent.gameObject);
        //Load
        /*if (SaveSystem.gameData != null)
            Load(SaveSystem.gameData.tutorialData);*/

        if (!isCompleted && !tutorialTriggered)
        {
            tutorialTriggered = true;
            //StartCoroutine(HotelDialogueScript());
            StartCoroutine(TrainDialogueScript());
        }
        else
        {
            TriggerTutorialEnd();
        }
        //StartCoroutine(WaitForPhotographerEnter());


    }


    private void OnEnable()
    {
        onFirstMovement += TriggerFirstMovement;
        onFirstRatPossession += TriggerFirstRatPossession;
        onFirstTVEnter += TriggerTVEnter;
        onPhotographerEnter += TriggerPhotographerEnter;
        onFirstPhoto += TriggerFirstPhoto;
        onFirstCloseScrapbook += TriggerFirstCloseScrapbook;

        onFirstPossess += OnFirstPossess;
        //onRead += TriggerOnRead;
        //onPickupFuse += TriggerPickupFuse;
    }

    private void OnDisable()
    {
        onFirstMovement -= TriggerFirstMovement;
        onFirstRatPossession -= TriggerFirstRatPossession;
        onFirstTVEnter -= TriggerTVEnter;
        onPhotographerEnter -= TriggerPhotographerEnter;
        onFirstPhoto -= TriggerFirstPhoto;
        onFirstCloseScrapbook -= TriggerFirstCloseScrapbook;

        onFirstPossess -= OnFirstPossess;
        //onRead -= TriggerOnRead;
        //onPickupFuse -= TriggerPickupFuse;
    }


    // Start is called before the first frame update
    bool tutorialTriggered = false;


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

    //HOTEL OBJECTIVES
    public void OnFirstMovement()
    {
        onFirstMovement?.Invoke();
    }

    public void OnFirstTVEnter()
    {
        onFirstTVEnter?.Invoke();
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

    //TRAIN OBJECTIVES
    public void TriggerFirstPossess()
    {
        onFirstPossess?.Invoke();
    }

    private void OnFirstPossess()
    {
        StartCoroutine(FirstPossess());
    }

    public void TriggerOnRead()
    {
        //onRead?.Invoke();
    }

    public void TriggerPickupFuse()
    {
        //onPickupFuse?.Invoke();
    }



    private void TriggerOnReads()
    {
        StartCoroutine(OnRead());
    }

    private IEnumerator OnRead()
    {
        while (!Dialogue.holding || onFirstPossess != null)
        {
            yield return null;
        }
        canPickupFuse = true;
        Dialogue.ContinueDialogue();
        //onRead -= TriggerOnRead;
    }

    private void PickedUpFuse()
    {
        StartCoroutine(PickupFuse());
    }

    private IEnumerator PickupFuse()
    {
        while (!Dialogue.holding || onFirstPossess != null/* || onRead != null*/)
        {
            yield return null;
        }
        Dialogue.ContinueDialogue();
        //onPickupFuse -= TriggerPickupFuse;
    }

    private IEnumerator FirstMovement()
    {
        while(!Dialogue.holding)
        {
            yield return null;
        }
        canEnterTV = true;
        canPossess = true;
        Dialogue.ContinueDialogue();
        onFirstMovement -= TriggerFirstMovement;
    }

    private IEnumerator FirstPossess()
    {
        Debug.Log("triggered");
        while (!Dialogue.holding || onFirstMovement != null)
        {
            yield return null;
        }
        Dialogue.ContinueDialogue();
        onFirstPossess -= OnFirstPossess;
    }

    //FIRST TV ENTERED
    private void TriggerTVEnter()
    {
        StartCoroutine(TVEnter());
    }

    private IEnumerator TVEnter()
    {
        while (!Dialogue.holding || onFirstMovement != null)
        {
            yield return null;
        }
        canEnterTV = false;
        canEnterRat = true;
        Dialogue.ContinueDialogue();
        onFirstTVEnter -= TriggerTVEnter;
    }

    //RAT POSSESSION
    private void TriggerFirstRatPossession()
    {
        StartCoroutine(FirstRatPossession());
    }

    private IEnumerator FirstRatPossession()
    {
        while (!Dialogue.holding || onFirstMovement != null || onFirstTVEnter != null)
        {
            yield return null;
        }
        canEnterRat = false;
        canBringMan = true;
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
        while (!photographerEntered || !Dialogue.holding || onFirstMovement != null || onFirstTVEnter != null || onFirstRatPossession != null)
        {
            yield return null;
        }
        canBringMan = false;
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
        while (!Dialogue.holding || onFirstMovement != null || onFirstRatPossession != null || onFirstTVEnter != null || onPhotographerEnter != null)
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
        while (!Dialogue.holding || onFirstMovement != null || onFirstRatPossession != null || onFirstTVEnter != null || onPhotographerEnter != null
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
            if (isCompleted)
                break;
            yield return null;
        }

       

        //INSERT CODE TO OCCUR AFTER TUTORIAL END HERE
        /*foreach(GameObject wall in invisibleWalls)
        {
            Destroy(wall);
        }*/

        isCompleted = true;

       if (SaveSystem.gameData.tutorialData.tutorialCompleted == false)
        {
            objectives.Enqueue("Solve your murder");
            UpdateObjective();
            ShowObjective(true);
            yield return new WaitForSeconds(7f);
            ShowObjective(false);
            //Debug.Log("TutorialEnd");
        }

    }

    /*public static void Load(Data.TutorialData tutorialData)
    {
        Debug.Log("Loading Tutorial isCompleted: " + tutorialData.tutorialCompleted + " | From Slot: " + SaveSystem.currentSaveSlot);
        instance.isCompleted = tutorialData.tutorialCompleted;
    }*/
}
