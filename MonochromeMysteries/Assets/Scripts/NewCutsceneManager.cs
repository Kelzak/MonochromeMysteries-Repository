using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.Timeline;
using Cinemachine;
using UnityEngine.SceneManagement;

public class NewCutsceneManager : MonoBehaviour
{
    public bool camAnimationFirst;

    //cutscene clips 
    public VideoClip[] videoClips;
    private VideoClip clip;
    public VideoPlayer videoPlayer;

    //camera animations
    public Animation[] animations;
    private Animation anim;
    public Animator animator;

    public PlayableDirector playableDirector;
    public Playable[] playables;
    public PlayableAsset[] playableAssets;
    public TimelineAsset[] playableAssetz;

    public PlayableDirector[] playableDirectors;

    public Image fadeImage;
    public float fadeTime;
    public bool playOnStart;

    private bool dark = false;

    //variables
    private int index = 0;
    private IEnumerator coroutine;


    // Start is called before the first frame update
    void Start()
    {
        //initialize fade settings
        fadeImage.color = Color.black;
        dark = false;


        if (playOnStart)
        {
            StartCutscene();
        }

    }

    void Update()
    {
        if (dark)
        {
            print("fading to dark");
            //fade color to clear
            Color color = Color.Lerp(fadeImage.color, Color.black, fadeTime * Time.deltaTime);
            fadeImage.color = color;


        }
        else
        {
            print("fading to clear");
            Color color = Color.Lerp(fadeImage.color, Color.clear, fadeTime * Time.deltaTime);
            fadeImage.color = color;

        }
    }

    public void StartCutscene()
    {
        coroutine = camAnimationFirst ? PlayAnimation() : PlayClip();
        StartCoroutine(coroutine);
    }

    IEnumerator PlayClip()
    {
        //initialize the clip on the video player
        clip = videoClips[index];
        videoPlayer.clip = clip;
        

           

        videoPlayer.Play();
        yield return new WaitForSecondsRealtime(fadeTime);
        ToggleFade(false);

        yield return new WaitForSecondsRealtime((float)clip.length-fadeTime);

        ToggleFade(true);

        yield return new WaitForSecondsRealtime(fadeTime);
        
        videoPlayer.Stop();       
        

        //continue
        if (camAnimationFirst)
        {
            Cycle();
        }
        else
        {

            coroutine = PlayAnimation();
            StartCoroutine(coroutine);
        }

    }

    void ToggleFade(bool b)
    {
        dark = b;
    }

    IEnumerator PlayAnimation()
    {

        print("playing camera animation");
        playableDirectors[index].gameObject.GetComponent<CinemachineVirtualCamera>().Priority = playableDirectors[index].gameObject.GetComponent<CinemachineVirtualCamera>().Priority * 2;
             

        playableDirectors[index].Play();
        yield return new WaitForSecondsRealtime(fadeTime);
        ToggleFade(false);

        yield return new WaitForSecondsRealtime((float)playableDirectors[index].playableAsset.duration - fadeTime);

        ToggleFade(true);

        yield return new WaitForSecondsRealtime(fadeTime);

        playableDirectors[index].Stop();

        ToggleFade(false);

        //---

        ////playableDirector.ClearGenericBinding(this);
        //PlayableAsset temp = playableAssets[index];

        //playableDirector.playableAsset = temp;
        ////playableDirector.SetGenericBinding(temp, playableAssets[index]);

        ////playableDirector.time = 0;
        //playableDirector.Play(temp);


        ////anim = animations[index];
        ////anim.Play();

        //---


        //anim.Stop();
        //playableDirector.Stop();

        //continue
        if (camAnimationFirst)
        {
            coroutine = PlayClip();
            StartCoroutine(coroutine);
        }
        else
        {
            Cycle();
        }

    }



    void Cycle()
    {
        
        index = index + 1;
        print("Cutscene cycle | index: " + index);

        //if  cam anim is first, do animation array length, otherwise do video clip array length
        int temp = camAnimationFirst ? playableAssets.Length : videoClips.Length;

        //end cutscene if it was last thing, or keep going
        if (index >= temp)
        {
            //end cutscene
            print("Cutscene end");
            SceneManager.LoadScene(2);
        }
        else
        {
            ToggleFade(true);
            coroutine = camAnimationFirst ? PlayAnimation() : PlayClip();
            StartCoroutine(coroutine);
        }

    }

    // Update is called once per frame
    
}
