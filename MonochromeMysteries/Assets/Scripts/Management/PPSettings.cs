using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;


public class PPSettings : MonoBehaviour
{
    [SerializeField] private PostProcessVolume activeVolume;

    public void Toggle(bool value)
    {
        ColorGrading colorGrading;
        activeVolume.profile.TryGetSettings(out colorGrading);

        if(value)
        {
            colorGrading.active = true;
        }
        else
        {
            colorGrading.active = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
