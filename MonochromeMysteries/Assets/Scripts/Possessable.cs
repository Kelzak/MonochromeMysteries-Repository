using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Possessable : MonoBehaviour
{
    protected bool canMove;
    private bool isHighlighted = false;
    private static Possessable highlightedObject;

    protected delegate void PossessionEvent();
    protected event PossessionEvent OnPossession;

    public abstract void Ability();

    public void TriggerOnPossession()
    {
        OnPossession?.Invoke();
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
}
