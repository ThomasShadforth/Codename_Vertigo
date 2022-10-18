using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{

    public AudioMixerGroup masterAudioGroup;
    public AudioMixerGroup musicAudioGroup;
    public AudioMixerGroup SFXAudioGroup;

    public void SetMasterVolume(float volume)
    {
        
        masterAudioGroup.audioMixer.SetFloat("Master_Volume", Mathf.Log10(volume) * 20);
    }

    public void SetMusicVolume(float volume)
    {
        musicAudioGroup.audioMixer.SetFloat("Music_Volume", Mathf.Log10(volume) * 20);
    }

    public void SetSFXVolume(float volume)
    {
        SFXAudioGroup.audioMixer.SetFloat("FX_Volume", Mathf.Log10(volume) * 20);
    }

    public void SetGraphicsQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }
}
