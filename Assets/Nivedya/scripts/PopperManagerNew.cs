using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.UI;
using TMPro.Examples;

public class PopperManagerNew : MonoBehaviour
{
    // Start is called before the first frame update
    [System.Serializable]
    public class ParticleAudioPair
    {
        public ParticleSystem particlesType;      // Name of the particle system (child of the parent)
        public AudioClip audioClip;      // Audio clip directly assigned in the Inspector
    }

    public ParticleAudioPair[] particleAudioPairs; // Array of particle-audio pairs
    public AudioSource audioPopper;
    //public GameObject sample;
    public bool enableFadeIn = false;
    //public UnityEvent afterPopperPlayed;
    public Image fadeImage;
    public float maxAlpha = 0.78f;
    public float delay = 3f;
    public enum PopEnumType
    {
        popOne, popTwo, popThree, popFour, popFive, popSix, popSeven, popEight, popNine, popTen, pop11, pop12, pop13, pop14, pop15, pop16, pop17, pop18, pop19, pop20, pop21, pop22, pop23, pop24, pop25, pop26, pop27, pop28, pop29, pop30, pop31, pop32, pop33, pop34
    }
    public void PopperFuncWithEnum(PopEnumType enumType)
    {
        switch (enumType)
        {
            case PopEnumType.popOne:
                PlayEffect(0);
                break;
            case PopEnumType.popTwo:
                PlayEffect(1);
                break;
            case PopEnumType.popThree:
                PlayEffect(2);
                break;
            case PopEnumType.popFour:
                PlayEffect(3);
                break;
            case PopEnumType.popFive:
                PlayEffect(4);
                break;
            case PopEnumType.popSix:
                PlayEffect(5);
                break;
            case PopEnumType.popSeven:
                PlayEffect(6);
                break;
            case PopEnumType.popEight:
                PlayEffect(7);
                break;
            case PopEnumType.popNine:
                PlayEffect(8);
                break;
            case PopEnumType.popTen:
                PlayEffect(9);
                break;
            case PopEnumType.pop11:
                PlayEffect(10);
                break;
            case PopEnumType.pop12:
                PlayEffect(11);
                break;
            case PopEnumType.pop13:
                PlayEffect(12);
                break;
            case PopEnumType.pop14:
                PlayEffect(13);
                break;
            case PopEnumType.pop15:
                PlayEffect(14);
                break;
            case PopEnumType.pop16:
                PlayEffect(15);
                break;
            case PopEnumType.pop17:
                PlayEffect(16);
                break;
            case PopEnumType.pop18:
                PlayEffect(17);
                break;
            case PopEnumType.pop19:
                PlayEffect(18);
                break;
            case PopEnumType.pop20:
                PlayEffect(19);
                break;
            case PopEnumType.pop21:
                PlayEffect(20);
                break;
            case PopEnumType.pop22:
                PlayEffect(21);
                break;
            case PopEnumType.pop23:
                PlayEffect(22);
                break;
            case PopEnumType.pop24:
                PlayEffect(23);
                break;
            case PopEnumType.pop25:
                PlayEffect(24);
                break;
            case PopEnumType.pop26:
                PlayEffect(25);
                break;
            case PopEnumType.pop27:
                PlayEffect(26);
                break;
            case PopEnumType.pop28:
                PlayEffect(27);
                break;
            case PopEnumType.pop29:
                PlayEffect(28);
                break;
            case PopEnumType.pop30:
                PlayEffect(29);
                break;
            case PopEnumType.pop31:
                PlayEffect(30);
                break;
            case PopEnumType.pop32:
                PlayEffect(31);
                break;
            case PopEnumType.pop33:
                PlayEffect(32);
                break;
            case PopEnumType.pop34:
                PlayEffect(33);
                break;
            default:
                Debug.LogError("Invalid enum type provided.");
                break;
        }
    }
    private void Start()
    {
        if (fadeImage != null)
        {
            Color color = fadeImage.color;
            color.a = 0; // Set alpha to 0 (fully transparent)
            fadeImage.color = color;
        }
    }

    public void PlayEffect(int index)
    {
        ParticleAudioPair pair = particleAudioPairs[index];

        if (enableFadeIn && fadeImage != null)
        {
            StartCoroutine(FadeInWithParticles(pair));
        }
        else
        {
            PlayParticleEffect(pair);
        }
    }

    private void PlayParticleEffect(ParticleAudioPair pair)
    {
        pair.particlesType.Play();
        audioPopper.clip = pair.audioClip;
        audioPopper.Play();

        StartCoroutine(WaitForParticleFinish(pair.particlesType));
    }

