using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
public class Scorecalculator : MonoBehaviour
{
    public TextMeshProUGUI answertext;
    public int questionInt;
    private int count = 1;
    private int answerInt = 0;
    public UnityEvent Won;

    public void numberCalculator(int num)
    {
        if (count > 0)
        {
            answerInt = answerInt + num;
            count--;
        }
        else if (count == 0)
        {
            answerInt = answerInt * 10 + num;
            checkWin();
        }
        
      
        answertext.text = answerInt.ToString();
    }

    public void checkWin()
    {
        if (answerInt == questionInt)
        {
            Won.Invoke();
            Debug.Log("Win");
        }
        else
        {
            Debug.Log("lost");
        }
    }
}
    
