using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Possessable : MonoBehaviour
{
    protected bool canMove;
    private bool isHighlighted = false;
    private static Possessable highlightedObject;

    [SerializeField]
    protected static GameObject possessionVignette;
    private static bool hudActive;
    public static bool HudActive
    {
        get
        {
            return hudActive;
        }
        set
        {
            if (value)
                possessionVignette.SetActive(true);
            else
                possessionVignette.SetActive(false);
            hudActive = value;
        }

    }

    protected delegate void PossessionEvent(bool active);
    protected event PossessionEvent OnPossession;


    public abstract void Ability();

    public void TriggerOnPossession(bool possessionActive)
    {
        OnPossession?.Invoke(possessionActive);
    }

    public void TriggerHighlight()
    {
        if (!isHighlighted && this != highlightedObject)
        {
            if(highlightedObject != null)
            highlightedObject.TriggerHighlight();

            highlightedObject = this;
            StartCoroutine(Highlight());
        }
        else if(this == highlightedObject)
        {
            highlightedObject = null;
        }
    }

    protected virtual void Awake()
    {
    }

    protected virtual void Start()
    {
        if (possessionVignette == null)
            possessionVignette = GameObject.Find("HUD").transform.Find("PossessionVignette").gameObject;

        HudActive = false;

        OnPossession += ToggleVignette;
    }

    public void ToggleVignette(bool possessionActive)
    {
        HudActive = possessionActive;
    }

    public static Possessable GetHighlightedObject()
    {
        return highlightedObject;
    }

    public bool IsHighlighted()
    {
        return isHighlighted;
    }

    public IEnumerator Highlight()
    {
        isHighlighted = true;

        //HIGHLIGHT VARIABLE
        float transitionTime = 0.75f;
        float currentTime = 0;

        Material mat = GetComponent<MeshRenderer>().material;
        mat.EnableKeyword("_EMISSION");
        Color baseColor = mat.color;
        Color highlightColor = new Color(0 / 255f, 0 / 255f, 255 / 255f);

        mat.SetColor("_EmissionColor", baseColor);
        while (this == highlightedObject)
        {
            //Shift to highlighted color
            while(mat.GetColor("_EmissionColor") != highlightColor && this == highlightedObject)
            {
                mat.SetColor("_EmissionColor", Color.Lerp(baseColor, highlightColor, Mathf.Clamp01(currentTime / transitionTime) ));
                currentTime += Time.deltaTime;
                yield return null;
            }

            currentTime = 0;

            while(mat.GetColor("_EmissionColor") != baseColor && this == highlightedObject)
            {
                mat.SetColor("_EmissionColor", Color.Lerp(highlightColor, baseColor, Mathf.Clamp01(currentTime / transitionTime) ));
                currentTime += Time.deltaTime;
                yield return null;
            }

            currentTime = 0;
            yield return null;
        }
        mat.SetColor("_EmissionColor", baseColor);
        mat.DisableKeyword("_EMISSION");

        isHighlighted = false;
    }

    protected virtual void Update()
    {
        if (Input.GetMouseButtonDown(0) && GetComponent<Player>() != null)
            Ability();
    }

    private void OnDisable()
    {
        OnPossession -= ToggleVignette;

    }
}
