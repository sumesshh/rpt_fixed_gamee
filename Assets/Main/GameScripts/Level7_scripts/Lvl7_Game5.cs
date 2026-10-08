using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Lvl7_Game5 : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject tick1;
    public GameObject tick2;
    public GameObject tick3;
    public UnityEvent OnWin;

    private bool won = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CorrectChecker() {
        if (won)
        {
            return;
        }
        if (tick1.activeSelf && tick2.activeSelf && tick3.activeSelf) { 
            OnWin.Invoke();
            won = true;
            
        }
    }
}
