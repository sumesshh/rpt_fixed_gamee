using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private bool DevelopmentMode;
    [SerializeField] private GameObject console;

    private void Start()
    {
        if(DevelopmentMode)
        console.SetActive(true);
    }
}
