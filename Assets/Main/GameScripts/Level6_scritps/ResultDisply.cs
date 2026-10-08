using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultDisply : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject congratulationsDisplay;
    public GameObject retryDisplay;

 

    void Start()
    {
        congratulationsDisplay.SetActive(false);
        retryDisplay.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSuccess() { 
        retryDisplay.SetActive(false);
        congratulationsDisplay.SetActive(true);
        StartCoroutine(DisableDelay());
    }

    public void OnFailure() {
        retryDisplay?.SetActive(true);
    }

    private IEnumerator DisableDelay() {
        yield return new WaitForSeconds(3f);
        congratulationsDisplay.SetActive(false);
        retryDisplay.SetActive(false);
    }
    

}
