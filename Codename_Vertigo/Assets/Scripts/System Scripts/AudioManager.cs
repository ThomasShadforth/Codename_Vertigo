using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] AudioSource masterAudioSource;
    [SerializeField] AudioSource musicAudioSource;
    [SerializeField] AudioSource SFXAudioSource;

    [Header("Music and Sounds")]
    [SerializeField] AudioClip[] musicTracks;
    [SerializeField] AudioClip[] SFX;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayMusic(int trackNumber)
    {

    }

    public void PlaySFX()
    {

    }
}
