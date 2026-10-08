using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class popperscript : MonoBehaviour
{
    // Start is called before the first frame update
    public ParticleSystem MyPopper;
    public GameObject gameprev;
    public GameObject gamenext;
    void Start()
    {
        
    }

    // Update is called once per frame
   
    void Update()
    {
        
    }
    public void PopperPlay()
    {
        MyPopper.Play();
        Invoke("canvaschange", 2.0f);

    }
    public void canvaschange()
    {
        gameprev.SetActive(false);
        gamenext.SetActive(true);
    }
}
