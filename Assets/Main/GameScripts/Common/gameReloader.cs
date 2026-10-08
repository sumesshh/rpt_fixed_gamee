using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameReloader : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject game;
    private Camera mainCamera;
    void Start()
    {
        mainCamera = Camera.main;
        Debug.Log("Camera assigned");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void game7reload() {
        GameObject instance = Instantiate(game);
        Canvas instanceCanvas = instance.GetComponent<Canvas>();
        if (instanceCanvas != null)
        {
            instanceCanvas.worldCamera = mainCamera;
        }
    }
}
