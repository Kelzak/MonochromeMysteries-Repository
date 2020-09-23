using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer musicAudioMixer;
    public AudioMixer SFXAudioMixer;
    public AudioMixer MasterAudioMixer;

    public void SetMusicVolume(float volume)
    {
        musicAudioMixer.SetFloat("MusicVolume", volume); 
    }
    public void SetSFXVolume(float volume)
    {
        SFXAudioMixer.SetFloat("SFXVolume", volume);
    }
    public void SetMasterVolume(float volume)
    {
        MasterAudioMixer.SetFloat("MasterVolume", volume);
    }

    public void ToggleFullscreen(bool fullscreen)
    {
        if (fullscreen)
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
            //Debug.Log("is fullscreen");
        }

        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            //Debug.Log("is windowed");
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
