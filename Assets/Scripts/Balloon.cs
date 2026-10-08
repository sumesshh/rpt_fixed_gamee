using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Balloon : MonoBehaviour
{
    public GameObject scoreObj;
    public GameObject manager;
    private Animator anim;
    public int count = 0;
    public AudioSource audio;
    public AudioSource audio1;
    public static int poppedBalloonCount = 0; // Tracks the number of balloons popped
    public int targetBalloonCount = 6;  // The target count to win
    public Button tick;
    public GameObject winMessage;
    private GameObject retryMessage;

    public UnityEvent onWin;

    private bool gameWon;
    private S_Level_1_Manager managerScript;
    private ScoreSaver scoreScript;
    // Start is called before the first frame update
    void Start()
    {
        if (scoreObj != null)
        {
            scoreScript = scoreObj.GetComponent<ScoreSaver>();
            scoreScript.totalTime = 50f;
            scoreScript.elapsedTime = 50f;
        }
        Debug.Log("Message in start");
        managerScript = manager.GetComponent<S_Level_1_Manager>();
        gameWon = false;
        anim = GetComponent<Animator>();
        tick.onClick.AddListener(check);
        retryMessage = managerScript.activeCanvas.transform.Find("UI_Txt_retry (3)").gameObject;
        if (retryMessage != null)
        {
            Debug.Log("Retry not null");
            if (retryMessage.activeSelf)
            {
                Debug.Log("Retry Active");
                StartCoroutine(RetryDelay());
            }

        }
        else {
            Debug.Log("Retry null");
        }
       

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseEnter()
    {
        if (gameWon) return;
        anim.Play("balloonpop");
        audio.Play();
        poppedBalloonCount=poppedBalloonCount+1;

        // Check if the target has been reached
      

        Destroy(gameObject, 0.5f);
    }

    void check()
    {
        Debug.Log(poppedBalloonCount + "Popped Balloon Count");
        Debug.Log(targetBalloonCount + "Target Balloon Count");
        if (poppedBalloonCount == targetBalloonCount)
        {
            Debug.Log("True");
            ShowWinMessage();
            onWin.Invoke();
            gameWon = true;
        }
        else {
            Debug.Log("False");
            managerScript.ResetCanvasWithoutTimerReset();
            poppedBalloonCount = 0;
            retryMessage = managerScript.activeCanvas.transform.Find("UI_Txt_retry (3)").gameObject;
            retryMessage.SetActive(true);
           // ShowRetryMessage();
        }
        //else if (poppedBalloonCount > targetBalloonCount)
        //{
        //    ShowWinMessage();
        //}
    }

    void ShowWinMessage()
    {
        // Display the "You Won!" message

        if (retryMessage != null) {
            retryMessage.SetActive(false);
        }
        
            winMessage.SetActive(true);
        if (audio1 != null) {
            audio1.Play();
        }
           
      

        Debug.Log("You Won!");
    }

    private IEnumerator RetryDelay() {
        yield return new WaitForSeconds(2f);
        retryMessage.SetActive(false);
        Debug.Log("Retry Deactivated");
    }

    void ShowRetryMessage()
    {
        retryMessage.SetActive(true);
        Debug.Log("Retry Activated");
        Invoke("HideRetryMessage", 2f);
    }

    void HideRetryMessage()
    {
        retryMessage.SetActive(false);
        Debug.Log("Retry Deactivated");
    }


}
