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

    public void AddLine(Character character, params string[] message)
    {
        foreach(string line in message)
        {
            dialogueQueue.Enqueue(new DialogueLine(character, line));
        }
    }

    bool dialogueRunning = false;
    private IEnumerator RunDialogue()
    {
        dialogueRunning = true;
        char[] currentMessage;

        while (dialogueQueue.Peek() != null)
        {
            currentMessage = dialogueQueue.Peek().message.ToCharArray();
            dialogueText.text = "";
            this.speakerImage.sprite = dialogueQueue.Peek().speakerPicture;
            this.speakerName.text = dialogueQueue.Peek().speakerName;

            for (int i = 0; i < currentMessage.Length; i++)
            {
                dialogueText.text += currentMessage[i];
                yield return new WaitForSecondsRealtime(0.1f);
            }

            yield return new WaitForSecondsRealtime(3f);

        }

        dialogueRunning = false;
    }

    private void SetDialogueWindowActive(bool active)
    {
        panel.SetActive(active);
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
        speakerName = panel.transform.Find("PhotoSlot").Find("Name").GetComponent<Text>();
        dialogueText = panel.transform.Find("Text").GetComponent<Text>();

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
