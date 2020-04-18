using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Dialogue : MonoBehaviour
{
    public static Dialogue instance;

    const int MAX_CHARACTERS = 87;

    //public static bool leftClickPriority = false;

    public static bool holding = false;
    public GameObject panel;
    private Image speakerImage;
    private Text speakerName;
    private TMP_Text dialogueText;
    private GameObject continuePrompt;

    public Queue<DialogueLine> dialogueQueue;
    public class DialogueLine
    {
        public string speakerName;
        public string message;
        public Sprite speakerPicture;
        public bool holdLine = false;

        public DialogueLine(Character character, string message)
        {
            speakerName = instance.characters[(int) character].name;
            speakerPicture = instance.characters[(int) character].picture;
            this.message = message;
        }

        public void TriggerHold(bool hold)
        {
            holdLine = hold;
        }
    }

    public enum Character { Pete, Photographer };
    public CharacterDetails[] characters;
    [System.Serializable]
    public struct CharacterDetails
    {
        public string name;
        public Sprite picture;
    }

    public static void AddLine(Character character, bool hold, params string[] message)
    {
        //Debug.Log((instance == null) + " " + (instance.dialogueQueue.Count > 0));
        foreach (string line in message)
        {
            DialogueLine lineToAdd = new DialogueLine(character, line);
            lineToAdd.TriggerHold(hold);
            instance.dialogueQueue.Enqueue(lineToAdd);
        }
    }

    public static void AddLine(Character character, params string[] message)
    {
        AddLine(character, false, message);
    }

    public static void ContinueDialogue()
    {
        if(instance.dialogueQueue.Count > 0)
        instance.dialogueQueue.Peek().TriggerHold(false);
    }

    bool dialogueRunning = false;
    public bool textPrinting = false;
    private IEnumerator RunDialogue()
    {
        dialogueRunning = true;
        char[] currentMessage;

        //Initialize for transition
        dialogueText.text = "";
        this.speakerImage.sprite = dialogueQueue.Peek().speakerPicture;
        this.speakerName.text = dialogueQueue.Peek().speakerName;
        StartCoroutine(SetDialogueWindowActive(true));
        //panel.SetActive(true);

        while (transitionInProgress)
            yield return null;

        //While there are still dialogue lines, continue to run
        while (dialogueQueue.Count > 0)
        {
            currentMessage = dialogueQueue.Peek().message.ToCharArray();
            dialogueText.text = "";
            this.speakerImage.sprite = dialogueQueue.Peek().speakerPicture;
            this.speakerName.text = dialogueQueue.Peek().speakerName;

            //leftClickPriority = true;
            textPrinting = true;
            //Print out the text one character at a time until skip key is pressed
            for (int i = 0; i < currentMessage.Length && !Input.GetKeyDown(KeyCode.Space); i++)
            {
                //If dialogue panel is hidden because player is in main menu, wait for it to become unhidden to continue printing
                while (hidden)
                    yield return null;

                dialogueText.text += currentMessage[i];
                if (i % 2 == 0)
                    yield return null;
            }
            //In case skip key was pressed, print out the rest of the message instantly
            dialogueText.text = dialogueQueue.Peek().message;
            //Clear Mouse Button Down Buffer
            if (Input.GetKeyDown(KeyCode.Space))
                yield return null;
            textPrinting = false;

            //Press Button To continue
            int frameCount = 0, triggerFrame = 30;
            while (!Input.GetKeyDown(KeyCode.Space))
            {
                if (frameCount % triggerFrame == 0 && !continuePrompt.activeSelf)
                    continuePrompt.SetActive(true);
                else if (frameCount % triggerFrame == triggerFrame / 2 && continuePrompt.activeSelf)
                    continuePrompt.SetActive(false);
                else if (frameCount == triggerFrame)
                    frameCount = 0;

                frameCount++;
                yield return null;
            }
            continuePrompt.SetActive(false);


            //Wait if line is designated to hold until condition is met
            //Fade away panel after text is read
            if (dialogueQueue.Peek().holdLine == true)
            {
                StartCoroutine(SetDialogueWindowActive(false));
                while (transitionInProgress)
                    yield return null;
                Tutorial.UpdateObjective();
                Tutorial.ShowObjective(true);
            }

            //Wait for hold to be taken off
            while (dialogueQueue.Peek().holdLine == true)
            {
                holding = true;
                yield return null;
            }
            holding = false;

            //Fade Panel back in
            if (panel.activeSelf == false)
            {
                dialogueText.text = "";
                Tutorial.ShowObjective(false);
                StartCoroutine(SetDialogueWindowActive(true));
                while (transitionInProgress)
                    yield return null;
            }


            //if (Input.GetMouseButtonDown(0))
            //    yield return null;
            //leftClickPriority = false;

            

            dialogueQueue.Dequeue();
            yield return null;
        }


        StartCoroutine(SetDialogueWindowActive(false));


        while (transitionInProgress)
            yield return null;
        //panel.SetActive(false);
        dialogueRunning = false;
    }

    bool transitionInProgress = false;
    private IEnumerator SetDialogueWindowActive(bool active)
    {
        transitionInProgress = true;
        float endAlpha;
        //If set Dialogue window to active
        if(active)
        {
            panel.GetComponent<Image>().CrossFadeAlpha(0, 0f, true);
            dialogueText.CrossFadeAlpha(0, 0f, true);
            foreach (Image x in panel.GetComponentsInChildren<Image>())
            {
                x.CrossFadeAlpha(0, 0f, true);
            }


            panel.SetActive(true);
            endAlpha = 1;
        }
        //If set Dialogue window to inactive
        else
        {
            endAlpha = 0;
        }

        panel.GetComponent<Image>().CrossFadeAlpha(endAlpha, 0.5f, true);
        foreach (Image x in panel.GetComponentsInChildren<Image>())
        {
            x.CrossFadeAlpha(endAlpha, 0.5f, true);
        }
        foreach (TMP_Text x in panel.GetComponentsInChildren<TMP_Text>())
        {
            x.CrossFadeAlpha(endAlpha, 0.5f, true);
        }
        yield return new WaitForSecondsRealtime(0.5f);


        if (!active)
            panel.SetActive(false);

        transitionInProgress = false;
    }


    bool hidden = false;
    /// <summary>
    /// Specifically for hiding dialogue when MainMenu is opened
    /// </summary>
    /// <param name="shouldHide"></param>
    private void HideDialogue(bool shouldHide)
    {
        hidden = shouldHide;
        if(shouldHide && dialogueRunning)
        {
            panel.SetActive(false);
        }
        else if(dialogueRunning)
        {
            panel.SetActive(true);
        }
    }

    void Awake()
    {
        instance = this;
        MainMenu.onMainMenuTriggered += HideDialogue;
    }

    // Start is called before the first frame update
    void Start()
    {
        panel = GameObject.Find("HUD").transform.Find("Dialogue").gameObject;
        speakerImage = panel.transform.Find("PhotoSlot").Find("Image").GetComponent<Image>();
        speakerName = panel.transform.Find("PhotoSlot").Find("Label").GetComponent<Text>();
        dialogueText = panel.transform.Find("Text").GetComponent<TMP_Text>();
        continuePrompt = panel.transform.Find("Prompt").gameObject;

        continuePrompt.SetActive(false);
        panel.SetActive(false);
        dialogueQueue = new Queue<DialogueLine>();
    }

    // Update is called once per frame
    void Update()
    {
        if (dialogueQueue != null && dialogueQueue.Count > 0 && !dialogueRunning)
        {
            StartCoroutine(RunDialogue());
        }
    }

    private void OnDisable()
    {
        MainMenu.onMainMenuTriggered -= HideDialogue;
    }
}
