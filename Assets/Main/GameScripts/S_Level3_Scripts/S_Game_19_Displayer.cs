using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Game_19_Displayer : MonoBehaviour
{
    public GameObject[] pappayas;
    public GameObject[] draggablePappayas;
    private int count = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PappayaRenderer() {
       
        pappayas[count].gameObject.SetActive(true);
        count++;


        
    }
}
