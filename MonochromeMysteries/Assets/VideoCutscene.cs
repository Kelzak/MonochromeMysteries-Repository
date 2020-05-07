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

    [Range(0.0f, 1.0f)]
    public float clipVolume = .5f;

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

    IEnumerator LoadScene(int index)
    {
        yield return new WaitForSecondsRealtime(length);

        Debug.Log("load Scene");
        if (SceneManager.GetActiveScene().name.Contains("Opening"))
        {
            SaveSystem.NewGame(SaveSystem.currentSaveSlot);
        }
        else if (SceneManager.GetActiveScene().name.Contains("Win"))
        {

        }
        else if (SceneManager.GetActiveScene().name.Contains("Lose"))
        {

        }
    }
    

}
