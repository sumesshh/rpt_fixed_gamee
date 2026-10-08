using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class commonCelebrate : MonoBehaviour
{
    public GameObject[] games;
    int currentCanvas = 0;
    public int waitForNext = 3;
    public ParticleSystem celeb1;
    public ParticleSystem celeb2;
    public ParticleSystem celeb3;
    public ParticleSystem celeb4;
    public ParticleSystem celeb5;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PlayOne() {
        if (celeb1 != null)
        {
            celeb1.Play();
            
            //StartCoroutine(Delay());
        }
        
    }

    public void PlayTwo() {
        if (celeb2 != null)
        {
            celeb2.Play();
            
            StartCoroutine(Delay());
        }
    }

    public void PlayThree()
    {
        if (celeb3 != null)
        {
            celeb3.Play();
            
            StartCoroutine(Delay());
        }
    }
    public void PlayFour()
    {
        if (celeb4 != null)
        {
            celeb4.Play();
            StartCoroutine(Delay());
        }
    }

    public void PlayFive()
    {
        if (celeb5 != null)
        {
            celeb5.Play();
            StartCoroutine(Delay());
        }
    }

    public void StopAllParticleSystems()
    {
        // Find all ParticleSystems in the scene
        ParticleSystem[] allParticleSystems = FindObjectsOfType<ParticleSystem>();

        // Stop each ParticleSystem
        foreach (ParticleSystem ps in allParticleSystems)
        {
            ps.Stop();
            ps.Clear();
        }
    }

    private IEnumerator Delay() {
        yield return new WaitForSeconds(waitForNext);
        StopAllParticleSystems();
    
        if (currentCanvas <= games.Length)
        {
            games[currentCanvas+1].SetActive(true);
            games[currentCanvas].SetActive(false);
            currentCanvas++;
        }
    }

   
}
