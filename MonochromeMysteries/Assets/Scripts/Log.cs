using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Log : MonoBehaviour
{
    const int MAX_ENTRIES = 4;
    const int MAX_CHARACTERS = 36;
    const float MESSAGE_LIFETIME = 5.0f;
    public GameObject logEntryPrefab;
    public static Log instance;

    public Queue<LogEntry> entries;
    public GameObject panel;
    private Transform entrySpawnPoint;

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
        instance.StartCoroutine(instance.LogEntryDecay());
    }

    public IEnumerator AddEntryCoroutine(string text)
    {
        //If there will be too many entries, delete the oldest one
        if (entries.Count + 1 > MAX_ENTRIES)
        {
            LogEntry remove = entries.Dequeue();
            Destroy(remove.@object);
        }

        //Shift every entry up
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

    }

    float transitionTime = 0.25f;
    bool isShifting = false;
    public IEnumerator Shift(Vector3 direction)
    {
        isShifting = true;
        float currentTime = 0;
        float moveDist = logEntryPrefab.GetComponent<RectTransform>().sizeDelta.y + 5;
        //Get All the start positions so they can be used for lerp
        List<Vector3> startPositions = new List<Vector3>();
        foreach (LogEntry x in entries)
        {
            startPositions.Add(x.@object.transform.position);
        }

        //Move Log Entry in direction over time
        while (currentTime < transitionTime)
        {
            for (int i = 0; i < entries.Count; i++)
            {
                entries.ToArray()[i].@object.transform.position = Vector3.Lerp(startPositions[i], startPositions[i] + direction * moveDist * GetComponentInParent<CanvasScaler>().scaleFactor, 
                                                                               Mathf.SmoothStep(0f, 1f, currentTime / transitionTime));
            }

            currentTime += Time.deltaTime;
            yield return null;
        }
        isShifting = false;
    }

    private IEnumerator LogEntryDecay()
    {
        yield return new WaitForSecondsRealtime(MESSAGE_LIFETIME);
        LogEntry toRemove = entries.Peek();
        Text textComp = toRemove.@object.GetComponent<Text>();
        Color targetColor = new Color(0, 0, 0, 0), startColor = textComp.color;

        float currentTime = 0, transitionTime = 0.25f;
        while(currentTime < transitionTime)
        {
            textComp.color = Color.Lerp(startColor, targetColor, Mathf.SmoothStep(0f, 1f, currentTime / transitionTime));

            currentTime += Time.deltaTime;
            yield return null;
        }

        entries.Dequeue();
        Destroy(toRemove.@object);
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
