using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class Lvl9_Game6 : MonoBehaviour, IPointerClickHandler
{
    public GameObject[] coconuts;

    public float maxDelay = 3f;
    public float fallAreaBegin = 0.5f;
    public float fallAreaEnd = 1f;
    private bool canFall;

    public UnityEvent OnWin;


    public void OnPointerClick(PointerEventData eventData)
    {



        transform.DOShakePosition(2f, new Vector3(10f, 0f, 0f), 10, 0f, false, false);

        if (!canFall)
        {
            return;
        }

        foreach (var coco in coconuts)
        {
            float randomDelay = Random.Range(0f, maxDelay); // Random delay for each object
            float fallPos = Random.Range(fallAreaBegin, fallAreaEnd);
            coco.transform.DOMoveY(-fallPos, 1f)
                .SetEase(Ease.OutBounce)
                .SetDelay(randomDelay)
                .OnComplete(() =>
                {
                    

                }); // Randomized start time


        }
        canFall = false;
        StartCoroutine(WinDelay());
    }
    void Start()
    {
        canFall = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator WinDelay() { 
        yield return new WaitForSeconds(maxDelay);
        OnWin.Invoke();

    }
}
