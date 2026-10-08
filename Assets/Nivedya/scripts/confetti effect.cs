using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class confettieffect : MonoBehaviour
{
    // Start is called before the first frame update
    public Button winOne;
    public Button winTwo;
    public Button winThree;
    public Button winFour;
    public ParticleSystem confettiOne;
    public ParticleSystem confettiTwo;
    public ParticleSystem confettiThree;
    public ParticleSystem confettiFour;
    public ParticleSystem confettiFive;
    public ParticleSystem confettiSix;
    public ParticleSystem confettiSeven;
    public ParticleSystem confettiEight;
    void Start()
    {
        winOne.onClick.AddListener(playOne);
        winTwo.onClick.AddListener(playTwo);
        winThree.onClick.AddListener(playThree);
        winFour.onClick.AddListener(playFour);
    }

    public void playOne() { 
        confettiOne.Play();
    }
    public void playTwo() { confettiTwo.Play(); }
    public void playThree() { confettiThree.Play();
        confettiFour.Play();
        confettiFive.Play();
        confettiSix.Play();
    }
    public void playFour() { confettiSeven.Play();
        confettiEight.Play();
    }
}
