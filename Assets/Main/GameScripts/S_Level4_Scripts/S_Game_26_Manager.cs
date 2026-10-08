using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Game_26_Manager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject crown;
    public GameObject firstQn;
    public GameObject secondQn;
    public GameObject textAnimator;
    public float delayTime = 5f;

    private GameObject score;
    private ScoreSaver scoreScript;
    

    private void Awake()
    {
        GameObject fpsCanvas = GameObject.Find("fps");
        if (fpsCanvas != null)
        {
            Transform scoreTransform = fpsCanvas.transform.Find("Score_v1");
            if (scoreTransform != null)
            {
                scoreScript = scoreTransform.GetComponent<ScoreSaver>();
               
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        crown.SetActive(false);
        firstQn.SetActive(true);
        secondQn.SetActive(false);

        if (scoreScript != null)
        {
            scoreScript.pauseTimer();
            Debug.Log("Script accessed");
        }
        else {
            Debug.Log("Script not found");
        }
     
       
        
        StartCoroutine(Delay());
    }


    private IEnumerator Delay() {
        yield return new WaitForSeconds(delayTime);
        crown.SetActive(true);
        firstQn.SetActive(false);
        secondQn.SetActive(true);
        textAnimator.SetActive(true);

        scoreScript.continueTimer();


    
    }
}
