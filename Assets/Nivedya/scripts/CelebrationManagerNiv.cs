using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CelebrationManagerNiv : MonoBehaviour
{
    public ParticleSystem pop1;
    public ParticleSystem pop2;
    public ParticleSystem pop3;
    public Button winOne;
    public Button winTwo;
    public Button winThree;
    public enum enumtypeNiv
    {
        thoppi,
        kannada,
        tamil,
        telugu
    }
    public enumtypeNiv xyz;
   // enum xyz;
    [System.Serializable]
    public class ParticleAudioPair
    {
        public ParticleSystem particleName;      // Name of the particle system (child of the parent)
        public AudioClip audioClip;      // Audio clip directly assigned in the Inspector
    }

    public ParticleAudioPair[] particleAudioPairs; // Array of particle-audio pairs
    public AudioSource audioSource;

    public enum ParticleIndex
    {
        FirstPair = 0,
        SecondPair = 1,
        ThirdPair = 2,
        // Add more enum entries as needed, matching the array size
    }
    void Start()
    {
        winOne.onClick.AddListener(PlayOne);
        winTwo.onClick.AddListener(PlayTwo);
        winThree.onClick.AddListener(PlayThree);
    }
    public void PlayOne()
    {
        pop1.Play();
    }

    public void PlayTwo()
    {
        pop2.Play();
    }
    public void PlayThree()
    {
        pop3.Play();
    }

    /// <summary>
    /// Plays the particle effect and audio by index.
    /// </summary>
    /// <param name="index">Index of the particle-audio pair to play.</param>
    public void PlayParticleByIndex(ParticleIndex index)
    {
        int intIndex = (int)index;
        if (intIndex < 0 || intIndex >= particleAudioPairs.Length)
        {
            Debug.LogError("Index out of range. Please check the assigned ParticleAudioPairs.");
            return;
        }

        ParticleAudioPair pair = particleAudioPairs[intIndex];

        // Find and play the particle system
        pair.particleName.Play();
        audioSource.clip = pair.audioClip;
        audioSource.Play();

    }
    public void abc(enumtypeNiv dupe2)
    {

    }
    public enum EnumType
    {
        abc,
        xyz,
        teo
    }
    /// new lines
    public void FuncWithEnum(EnumType enumType)
    {
        switch (enumType)
        {
            case EnumType.abc:
                //Do something
                break;
            case EnumType.xyz:
                //Do something
                break;
        }
    }

    public void FuncWithEnumValue1() => FuncWithEnum(EnumType.abc);
    public void FuncWithEnumValue2() => FuncWithEnum(EnumType.xyz);
}


