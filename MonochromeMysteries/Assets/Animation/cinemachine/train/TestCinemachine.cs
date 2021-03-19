using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class TestCinemachine : MonoBehaviour
{
    //public GameObject videoObject;

    public VideoPlayer videoPlayer;
    public VideoClip[] videoClips;
    public float[] betweenClipTime;

    public CinemachineVirtualCamera virtualCamera;
   // public CinemachineSmoothPath path;
    public CinemachineTrackedDolly dolly;

    public PlayableDirector playableDirector;


    public CinemachineStoryboard storyboard;
    private float storyboardValue = 1f;
    public float storyboardFadeTime;

    private bool alpha = false;
    private bool pause = false;
    private bool clip1 = false;
    private bool clip2 = false;
    private bool clip3 = false;

    private int index = -1;
    // Start is called before the first frame update
    void Start()
    {
        playableDirector.Play();
        StartCoroutine("PlayClip");
        videoPlayer.targetCameraAlpha = 1;
        //dolly = virtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
    }

    // Update is called once per frame
    void Update()
    {
        //storyboard.m_Alpha = storyboardValue;

        //if (dolly.m_PathPosition < 2.9)
        //{
        //    Invoke("StartClip", .5f);
        //}
        //if (dolly.m_PathPosition > 4)
        //{
        //    Invoke("StartClip", .5f);
        //}

        

        if(playableDirector.time > 22 && !clip1)
        {
            clip1 = true;
            StartCoroutine("PlayClip");
        }
        if (playableDirector.time > 34 && !clip2)
        {
            clip2 = true;
            StartCoroutine("PlayClip");
        }
        if (playableDirector.time > 47 && !clip3)
        {
            clip3 = true;
            StartCoroutine("PlayClip");
        }


        if (alpha)
        {
            videoPlayer.targetCameraAlpha = Mathf.Lerp(videoPlayer.targetCameraAlpha, 1, storyboardFadeTime * Time.deltaTime);
        }
        else
        {
            videoPlayer.targetCameraAlpha = Mathf.Lerp(videoPlayer.targetCameraAlpha, 0, storyboardFadeTime * Time.deltaTime);
        }

    }
    IEnumerator PlayClip()
    {

        index++;
        if (index >= 3)
        {
            //scene change
            print("change scene");
            SceneManager.LoadScene(1);
        }
        print("current index: " + index);
        //pause virtual camera
        playableDirector.Pause();
        virtualCamera.Priority = 9;
        //if is not the starting clip, then fade
        //if (index > 0)
        //StoryboardFade();

        //turn on video overlay, configure and play
        videoPlayer.clip = videoClips[index];
        videoPlayer.Play();
        //videoObject.SetActive(true);
        alpha = true;

        print("wait for: " + videoClips[index].length + " seconds");
        //after video is done, do stuff
        yield return new WaitForSecondsRealtime((float)(videoClips[index].length));

        alpha = false;
        //stop and close video
        //videoObject.SetActive(false);
        //StoryboardFade();
        virtualCamera.Priority = 11;
        playableDirector.Play();

        //increase index, get ready for next marker
        //Invoke("StartClip", betweenClipTime[index]);
        
    }

    void StartClip()
    {
        StartCoroutine("PlayClip");

    }

    //at 22, at 33

    void TogglePause()
    {
        if(pause)
        {
            playableDirector.Play();
        }
        else
        {
            playableDirector.Pause();

        }
    }

    //void StoryboardFade()
    //{
    //    if (dark)
    //    {
    //        storyboardValue = Mathf.Lerp(storyboardValue, 0, storyboardFadeTime);
    //        dark = false;
    //    }
    //    else
    //    {
    //        storyboardValue = Mathf.Lerp(storyboardValue, 1, storyboardFadeTime);
    //        dark = true;
    //    }

    //}
}

