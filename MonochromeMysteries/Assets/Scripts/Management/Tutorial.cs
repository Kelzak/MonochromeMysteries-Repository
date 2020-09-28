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

    public GameObject[] invisibleWalls;

    private Queue<string> objectives;
    private TMP_Text objectiveText;

    public event TutorialEvent onFirstMovement;
    public event TutorialEvent onFirstRatPossession;
    public event TutorialEvent onFirstTVEnter;
    public event TutorialEvent onPhotographerEnter;
    public event TutorialEvent onFirstPhoto;
    public event TutorialEvent onFirstCloseScrapbook;
    public event TutorialEvent onTutorialEnd;

    bool photographerEntered = false;

    private IEnumerator DialogueScript()
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
        Dialogue.AddLine(Dialogue.Character.Pete, "You can use photos to save information about the important objects you find, to inspect them for further information, or use later.", "Keep that man nearby, the photos will prove a vital source of information. At any time you may examine the scrapbook and review your photos.");
        objectives.Enqueue("Press [Tab] to open your scrapbook and examine photos");
        Dialogue.AddLine(Dialogue.Character.Pete, "I’ve given you all the information I can at this point. From here on out, you’ll be on your own.", "This journey will be fraught with twists that will challenge your intellect, but I trust you will be successful. The fate of your heart depends on it.");
        TriggerTutorialEnd();
    }

    Collider[] hit;
    private IEnumerator WaitForPhotographerEnter()
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
    }

    private void Awake()
    {
        instance = this;

        isCompleted = false;
        objectives = new Queue<string>();
        objectiveText = GameController._instance.mainHUD.transform.Find("Objective").GetComponent<TMP_Text>();
    }

    private void Start()
    {

        //DontDestroyOnLoad(instance.transform.parent.gameObject);
        //Load
        if (SaveSystem.gameData != null)
            Load(SaveSystem.gameData.tutorialData);

        if (!isCompleted && !tutorialTriggered)
        {
            tutorialTriggered = true;
            StartCoroutine(DialogueScript());
        }
        else
        {
            TriggerTutorialEnd();
        }
        StartCoroutine(WaitForPhotographerEnter());


    }


    private void OnEnable()
    {
        onFirstMovement += TriggerFirstMovement;
        onFirstRatPossession += TriggerFirstRatPossession;
        onFirstTVEnter += TriggerTVEnter;
        onPhotographerEnter += TriggerPhotographerEnter;
        onFirstPhoto += TriggerFirstPhoto;
        onFirstCloseScrapbook += TriggerFirstCloseScrapbook;

    }

    private void OnDisable()
    {
        onFirstMovement -= TriggerFirstMovement;
        onFirstRatPossession -= TriggerFirstRatPossession;
        onFirstTVEnter -= TriggerTVEnter;
        onPhotographerEnter -= TriggerPhotographerEnter;
        onFirstPhoto -= TriggerFirstPhoto;
        onFirstCloseScrapbook -= TriggerFirstCloseScrapbook;

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

    private IEnumerator FirstMovement()
    {
        while(!Dialogue.holding)
        {
            yield return null;
        }

        Dialogue.ContinueDialogue();
        onFirstMovement -= TriggerFirstMovement;

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
        while (!photographerEntered || !Dialogue.holding || onFirstMovement != null || onFirstRatPossession != null || onFirstTVEnter != null)
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
        foreach(GameObject wall in invisibleWalls)
        {
            Destroy(wall);
        }

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

    public static void Load(Data.TutorialData tutorialData)
    {
        Debug.Log("Loading Tutorial isCompleted: " + tutorialData.tutorialCompleted + " | From Slot: " + SaveSystem.currentSaveSlot);
        instance.isCompleted = tutorialData.tutorialCompleted;
    }
}
