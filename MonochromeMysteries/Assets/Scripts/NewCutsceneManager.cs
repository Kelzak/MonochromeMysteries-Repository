using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.Timeline;

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

    public Image fadeImage;
    public float fadeTime = 3f;
    public bool playOnStart;

    private bool dark;

    //variables
    private int index = 0;
    private IEnumerator coroutine;


    // Start is called before the first frame update
    void Start()
    {
        //initialize fade settings
        fadeImage.color = Color.black;
        dark = true;


        if (playOnStart)
        {
            StartCutscene();
        }

    }

    void StartCutscene()
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

        ToggleFade();

        yield return new WaitForSecondsRealtime((float)clip.length);

        ToggleFade();

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

    void ToggleFade()
    {
        if (dark)
        {
            dark = false;
        }
        else
        {
            dark = true;
        }
    }

    IEnumerator PlayAnimation()
    {
        TimelineAsset temp = playableAssetz[index];
        playableDirector.playableAsset = temp;
        playableDirector.Play();
        

        //anim = animations[index];
        //anim.Play();

        ToggleFade();

        yield return new WaitForSecondsRealtime((float)playableAssetz[index].duration);

        ToggleFade();

        //anim.Stop();
        playableDirector.Stop();

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
        int temp = camAnimationFirst ? playableAssetz.Length : videoClips.Length;

        //end cutscene if it was last thing, or keep going
        if (index >= temp)
        {
            //end cutscene
            print("Cutscene end");
        }
        else
        {
            coroutine = camAnimationFirst ? PlayAnimation() : PlayClip();
            StartCoroutine(coroutine);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (dark)
        {
            //fade color to clear
            fadeImage.color = Color.Lerp(fadeImage.color, Color.black, fadeTime * Time.deltaTime);
            
        }
        else
        {
            //fade color to clear
            fadeImage.color = Color.Lerp(fadeImage.color, Color.clear, fadeTime * Time.deltaTime);
            
        }
    }
}
