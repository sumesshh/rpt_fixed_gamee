using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using static Audio_Manager;


public class Audio_Manager: MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;
    public Sound[] MusicSounds, SFXSounds;

    public Slider MusicSlider;                                                  //Slider for music and SFX
    public Slider SFXSlider;
    public AudioMixer MusicMixer;                                               //Audio Mixer for MusicSource
    public AudioMixer SFXMixer;

    public bool isMusicMuted = false;
    public bool isSFXMuted = false;


    [System.Serializable]
    public class Sound                                                          //class name Sound for storing music sounds...
    {
        public string name; 
        public AudioClip clip;
        public bool loop;
    }
    void Start ()   
    {
    isMusicMuted = PlayerPrefs.GetInt("MusicMuted", 0) == 1;                  //check for the mute of music
    musicSource.mute = isMusicMuted;
        if (!isMusicMuted)
        {

            PlayMusic("BackGroundMusic");                                       //play bg music
        }

        if (MusicSlider != null)
        {
            MusicSlider.value = PlayerPrefs.GetFloat("MusicVolume", -10);           //set slider as default -10(max0 to min(-80))
            setMusicVolume();
        }
  
    }
  
    public void PlayMusic(string name)                                          //play music on the string value
    {
        Sound s=Array.Find(MusicSounds, x => x.name == name);
        if (s != null)
        {
            musicSource.clip = s.clip;
            musicSource.loop = s.loop;
            musicSource.Play();
            
        }
        else {
            Debug.Log("Sound Not found...");
        }
    }
    public void MusicMute()                                                        //for mute and unmute Muisc Sound
    {
        isMusicMuted = !isMusicMuted;
        musicSource.mute = isMusicMuted;
        PlayerPrefs.SetInt("MusicMuted", isMusicMuted ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log("isMusicMuted" + isMusicMuted);


        if (!isMusicMuted )                              //play after unmute of music
        {
            PlayMusic("BackGroundMusic");
            musicSource.Play();
        }

    }

    public void SFXMute()                                                          //for mute and unmute SFX Sound
    {
        isSFXMuted = !isSFXMuted;
        SFXSource.mute = isSFXMuted;
        PlayerPrefs.SetInt("SFXMuted", isSFXMuted ? 1 : 0);
        PlayerPrefs.Save();


        if (!isSFXMuted)                              //play after unmute of SFX
        {
            SFXSource.Play();
        }


    }
    public void setMusicVolume()                                                    //Set music volume from 0 to 100
    {
        MusicMixer.SetFloat("volume", MusicSlider.value);
        Debug.Log("volume"+MusicSlider.value);
    }
    public void setSFXVolume()                                                      //Set SFX Volume from 0 to 100
    {
        MusicMixer.SetFloat("volume", SFXSlider.value);
    }
    public void PlaySound(AudioClip clip)                                           //Audio playing with Audio Clip
    {
        if ( isMusicMuted==false)                                                    //checking MusicMuted is false to play audio
        {
            musicSource.clip = clip;
            musicSource.Play();
        }
    }
    public void playSFX(AudioClip clip)
    {
        if (isSFXMuted == false)
        {
            SFXSource.clip = clip;
            SFXSource.Play();
        }
    }
}
