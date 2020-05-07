/* Name: VideoCutscene.cs
 * Primary Author: Matthew Kirchoff - Main Video Functinoality
 * Contributors: Zackary Seiple - Specific implementation for functioning with differing cutscenes and transitions between scenes
 * Description: Handles the playing of cutscenes as well as specific implementation for changing scenes and handling UI in win and lose states
 * Last Updated: 5/6/2020 (Zackary Seiple)
 * Changes: Added Header
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(VideoPlayer))]
public class VideoCutscene : MonoBehaviour
{
    public VideoClip video;
    public int sceneIndex;
    float length;
    private AudioSource audioSource;
    private GameObject interactiveCanvas;

    [Range(0.0f, 1.0f)]
    public float clipVolume = .5f;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = clipVolume;
        video = GetComponent<VideoPlayer>().clip;
        length = (float)video.length;
        //Invoke("LoadScene", length + 1f);
        StartCoroutine(LoadScene(sceneIndex));
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if(scene.name.Contains("Win") || scene.name.Contains("Lose"))
        {
            //Get Win or Lose canvas and hide it while video plays
            interactiveCanvas = GameObject.Find("Canvas").gameObject;
            interactiveCanvas.SetActive(false);

            //Make Button return the player to the Main Menu (into the TV as if the player was just starting the game
            interactiveCanvas.transform.Find("Return To Main Menu").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => 
                {
                    GameController.initialLoad = true;
                    GameController.initialTVTransition = true;
                    SceneManager.LoadScene(0, LoadSceneMode.Single);
                });
        }
    }

    IEnumerator LoadScene(int index)
    {
        yield return new WaitForSecondsRealtime(length);

        Debug.Log("load Scene");
        if (SceneManager.GetActiveScene().name.Contains("Opening"))
        {
            SaveSystem.NewGame(SaveSystem.currentSaveSlot);
        }
        else
        {
            if(interactiveCanvas != null)
                interactiveCanvas.SetActive(true);
        }
    }
    

}
