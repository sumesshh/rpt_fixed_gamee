using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class G2_Lvl1_Game6 : MonoBehaviour
{
    // Start is called before the first frame update
    public Button[] buttons;
    public Button backSpace;
    public Button submit;
    public GameObject retry;
    public int rightAnswer;
    public UnityEvent onWin;


    private string input = "";

    private int maxCount = 4;

    public TextMeshProUGUI numberField;
    void Start()
    {
        foreach (var button in buttons) { 
            button.onClick.AddListener(() => Buttonpressed(button));
        }
        backSpace.onClick.AddListener(BackSpacePressed);
        submit.onClick.AddListener(Submit);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Buttonpressed(Button button) { 
        int index = Array.IndexOf(buttons, button);
        if (input.Length <= maxCount) {
            input = input + index.ToString();
            numberField.text = numberField.text + index;
        } 
    }

    private void BackSpacePressed() {
        if (input.Length > 0)
        {
            input = input.Substring(0, input.Length - 1);
            numberField.text = input;
        }
    }

    private void Submit() {
        int number = int.Parse(input);
        if (number == rightAnswer)
        {
            onWin.Invoke();
            retry.gameObject.SetActive(false);
            foreach (var button in buttons)
            {
                button.interactable = false;
            }
            backSpace.interactable = false;
            submit.interactable = false;

        }
        else {
            input = "";
            numberField.text = input;
            StartCoroutine(RetryDelay());
        }
    }

    private IEnumerator RetryDelay() { 
        retry.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        retry.gameObject.SetActive(false);
    }
}
