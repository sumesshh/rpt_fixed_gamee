using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class butttonNumberRetriever : MonoBehaviour
{
    private int buttonInt;
    public GameObject scoreCalculator;
  
    public void OnButtonClick()
    {
        Scorecalculator scorecalculator = scoreCalculator.GetComponent<Scorecalculator>();
        Button button = GetComponent<Button>();
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        buttonInt = int.Parse(buttonText.text);
        Debug.Log(buttonInt);
        scorecalculator.numberCalculator(buttonInt);
    }
}



