using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    public static Dialogue instance;

    const int MAX_CHARACTERS = 87;

    public GameObject panel;
    private Image speakerImage;
    private Text speakerName;
    private Text dialogueText;

    public Queue<DialogueLine> dialogueQueue;
    public class DialogueLine
    {
        public string speakerName;
        public string message;
        public Sprite speakerPicture;

        public DialogueLine(Character character, string message)
        {
            speakerName = instance.characters[(int) character].name;
            speakerPicture = instance.characters[(int) character].picture;
            this.message = message;
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

    public static void AddLine(Character character, params string[] message)
    {
        Debug.Log((instance == null) + " " + (instance.dialogueQueue.Count > 0));
        foreach(string line in message)
        {
            instance.dialogueQueue.Enqueue(new DialogueLine(character, line));
        }
    }

    bool dialogueRunning = false;
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

        while (dialogueQueue.Count > 0)
        {
            currentMessage = dialogueQueue.Peek().message.ToCharArray();
            dialogueText.text = "";
            this.speakerImage.sprite = dialogueQueue.Peek().speakerPicture;
            this.speakerName.text = dialogueQueue.Peek().speakerName;

            for (int i = 0; i < currentMessage.Length; i++)
            {
                dialogueText.text += currentMessage[i];
                yield return new WaitForSecondsRealtime(0.025f);
            }

            yield return new WaitForSecondsRealtime(3f);
            dialogueQueue.Dequeue();
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
        foreach (Text x in panel.GetComponentsInChildren<Text>())
        {
            x.CrossFadeAlpha(endAlpha, 0.5f, true);
        }
        yield return new WaitForSecondsRealtime(0.5f);


        if (!active)
            panel.SetActive(false);

        transitionInProgress = false;
    }

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        panel = GameObject.Find("HUD").transform.Find("Dialogue").gameObject;
        speakerImage = panel.transform.Find("PhotoSlot").Find("Image").GetComponent<Image>();
        speakerName = panel.transform.Find("PhotoSlot").Find("Label").GetComponent<Text>();
        dialogueText = panel.transform.Find("Text").GetComponent<Text>();

        panel.SetActive(false);
        dialogueQueue = new Queue<DialogueLine>();
    }

    // Update is called once per frame
    void Update()
    {
        if(dialogueQueue.Count > 0 && !dialogueRunning)
        {
            StartCoroutine(RunDialogue());
        }
    }
}
