using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasResetter : MonoBehaviour
{
    public GameObject canvas;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetCanvas() { 
        canvas.SetActive(false);
        canvas.SetActive(true);
        Debug.Log("resetted");
    }
}
