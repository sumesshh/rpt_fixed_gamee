using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class celebrationManager : MonoBehaviour
{
    public ParticleSystem celebration1;
    public ParticleSystem celebration2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

     public void PlayOne() {
        if (celebration1 != null)
        {
            celebration1.Play();
        }

       
     }

    public void PlayTwo()
    {
        if (celebration2 != null)
        {
            celebration2.Play();
        }


    }
}
