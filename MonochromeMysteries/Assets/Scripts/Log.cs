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
        instance.StartCoroutine(instance.AddEntryCoroutine(text));
    }

    bool addingEntry = false;
    public IEnumerator AddEntryCoroutine(string text)
    {
        while (addingEntry || isShifting)
            yield return null;
        addingEntry = true;
        //If there will be too many entries, delete the oldest one
        if (trueCount + 1 >= MAX_ENTRIES && text != entries.Peek().text)
        {
            LogEntry remove = entries.Dequeue();
            Destroy(remove.@object);
            trueCount--;
        }
        else if(trueCount >= MAX_ENTRIES && text == entries.Peek().text)
        {
            addingEntry = false;
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
        addingEntry = false;
    }

    float transitionTime = 0.25f;
    bool isShifting = false;
    public IEnumerator Shift(Vector3 direction)
    {
        while (isShifting)
            yield return null;
        isShifting = true;
        float shiftDist = logEntryPrefab.GetComponent<Text>().fontSize * 1.5f * (Screen.width / 800) * (Screen.height / 600);
        //if (isShifting)
        //    yield break;

       
        float currentTime = 0;
        //Get All the current positions so they can be used for lerp
        List<Vector3> startPosition = new List<Vector3>();

        foreach (LogEntry x in entries)
        {
            startPosition.Add(x.@object.transform.position);
        }

        //Move Log Entry in direction over time
        while (currentTime < transitionTime)
        {
            currentTime += Time.unscaledDeltaTime;
            for (int i = 0; i < entries.Count; i++)
            {
                entries.ToArray()[i].@object.transform.position = Vector3.Lerp(startPosition[i], startPosition[i] + (direction * shiftDist), 
                                                                               Mathf.SmoothStep(0f, 1f, currentTime / transitionTime));
            }

            //Canvas.ForceUpdateCanvases();
            //yield return new WaitForEndOfFrame();
            //Canvas.ForceUpdateCanvases();

            //if(currPositions.Count > 0)
            //totalToShift -= Vector3.Distance(currPositions[0], Vector3.Lerp(currPositions[0], currPositions[0] + (direction * totalToShift),
            //                                                                   Mathf.SmoothStep(0f, 1f, currentTime / transitionTime)));

            //startPosition.Clear();

            
            yield return null;
        }
        isShifting = false;
    }

    bool decayInProgress = false;
    private IEnumerator LogEntryDecay()
    {
        decayInProgress = true;
        yield return new WaitForSecondsRealtime(MESSAGE_LIFETIME);
        while (isShifting)
            yield return null;
        LogEntry toRemove = entries.Dequeue();
        Text textComp = toRemove.@object.GetComponent<Text>();
        Color targetColor = new Color(0, 0, 0, 0), startColor = textComp.color;

        float currentTime = 0, transitionTime = 0.25f;
        while (currentTime < transitionTime)
        {
            currentTime += Time.unscaledDeltaTime;
            if (toRemove == null || textComp == null)
            {
                yield break;
            }

            textComp.color = Color.Lerp(startColor, targetColor, Mathf.SmoothStep(0f, 1f, currentTime / transitionTime));

            yield return null;
        }

        Destroy(toRemove.@object);
        //entries.Dequeue();
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
