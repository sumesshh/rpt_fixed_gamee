using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class toggleSwitcher : MonoBehaviour
{
    public Toggle agreeToggle;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void FlipToggle() { 
        agreeToggle.isOn = !agreeToggle.isOn;
    }
}
