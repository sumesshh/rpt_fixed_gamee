using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class S_Game_7_Parent : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] objects;
    public GameObject scoreObj;

    //public GameObject hen2;
    //public GameObject hen3;
    //public GameObject hen4;
    public GameObject LevelManager;
    public UnityEvent Win;
    public bool gameWin;
    private ScoreSaver scoreScript;
    void Start()
    {
        if (scoreObj != null) {
            scoreScript = scoreObj.GetComponent<ScoreSaver>();
            scoreScript.totalTime = 40f;
            scoreScript.elapsedTime = 40f;

        }
        
        gameWin = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EndChecker() {

        StartCoroutine(DelayedCheck());
    }

    private IEnumerator DelayedCheck() {
        yield return new WaitForSeconds(1.1f);
        bool isWin = true;
        foreach (GameObject obj in objects) {
            if (obj.activeSelf) {
                isWin = false;
            }
        }
        if (isWin) { 
            gameWin = true;
            Debug.Log("Game Won");
        }
        if (isWin && Win != null) { 
            Win.Invoke();
        }

        /*
        if (!hen1.activeSelf && !hen2.activeSelf && !hen3.activeSelf && !hen4.activeSelf)
        {
            if (Win != null) { 
                Win.Invoke();
            }
        }*/
    }

    public void GameWin()
    {
        Debug.Log("Game Passed");
        S_Level_1_Manager scr = LevelManager.GetComponent<S_Level_1_Manager>();
        scr.PopperDelay();

    }


}
