using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Celebrations : MonoBehaviour
{
    public ParticleSystem one;
    public AudioSource Celeb;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void playOne()
    {
        one.Play();
        Celeb.Play();
    }
}
