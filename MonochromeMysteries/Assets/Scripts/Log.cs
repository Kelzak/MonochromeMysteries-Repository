using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Log : MonoBehaviour
{
    const int MAX_ENTRIES = 4;
    const int MAX_CHARACTERS = 36;
    const float MESSAGE_LIFETIME = 4.0f;
    public GameObject logEntryPrefab;
    public static Log instance;

    public Queue<LogEntry> entries;
    public GameObject panel;
    private Transform entrySpawnPoint;
    private int trueCount;

    public class LogEntry
    {
        public string text, textCropped;
        public GameObject @object;

        public LogEntry(string text, GameObject @object)
        {
            this.text = text;
            this.@object = @object;
        }
    }

    public static void AddEntry(string text)
    {
        if (instance.trueCount < MAX_ENTRIES)
            instance.StartCoroutine(instance.AddEntryCoroutine(text));
    }

    public IEnumerator AddEntryCoroutine(string text)
    {
        //If there will be too many entries, delete the oldest one
        if (trueCount + 1 >= MAX_ENTRIES && text != entries.Peek().text)
        {
            LogEntry remove = entries.Dequeue();
            Destroy(remove.@object);
            trueCount--;
        }
        else if(trueCount >= MAX_ENTRIES && text == entries.Peek().text)
        {
            yield break;
        }

        trueCount++;

        //Shift every entry
        StartCoroutine(Shift(Vector3.down));
        while (isShifting)
            yield return null;
        //Create new Entry
        GameObject entryInstance = Instantiate<GameObject>(logEntryPrefab, panel.transform);
        string textFull = text;
        text = text.Substring(0, Mathf.Min(textFull.Length, MAX_CHARACTERS));
        if (textFull.Length >= MAX_CHARACTERS)
            text += "...";
        entryInstance.GetComponent<Text>().text = text;

        entries.Enqueue(new LogEntry(text, entryInstance));

        StartCoroutine(LogEntryDecay());
    }

    float transitionTime = 0.25f, totalShift;
    bool isShifting = false;
    public IEnumerator Shift(Vector3 direction)
    {
        totalShift += logEntryPrefab.GetComponent<Text>().fontSize * 2 * GameController.mainHUD.GetComponent<CanvasScaler>().scaleFactor;
        if (isShifting)
            yield break;

        isShifting = true;
        float currentTime = 0;
        //Get All the current positions so they can be used for lerp
        List<Vector3> currPositions = new List<Vector3>();

        //Move Log Entry in direction over time
        while (currentTime < transitionTime)
        {
            foreach (LogEntry x in entries)
            {
                currPositions.Add(x.@object.transform.position);
            }

            for (int i = 0; i < entries.Count; i++)
            {
                entries.ToArray()[i].@object.transform.position = Vector3.Lerp(currPositions[i], currPositions[i] + direction * totalShift, 
                                                                               Mathf.SmoothStep(0f, 1f, currentTime / transitionTime));
            }

            Canvas.ForceUpdateCanvases();
            yield return new WaitForEndOfFrame();
            Canvas.ForceUpdateCanvases();

            if(currPositions.Count > 0)
            totalShift -= Vector3.Distance(currPositions[0], entries.ToArray()[0].@object.transform.position);

            currPositions.Clear();

            currentTime += Time.deltaTime;
            yield return null;
        }
        isShifting = false;
    }

    bool decayInProgress = false;
    private IEnumerator LogEntryDecay()
    {
        decayInProgress = true;
        yield return new WaitForSecondsRealtime(MESSAGE_LIFETIME);
        LogEntry toRemove = entries.Peek();
        Text textComp = toRemove.@object.GetComponent<Text>();
        Color targetColor = new Color(0, 0, 0, 0), startColor = textComp.color;

        float currentTime = 0, transitionTime = 0.25f;
        while (currentTime < transitionTime)
        {
            if (toRemove == null || textComp == null)
            {
                yield break;
            }

            textComp.color = Color.Lerp(startColor, targetColor, Mathf.SmoothStep(0f, 1f, currentTime / transitionTime));

            currentTime += Time.deltaTime;
            yield return null;
        }

        while (isShifting)
            yield return null;
        entries.Dequeue();
        Destroy(toRemove.@object);
        trueCount--;
            
        
        decayInProgress = false;
    }
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        panel = GameObject.Find("HUD").transform.Find("Log").gameObject;
        entries = new Queue<LogEntry>();
        entrySpawnPoint = panel.transform.Find("LogStart");
    }
}
