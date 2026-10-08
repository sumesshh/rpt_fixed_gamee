using UnityEngine;
using UnityEngine.Events;

public class CheckBox: MonoBehaviour
{
    public GameObject Tick; 
    public GameObject Cross; 
    public UnityEvent isCorrect;
    public UnityEvent isWrong;


    void Start()
    {
        // Ensure both images are invisible initially
        Tick.SetActive(false);
        Cross.SetActive(false);
    }

    
    public void IfCorrect()
    {
        Tick.SetActive(true);
        Cross.SetActive(false); // Hide Image2
        if (isCorrect != null)
        {
            isCorrect.Invoke();
        }
    }

    // Method to show the second image
    public void IfWrong()
    {
        Tick.SetActive(false); // Hide Image1
        Cross.SetActive(true);  // Show Image2
        if (isWrong != null)
        {
            isWrong.Invoke();
        }
    }
    
}
