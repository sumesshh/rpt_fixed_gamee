using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class checkbutton : MonoBehaviour
{
    public GameObject winMessage;     // UI Text for "YOU WON"
    public GameObject lossMessage;
    public Button checkButton;        // Checkmark button

    private bool countcorrect = false;
    // Start is called before the first frame update
    void Start()
    {
        // Initially hide the win message
        winMessage.SetActive(false);
        lossMessage.SetActive(false);


        // Add a click event listener to the button
        checkButton.onClick.AddListener(OnCheckButtonClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnCheckButtonClick()
    {
        winMessage.SetActive(false);
        lossMessage.SetActive(false);

        if (countcorrect)
        {
            winMessage.SetActive(true); // Show the "YOU WON" message
            Debug.Log("YOU WON!");
        }
        else
        {
            lossMessage.SetActive(true); // Show the "YOU LOSS" message
            Debug.Log("YOU LOSS!");
        }
    }
}
