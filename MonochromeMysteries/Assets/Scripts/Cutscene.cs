/* Name: Player.cs
 * Author: Matt Kirchoff
 * Description: This script handles cutscenes in forms of panels or pngs
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class Cutscene : MonoBehaviour
{
    public GameObject transitionPanel;

    public GameObject[] cutscenePanels;

    //time it takes between transitions
    public float transitonTime = 1f;
    //time it takes between panels
    public float panelTime = 5f;

    public AudioClip music;
    private AudioSource audioSource;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        //disable all panels to start
        for (int i = 0; i < cutscenePanels.Length; i++)
        {
            cutscenePanels[i].SetActive(false);
        }
        //initialize cutscene panel
        //transitionPanel.GetComponent<Image>().color = Color.black;
        animator = transitionPanel.GetComponent<Animator>();
        StartCoroutine("CutsceneCoroutine");

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = music;
        audioSource.Play();
    }

    IEnumerator CutsceneCoroutine()
    {
        cutscenePanels[0].SetActive(true);
        yield return new WaitForSeconds(panelTime);
 
        for (int i = 1; i < cutscenePanels.Length+1; i++)
        {
            animator.SetBool("fade", true);
            Debug.Log("Transiton");
            yield return new WaitForSeconds(panelTime);
            Debug.Log("panel");

            animator.SetBool("fade", false);
            yield return new WaitForSeconds(transitonTime);
            
            cutscenePanels[i - 1].SetActive(false);
            if(i < cutscenePanels.Length)
                cutscenePanels[i].SetActive(true);
            //yield return new WaitForSeconds(transitonTime);
            
        }

        //disable all panels to end
        for (int i = 0; i < cutscenePanels.Length; i++)
        {
            cutscenePanels[i].SetActive(false);
        }
        animator.SetBool("fade", true);
        yield return new WaitForSeconds(transitonTime);
        transitionPanel.SetActive(false);
        audioSource.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
