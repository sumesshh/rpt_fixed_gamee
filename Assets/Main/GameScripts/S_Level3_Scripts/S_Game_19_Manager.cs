using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class S_Game_19_Manager : MonoBehaviour, IPointerClickHandler
{
    public GameObject[] pappayas;
    private bool canFall;
    public GameObject firstQn;
    public GameObject secondQn;
    public GameObject textAnimator;
    public float maxDelay = 3f;
    public float fallAreaBegin = 1.4f;
    public float fallAreaEnd = 30.0f;
    public GameObject scoreObj;
    private ScoreSaver scoreScript;

    public void OnPointerClick(PointerEventData eventData)
    {
       


        transform.DOShakePosition(3f, new Vector3(10f, 0f, 0f), 10, 0f, false, false);

        if (!canFall) {
            return;
        }

        foreach (var pappaya in pappayas)
        {
            float randomDelay = Random.Range(0f, maxDelay); // Random delay for each object
            float fallPos = Random.Range(fallAreaBegin, fallAreaEnd);
            pappaya.transform.DOMoveY(-fallPos, 1f)
                .SetEase(Ease.OutBounce)
                .SetDelay(randomDelay)
                .OnComplete(() => {
                    newDrag dragScript = pappaya.GetComponent<newDrag>();
                    dragScript.changeInitialPos();
                    dragScript.allowMove = true;
                
                }); // Randomized start time


        }
        firstQn.gameObject.SetActive(false);
        secondQn.gameObject.SetActive(true);
        textAnimator.SetActive(true);
        canFall = false;


    }

    // Start is called before the first frame update
    void Start()
    {
        if (scoreObj != null) { 
            scoreScript = scoreObj.GetComponent<ScoreSaver>();
            scoreScript.totalTime = 50f;
            scoreScript.elapsedTime = 50f;
        }
        StartCoroutine(Delay());
        canFall = true;
        secondQn.SetActive(false);
        textAnimator .SetActive(false);
        
        
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }
    private IEnumerator Delay() {
        yield return new WaitForSeconds(0.1f);
        foreach (var pappaya in pappayas)
        {
            newDrag dragScript = pappaya.GetComponent<newDrag>();
            dragScript.allowMove = false;

        }

    }
}
