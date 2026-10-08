//using UnityEngine;
//using UnityEngine.UI;
//using System.Collections;

//public class PanelSnapper : MonoBehaviour
//{
//    public ScrollRect scrollRect; // Reference to the ScrollRect
//    private float[] panelPositions = { 0f, 0.5f, 1f }; // Normalized positions for 3 panels
//    private int currentPanel = 1; // Start at the middle panel
//    private bool isSnapping = false;

//    public GameObject[] scrolls;

//    public int CurrentPanelIndex { get; private set; } // Public getter for tracking panel index

//    private void Update()
//    {
//        if (Input.GetMouseButtonUp(0) && !isSnapping) // When user stops dragging
//        {
//            float scrollPos = scrollRect.horizontalNormalizedPosition;
//            float closestDistance = Mathf.Infinity;

//            // Find the nearest panel position
//            for (int i = 0; i < panelPositions.Length; i++)
//            {
//                float distance = Mathf.Abs(scrollPos - panelPositions[i]);
//                if (distance < closestDistance)
//                {
//                    closestDistance = distance;
//                    currentPanel = i;
//                }
//            }

//            CurrentPanelIndex = currentPanel;
//            // Start the smooth snap
//            StartCoroutine(SmoothSnap(panelPositions[currentPanel]));
//        }
//    }

//    private IEnumerator SmoothSnap(float targetPosition)
//    {
//        isSnapping = true;
//        float duration = 0.2f;
//        float time = 0;
//        float start = scrollRect.horizontalNormalizedPosition;

//        while (time < duration)
//        {
//            time += Time.deltaTime;
//            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(start, targetPosition, time / duration);
//            yield return null;
//        }

//        scrollRect.horizontalNormalizedPosition = targetPosition;
//        isSnapping = false;

//        Debug.Log(CurrentPanelIndex);
//        for (int i = 0; i < 3; i++)
//        {
//            scrolls[i].SetActive(i == CurrentPanelIndex);
//        }
//    }
//}
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class PanelSnapper : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    public ScrollRect scrollRect; // Reference to the ScrollRect
    private float[] panelPositions = { 0f, 0.5f, 1f }; // Normalized positions for 3 panels
    private int currentPanel = 0; // Start at the middle panel
    private bool isSnapping = false;

    public GameObject[] scrolls;

    public GameObject manager;
    public Button nextButton;

    private PanelShift managerScript;

    public int CurrentPanelIndex { get; private set; } // Public getter for tracking panel index

    private float dragStartPosition;
    private float dragEndPosition;
    private float swipeVelocity;

    private void Start()
    {
        nextButton.onClick.AddListener(OnNextButtonClick);
        managerScript = manager.GetComponent<PanelShift>();
    }

    private void OnNextButtonClick() {

        if (currentPanel < panelPositions.Length - 1)
        {
            currentPanel++;
            StartCoroutine(SmoothSnap(panelPositions[currentPanel]));
        }
        
        else {

            managerScript.NextButtonPressed();
        }

    }

    // Detect when the player starts dragging
    public void OnBeginDrag(PointerEventData eventData)
    {
        dragStartPosition = scrollRect.horizontalNormalizedPosition;
    }

    // Detect when the player stops dragging
    public void OnEndDrag(PointerEventData eventData)
    {
        dragEndPosition = scrollRect.horizontalNormalizedPosition;
        swipeVelocity = (dragEndPosition - dragStartPosition) / Time.deltaTime; // Calculate velocity

        DetermineTargetPanel();
    }

    private void DetermineTargetPanel()
    {
        float scrollPos = scrollRect.horizontalNormalizedPosition;
        float closestDistance = Mathf.Infinity;
        int targetPanel = currentPanel;

        // If swipe is fast enough, move to the next or previous panel
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

        Debug.Log(CurrentPanelIndex);
        for (int i = 0; i < 3; i++)
        {
            scrolls[i].SetActive(i == currentPanel);
        }
    }
}


