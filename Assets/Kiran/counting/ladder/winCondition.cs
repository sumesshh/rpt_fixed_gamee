
using UnityEngine;
using UnityEngine.Events;

public class winCondition : MonoBehaviour
{
    public UnityEvent Won;
    private int count = 0;
    public GameObject winMessage;

    public void onInvoke()
    {
        count++;
        Debug.Log(count);
    }
    public void Update()
    {
        if (count == 2)
        {
            Won.Invoke();
            winMessage.SetActive(true);
        }
    }
}
