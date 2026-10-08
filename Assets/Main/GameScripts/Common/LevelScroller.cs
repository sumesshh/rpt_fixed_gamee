using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelScroller : MonoBehaviour, IEndDragHandler, IBeginDragHandler
{
    public ScrollRect scrollRect;
    public RectTransform content;
    public RectTransform viewport;
    public int totalLevels;
    public float scrollSpeed = 10f;

    private int currentIndex = 0;
    private float[] targetPositions;

    private bool isSnapping = false;

    void Start()
    {
        CalculateTargetPositions();
    }

    void Update()
    {
        //float target = targetPositions[currentIndex];
        //scrollRect.horizontalNormalizedPosition = Mathf.Lerp(scrollRect.horizontalNormalizedPosition, target, Time.deltaTime * scrollSpeed);
        
       
        // Stop snapping if close enough
        //if (Mathf.Abs(scrollRect.horizontalNormalizedPosition - target) < 0.001f)
        //{
        //    scrollRect.horizontalNormalizedPosition = target;
        //    isSnapping = false;
        //}

        if (!isSnapping) {
            float target = targetPositions[currentIndex];
            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(scrollRect.horizontalNormalizedPosition, target, Time.deltaTime * scrollSpeed);
        }
        

    }

    public void OnNext()
    {
        if (currentIndex < totalLevels - 1)
            currentIndex++;
    }

    public void OnPrevious()
    {
        if (currentIndex > 0)
            currentIndex--;
    }

    void CalculateTargetPositions()
    {
        int childCount = content.childCount;
        targetPositions = new float[childCount];

        if (childCount == 1)
        {
            targetPositions[0] = 0.5f;
            return;
        }

        for (int i = 0; i < childCount; i++)
        {
            targetPositions[i] = (float)i / (childCount - 1);
        }
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        float currentPos = scrollRect.horizontalNormalizedPosition;

        // Find nearest target index
        float minDistance = float.MaxValue;
        int closestIndex = currentIndex;

        for (int i = 0; i < targetPositions.Length; i++)
        {
            float distance = Mathf.Abs(currentPos - targetPositions[i]);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestIndex = i;
            }
        }

        currentIndex = closestIndex;
        isSnapping = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isSnapping = true;
    }


}
