using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Lvl7_Game7 : MonoBehaviour
{

    // Start is called before the first frame update
    
    public int count = 3;
    public GameObject apple1;
    public GameObject apple2;
    public GameObject apple3;
    public TextMeshProUGUI countdown;
    public GameObject scoreObj;
    public UnityEvent OnWin;
    

    Button apple1Button;
    Button apple2Button;
    Button apple3Button;
    private ScoreSaver scoreScript;
    void Start()
    {
        if (scoreObj != null)
        {
            scoreScript = scoreObj.GetComponent<ScoreSaver>();
            scoreScript.totalTime = 40f;
            scoreScript.elapsedTime = 40f;
        }
        apple1Button = apple1.GetComponent<Button>();
        apple2Button = apple2.GetComponent<Button>();   
        apple3Button = apple3.GetComponent<Button>();
        countdown.text = "Items remaining: " + count;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReduceCount() {
        count -= 1;
        countdown.text = "Items remaining: " + count;
        if (count == 0) { 
            countdown.gameObject.SetActive(false);
            OnWin.Invoke();
        }
    }

    public void CommonReducer() {
        apple1Button.interactable = false;
        //CommonChangeColor(apple1);
        apple2Button.interactable = false;
        //CommonChangeColor(apple2);
        apple3Button.interactable = false;
        //CommonChangeColor(apple3);
        ReduceCount();
    }

    //private void CommonChangeColor(GameObject targetObject) {
    //    Image image = targetObject.GetComponent<Image>(); // Get Image component
    //    if (image != null && ColorUtility.TryParseHtmlString(hexColor, out Color newColor))
    //    {
    //        image.color = newColor; // Apply the color
    //    }
    //}
}
