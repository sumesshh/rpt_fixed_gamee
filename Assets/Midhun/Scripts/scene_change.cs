using System.Collections;
using UnityEngine;

public class scene_change : MonoBehaviour
{
    public GameObject[] Games;
    private int count = 0;


    private void Update()
    {
        
    }
    public void next()
    {
        Debug.Log("button");
        count++;
        UpdateElements();

        if (count == 2) {
            StartCoroutine(AutoSwitchToNext(2f));
        
        }

        // If element 2 (index 1) is activated, start auto-switching after a delay
        /* (count == 2 || count == 3)
        {
            StartCoroutine(AutoSwitchToNext(4f)); // Wait for 3 seconds
        }*/
       
    }

    public void prev()
    {
        count--;
        UpdateElements();
    }

    private void UpdateElements()
    {
        for (int i = 0; i < Games.Length; i++)
        {
            Games[i].SetActive(i == count);
            
        }
        Debug.Log(count);
    }

    private IEnumerator AutoSwitchToNext(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        count++;
        UpdateElements() ;

       
       
    }

    public void PopperDelay() {
        StartCoroutine(Popper(5f));
    }

    private IEnumerator Popper(float delay) { 
        yield return new WaitForSeconds(delay);
        next();
        Debug.Log("Enumerator worked");
        
    }
}
