using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class counter : MonoBehaviour
{
    private int number;
    private int count10 = 0;
    private int count1 = 0;
    private int totalCount;
    public TextMeshProUGUI text10;
    public TextMeshProUGUI text1;
    public TextMeshProUGUI totalCountText;
    public TextMeshProUGUI question;
    public UnityEvent Won;
    public UnityEvent wrong;
    public bool gameWon = false;
    

    public void Start()
    {
        number = int.Parse(question.text);
        Debug.Log("number:"+number);
    }
    public void Update()
    {
        
        
    }
    public void countMatchstick10()
    {
        if (gameWon) return;
        count10 = count10 + 10;
        checker();

    }
    public void countMatchstick1() 
    {
        if (gameWon) return;
        count1++;
        checker();
    }

    public void checker() {

        totalCount = count10 + count1;
        totalCountText.text = totalCount.ToString();
        text10.text = count10.ToString();
        text1.text = count1.ToString();
        if (totalCount == number)
        {
            gameWon = true;
            Won.Invoke();
            Debug.Log("won");
        }
        if (totalCount > number)
        {
            wrong.Invoke();
            Debug.Log("lost");
        }
        Debug.Log("totalCount" + totalCount);
    }
}
