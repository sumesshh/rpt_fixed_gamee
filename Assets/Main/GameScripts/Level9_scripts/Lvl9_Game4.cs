using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class Lvl9_Game4 : MonoBehaviour
{
    public Transform bamboo; // Assign the bamboo GameObject's Transform
    public float scaleXAmount = 1.2f; // How much to scale on the X-axis
    public float scaleYAmount = 2.0f; // How much to scale on the Y-axis
    public float duration = 2f; // Time taken to grow

    public RectTransform targetArea; // Assign your target UI GameObject
    public Camera uiCamera; // Assign the UI camera used in Screen Space - Camera mode

    public RectTransform draggableRect;

    public UnityEvent onWin;

    private bool isGrowing = false;
    private bool hasWon = false;

    private newDrag dragObject;


    public void GrowBamboo()
    {
        if (bamboo == null) return;

        Vector3 newScale = new Vector3(scaleXAmount, scaleYAmount, bamboo.localScale.z);
        bamboo.DOScale(newScale, duration).SetEase(Ease.OutBack); // Smooth growing effect
    }
    // Start is called before the first frame update
    void Start()
    {
        dragObject = draggableRect.GetComponent<newDrag>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hasWon) return;

        if (dragObject.isDragging)
        {
            draggableRect.eulerAngles = new Vector3(0f, 0f, 20f);
        }
        else {
            draggableRect.eulerAngles = new Vector3(0f, 0f, 0f);
        }

        if (IsOverlapping(draggableRect, targetArea))
        {
            Debug.Log("Draggable object entered the target!");
            StartGrowing();
        }
        else {
            StopGrowing();
        }

        CheckWinCondition();
    }

    private bool IsOverlapping(RectTransform rect1, RectTransform rect2)
    {
        Vector3[] worldCorners = new Vector3[4];
        rect1.GetWorldCorners(worldCorners);

        foreach (Vector3 corner in worldCorners)
        {
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(uiCamera, corner);
            if (RectTransformUtility.RectangleContainsScreenPoint(rect2, screenPoint, uiCamera))
            {
                return true;
            }
        }
        return false;
    }

    private void StartGrowing()
    {
        if (isGrowing) return; // Prevent multiple tween calls

        isGrowing = true;
        Vector3 newScale = new Vector3(scaleXAmount, scaleYAmount, bamboo.localScale.z);
        bamboo.DOScale(newScale, duration).SetEase(Ease.Linear); // Continuous growth while inside
    }

    private void StopGrowing()
    {
        isGrowing = false;
        bamboo.DOKill(); // Stop the animation when the cup exits the target area
    }

    private void CheckWinCondition()
    {
        if (hasWon) return;

        if (Mathf.Approximately(bamboo.localScale.x, scaleXAmount) &&
            Mathf.Approximately(bamboo.localScale.y, scaleYAmount))
        {
            Debug.Log("Win! Bamboo has fully grown!");
            if (onWin != null) { 
                onWin.Invoke();
            }
            hasWon = true;
            StopGrowing(); // Stop further growth
        }
    }
}
