using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class S_Game_16_Manager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject bigPanda;
    public GameObject smallPanda;

    public GameObject bigRabbit;
    public GameObject smallRabbit;

    public GameObject bigFox;
    public GameObject smallFox;

    public GameObject scoreObj;
    private ScoreSaver scoreScript;

    public UnityEvent Win;

    private int count;
    void Start()
    {
        if (scoreObj != null) {
            scoreScript = scoreObj.GetComponent<ScoreSaver>();
            scoreScript.totalTime = 50;
            scoreScript.elapsedTime = 50;

        }
        
        bigFox.SetActive(false);
        smallFox.SetActive(false);

        bigRabbit.SetActive(false);
        smallRabbit.SetActive(false);

        bigPanda.SetActive(true);
        smallPanda.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangePair() {
        StartCoroutine(DelayedCheck());
    }

    private IEnumerator DelayedCheck() {
        yield return new WaitForSeconds(1.1f);
        if (count == 0 && !bigPanda.activeSelf && !smallPanda.activeSelf)
        {

            bigRabbit.SetActive(true);
            smallRabbit.SetActive(true);
            count = count + 1;

        }
        else if (count == 1 && !bigRabbit.activeSelf && !smallRabbit.activeSelf)
        {
            bigFox.SetActive(true);
            smallFox.SetActive(true);
            count = count + 1;
        }
        else if (count == 2 && !bigFox.activeSelf && !smallFox.activeSelf) {
            Win.Invoke();
        
        }

    }
}
