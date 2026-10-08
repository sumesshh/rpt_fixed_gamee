using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CountDown : MonoBehaviour
{


    [SerializeField] Image countDown;
    public bool isEnded = false;//the time taken to complete the game

    float elapsedTime = 20;
    bool isStop = false;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    { 
        if (isStop==false||elapsedTime ==0)
        {
            elapsedTime -= Time.deltaTime;
            countDown.fillAmount = elapsedTime / 15;
        }
        
    }
    public void TimerStop()
    {
         isStop = true;
      
    }
}

