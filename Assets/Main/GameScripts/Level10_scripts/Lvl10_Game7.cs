using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Lvl10_Game7 : MonoBehaviour
{
    public GameObject retryText;
    public GameObject congratulationsText;

    public UnityEvent onWin;
    // Start is called before the first frame update
    void Start()
    {
        retryText.SetActive(false);
        congratulationsText.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GameWon() { 
        retryText.SetActive(false);
        congratulationsText.SetActive(true);
        onWin.Invoke();
    }

    public void WrongDraw()
    {
        StartCoroutine(RetryDelay());

    }

    private IEnumerator RetryDelay() {
        retryText.SetActive(true);
        yield return new WaitForSeconds(2f);
        retryText.SetActive(false);
    }
}
