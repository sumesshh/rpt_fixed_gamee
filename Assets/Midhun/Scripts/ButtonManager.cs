using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Find all buttons in the scene and add a listener
        Button[] buttons = FindObjectsOfType<Button>();

        foreach (Button btn in buttons)
        {
            btn.onClick.AddListener(() => DisableButton(btn));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DisableButton(Button btn) {
        btn.interactable = false;
 
    }
}
