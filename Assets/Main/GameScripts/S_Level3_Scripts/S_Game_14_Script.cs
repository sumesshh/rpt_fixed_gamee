using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

public class S_Game_14_Script : MonoBehaviour, IPointerClickHandler
{
    // Start is called before the first frame update
    public RectTransform[] targetTransform;
    public UnityEvent OnThrow;
    private bool canMove = true;
    public bool isCorrect;
    private static bool gameWon = false;
    //public Ease[] eases;
    //public float[] durations;
    //[SerializeField] private Image netImage;
    //[SerializeField] private Canvas netCanvas;
    //[SerializeField] private int netFinalSortingOrder = 2;
    //private Tween movementTween;
    

    public void OnPointerClick(PointerEventData eventData)
    {
        if (gameWon)
        {
            return;
        }
        if (!canMove) {
            return;
        }
        MoveAlongPath();
        canMove = false;
        if (isCorrect) {
            gameWon = true;
        
        }
    }

    void Start()
    {
        canMove = true;
        gameWon = false;
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }

    void MoveAlongPath()
    {
        Vector3[] path = new Vector3[targetTransform.Length];



        Debug.Log(path);

        for (int i = 0; i < targetTransform.Length; i++)
        {
            path[i] = targetTransform[i].position;
        }




        //movementTween = DOTween.Sequence()
        //.Append(transform.DOPath(path, duration, PathType.CatmullRom)
        //         .SetEase(Ease.OutSine))
        //.Append(transform.DOMove(path))
        //         .OnWaypointChange(OnPointReached)


        ////.Append(transform.DOPath(revPath, duration, PathType.CatmullRom)
        //// .SetEase(Ease.Linear))

        //.Play();


        transform.DOScale(0.6f, 1.5f);
        transform.DOPath(path, 2f, PathType.CatmullRom)
        .SetEase(Ease.OutQuad) // Smooth launch
        .OnComplete(() => {
            Debug.Log("Shot Completed!");
            OnThrow.Invoke();
            }
        );

        //Sequence sequence = DOTween.Sequence();

        //for (int i = 0; i < path.Length; i++)
        //{
        //    sequence.Append(transform.DOMove(path[i], durations[i]).SetEase(eases[i]));
        //}

        //sequence.Play();




    }

    //private void UpdateNetOrder(int newOrder)
    //{
    //    Debug.Log($"Attempting to update net order to {newOrder}");

    //    if (netImage != null)
    //    {
    //        Canvas parentCanvas = netImage.canvas;
    //        if (parentCanvas != null)
    //        {
    //            parentCanvas.overrideSorting = true;
    //            parentCanvas.sortingOrder = newOrder;
    //            Debug.Log($"Updated net image canvas sorting order to {newOrder}");
    //        }
    //        netImage.transform.SetSiblingIndex(newOrder);
    //        Debug.Log($"Updated net image sibling index to {newOrder}");
    //    }
    //    else if (netCanvas != null)
    //    {
    //        netCanvas.overrideSorting = true;
    //        netCanvas.sortingOrder = newOrder;
    //        Debug.Log($"Updated net canvas sorting order to {newOrder}");
    //    }
    //}

    //private void OnPointReached(int waypointIndex)
    //{
    //    if (waypointIndex == 1) // First point (index starts from 0)
    //    {
    //        Debug.Log("First point reached!");
    //        UpdateNetOrder(netFinalSortingOrder);

    //        // Call any function here
    //    }
    //}
}
