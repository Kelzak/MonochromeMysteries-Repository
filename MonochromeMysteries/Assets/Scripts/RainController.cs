using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainController : MonoBehaviour
{
    private AudioSource[] audioSources;
    private AudioSource audioSource;
    public Player player;
    private bool isPlaying0;
    private bool isPlaying1;
    public float rainVolume;
    public float fadeTime;
    public float thunderInterval;
    public float thunderVolume;
    public static bool navInside;

    // Start is called before the first frame update
    void Start()
    {
        audioSources = this.GetComponents<AudioSource>();
        audioSources[0].Play();
        audioSources[1].Play();
        InvokeRepeating("Thunder", 5f, thunderInterval);
    }

    // Update is called once per frame
    void Update()
    {
        player = GameObject.FindObjectOfType<Player>();

        if (player.IsInside() == true || navInside == true)
        {
            PlaySound(0);
        }
        else
        {
            PlaySound(1);
        }

    }
    void PlaySound(int index)
    {
        if(index == 0)
        {
            if (!isPlaying0)
            {
                isPlaying1 = false;
                StartCoroutine(FadeAudioSource.StartFade(audioSources[1], fadeTime, 0f));
                StartCoroutine(FadeAudioSource.StartFade(audioSources[0], fadeTime, rainVolume));
                isPlaying0 = true;
                
            }
        }
        if (index == 1)
        {
            if(!isPlaying1)
            {
                isPlaying0 = false;
                StartCoroutine(FadeAudioSource.StartFade(audioSources[0], fadeTime, 0f));
                StartCoroutine(FadeAudioSource.StartFade(audioSources[1], fadeTime, rainVolume));
                isPlaying1 = true;
               
            }
        }
    }
    void Thunder()
    {
        int rand = Random.Range(2, 7);
        audioSources[rand].volume = thunderVolume;
        audioSources[rand].Play();
    }
}

public static class FadeAudioSource
{

    public static IEnumerator StartFade(AudioSource audioSource, float duration, float targetVolume)
    {
        float currentTime = 0;
        float start = audioSource.volume;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        yield break;
    }
}