    private IEnumerator FadeInWithParticles(ParticleAudioPair pair)
    {
        // Start particle effect and audio immediately
        PlayParticleEffect(pair);

        // Start fade-in effect with maxAlpha limit
        Color color = fadeImage.color;
        float fadeDuration = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            color.a = Mathf.Lerp(0, maxAlpha, elapsedTime / fadeDuration); // Fade only to maxAlpha
            fadeImage.color = color;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        color.a = maxAlpha; // Ensure it reaches the defined max alpha
        fadeImage.color = color;

        // Wait for seconds before making it disappear instantly
        yield return new WaitForSeconds(delay);

        // Instantly set alpha back to 0 (make it disappear)
        color.a = 0;
        fadeImage.color = color;
    }

    private IEnumerator WaitForParticleFinish(ParticleSystem particleSystem)
    {
        // Check if the main particle system or any subemitters are still playing
        while (particleSystem.isPlaying || AreSubEmittersPlaying(particleSystem))
        {
            yield return null; // Wait until the next frame
        }

        // After all particles finish playing, invoke SampleMethod
        // Wait for 6 seconds before invoking the event
        yield return new WaitForSeconds(6f);

        // Invoke the afterPopperPlayed event after delay
        //afterPopperPlayed.Invoke();
    }

    // Helper method to check if any subemitter is still playing
    private bool AreSubEmittersPlaying(ParticleSystem parentParticle)
    {
        int subEmitterCount = parentParticle.subEmitters.subEmittersCount;

        for (int i = 0; i < subEmitterCount; i++)
        {
            ParticleSystem subEmitter = parentParticle.subEmitters.GetSubEmitterSystem(i);
            if (subEmitter != null && subEmitter.isPlaying)
            {
                return true; // A subemitter is still playing
            }
        }

        return false; // No subemitters are playing
    }



    public void PopperBurst() => PopperFuncWithEnum(PopEnumType.popOne);
    public void PopperSides() => PopperFuncWithEnum(PopEnumType.popTwo);
    public void PopperFirework() => PopperFuncWithEnum(PopEnumType.popThree);
    public void PopperBalloon() => PopperFuncWithEnum(PopEnumType.popFour);
    public void PopperBlast() => PopperFuncWithEnum(PopEnumType.popFive);
    public void PopperSideBlast() => PopperFuncWithEnum(PopEnumType.popSix);
    public void PopperLongCone() => PopperFuncWithEnum(PopEnumType.popSeven);
    public void PopperRain() => PopperFuncWithEnum(PopEnumType.popEight);
    public void PopperNewBurst() => PopperFuncWithEnum(PopEnumType.popNine);
    public void PopperFireWork1() => PopperFuncWithEnum(PopEnumType.popTen);
    public void PopperFireWork2() => PopperFuncWithEnum(PopEnumType.pop11);
    public void PopperFireWork3() => PopperFuncWithEnum(PopEnumType.pop12);

    public void PopperConfettiaircute() => PopperFuncWithEnum(PopEnumType.pop13);
    public void PopperConfettisimple() => PopperFuncWithEnum(PopEnumType.pop14);
    public void PopperConfettiplus() => PopperFuncWithEnum(PopEnumType.pop15);
    public void PopperConfettisparks() => PopperFuncWithEnum(PopEnumType.pop16);
    public void PopperConfettibig() => PopperFuncWithEnum(PopEnumType.pop17);
    public void PopperConfettiblue() => PopperFuncWithEnum(PopEnumType.pop18);
    public void PopperConfettifallingone() => PopperFuncWithEnum(PopEnumType.pop19);
    public void PopperConfettifallingtwo() => PopperFuncWithEnum(PopEnumType.pop20);
    public void PopperConfettifallingthree() => PopperFuncWithEnum(PopEnumType.pop21);
    public void PopperConfettigold() => PopperFuncWithEnum(PopEnumType.pop22);
    public void PopperConfettigreen() => PopperFuncWithEnum(PopEnumType.pop23);
    public void PopperConfettigroundc() => PopperFuncWithEnum(PopEnumType.pop24);
    public void PopperConfettigrounds() => PopperFuncWithEnum(PopEnumType.pop25);
    public void PopperConfettiloop() => PopperFuncWithEnum(PopEnumType.pop26);
    public void PopperConfettiloopc() => PopperFuncWithEnum(PopEnumType.pop27);
    public void PopperConfettipink() => PopperFuncWithEnum(PopEnumType.pop28);
    public void PopperConfettirandom() => PopperFuncWithEnum(PopEnumType.pop29);
    public void PopperConfettirandomc() => PopperFuncWithEnum(PopEnumType.pop30);
    public void PopperConfettisimpleex() => PopperFuncWithEnum(PopEnumType.pop31);
    public void PopperConfettisparkstwo() => PopperFuncWithEnum(PopEnumType.pop32);
    public void Poppersideone() => PopperFuncWithEnum(PopEnumType.pop33);
    public void PopperLongSphere() => PopperFuncWithEnum(PopEnumType.pop34);
}
