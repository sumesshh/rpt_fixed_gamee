using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class v2LevelScroller : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    // Start is called before the first frame update
    public ScrollRect scrollRect; // Reference to the ScrollRect
    private float[] panelPositions; // Normalized positions for 3 panels
    private int currentPanel = 0; // Start at the middle panel
    private bool isSnapping = false;
    public RectTransform content;
    //public GameObject[] scrolls;

    public GameObject manager;
    public Button[] nextButton;
    public Button[] prevButton;

    //private PanelShift managerScript;

    public int CurrentPanelIndex { get; private set; } // Public getter for tracking panel index

    private float dragStartPosition;
    private float dragEndPosition;
    private float swipeVelocity;
    private float dragStartTime;

    private void Start()
    {
        CalculateTargetPositions();
        for (int i = 0; i < nextButton.Length; i++) {
            nextButton[i].onClick.AddListener(OnNextButtonClick);
        }

        for (int i = 0; i < prevButton.Length; i++) {
            prevButton[i].onClick.AddListener(OnPrevButtonClick);
        }
       
        //managerScript = manager.GetComponent<PanelShift>();
    }

    private void OnNextButtonClick()
    {

        if (currentPanel < panelPositions.Length - 1)
        {
            currentPanel++;
            StartCoroutine(SmoothSnap(panelPositions[currentPanel]));
        }

        else
        {

            //managerScript.NextButtonPressed();
        }

    }

    private void OnPrevButtonClick() { 
        currentPanel--;
        StartCoroutine(SmoothSnap(panelPositions[currentPanel]));

    }

    // Detect when the player starts dragging
    public void OnBeginDrag(PointerEventData eventData)
    {
        dragStartPosition = scrollRect.horizontalNormalizedPosition;
        dragStartTime = Time.time;
    }

    // Detect when the player stops dragging
    public void OnEndDrag(PointerEventData eventData)
    {
        dragEndPosition = scrollRect.horizontalNormalizedPosition;
        float dragDuration = Time.time - dragStartTime;
        swipeVelocity = (dragEndPosition - dragStartPosition) / dragDuration; // Calculate velocity

        StartCoroutine(DelayedDeterminePanel());
    }

    private void DetermineTargetPanel()
    {
        float scrollPos = scrollRect.horizontalNormalizedPosition;
        float closestDistance = Mathf.Infinity;
        int targetPanel = currentPanel;
        Debug.Log(swipeVelocity);
        //If swipe is fast enough, move to the next or previous panel
        if (Mathf.Abs(swipeVelocity) > 2f) // Adjust threshold as needed
        {
            if (swipeVelocity > 0 && currentPanel < panelPositions.Length - 1)
                targetPanel = currentPanel + 1; // Move right
            else if (swipeVelocity < 0 && currentPanel > 0)
                targetPanel = currentPanel - 1; // Move left
        }
        else
        {
            // Find the nearest panel if the swipe was slow
            for (int i = 0; i < panelPositions.Length; i++)
            {
                float distance = Mathf.Abs(scrollPos - panelPositions[i]);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    targetPanel = i;
                }
            }
        }



        //for (int i = 0; i < panelPositions.Length; i++)
        //{
        //    float distance = Mathf.Abs(scrollPos - panelPositions[i]);
        //    if (distance < closestDistance)
        //    {
        //        closestDistance = distance;
        //        targetPanel = i;
        //    }
        //}

        CurrentPanelIndex = targetPanel;
        currentPanel = targetPanel;
        StartCoroutine(SmoothSnap(panelPositions[targetPanel]));
    }

    private IEnumerator SmoothSnap(float targetPosition)
    {
        isSnapping = true;
        float duration = 0.3f; // Slightly increased for smoother snapping
        float time = 0;
        float start = scrollRect.horizontalNormalizedPosition;

        while (time < duration)
        {
            time += Time.deltaTime;
            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(start, targetPosition, time / duration);
            yield return null;
        }

        scrollRect.horizontalNormalizedPosition = targetPosition;
        isSnapping = false;

        //Debug.Log(CurrentPanelIndex);
        //for (int i = 0; i < 3; i++)
        //{
        //    scrolls[i].SetActive(i == currentPanel);
        //}
    }

    void CalculateTargetPositions()
    {
        int childCount = content.childCount;
        panelPositions = new float[childCount];

        if (childCount == 1)
        {
            panelPositions[0] = 0.5f;
            return;
        }

        for (int i = 0; i < childCount; i++)
        {
            panelPositions[i] = (float)i / (childCount - 1);
        }
    }

    private IEnumerator DelayedDeterminePanel()
    {
        yield return new WaitForSeconds(0.4f); // Let inertia play out a bit
        DetermineTargetPanel();
    }

}
