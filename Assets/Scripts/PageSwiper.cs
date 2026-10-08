using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Bson;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PageSwiper : MonoBehaviour, IDragHandler, IEndDragHandler
{
    private Vector3 panelLocation;
    public float percentThreshold = 0.2f;
    public float easing = 0.5f;
    public List<RectTransform> panels; // Assign panels in the Inspector

    public GameObject scroll1 ;
    public GameObject scroll2 ;
    public GameObject scroll3 ;

    private int panelIndex;
    void Start()
    {
        panelLocation = transform.localPosition;
        LogCurrentPanelName(); // Log initial panel
    }

    public void OnDrag(PointerEventData data)
    {
        float difference = data.pressPosition.x - data.position.x;
        float newX = panelLocation.x - difference;

        // Calculate allowed X positions based on panel count
        float minX = -(panels.Count - 1) * Screen.width;
        float maxX = 0;
        newX = Mathf.Clamp(newX, minX, maxX);

        transform.localPosition = new Vector3(newX, panelLocation.y, panelLocation.z);
    }

    public void OnEndDrag(PointerEventData data)
    {
        float percentage = (data.pressPosition.x - data.position.x) / Screen.width;
        int currentPanelIndex = Mathf.RoundToInt(panelLocation.x / -Screen.width);

        if (Mathf.Abs(percentage) >= percentThreshold)
        {
            Vector3 newLocation = panelLocation;

            if (percentage > 0) // Swipe left
            {
                if (currentPanelIndex < panels.Count - 1)
                {
                    newLocation += new Vector3(-Screen.width, 0, 0);
                }
                else
                {
                    StartCoroutine(SmoothMove(transform.localPosition, panelLocation, easing));
                    return;
                }
            }
            else if (percentage < 0) // Swipe right
            {
                if (currentPanelIndex > 0)
                {
                    newLocation += new Vector3(Screen.width, 0, 0);
                }
                else
                {
                    StartCoroutine(SmoothMove(transform.localPosition, panelLocation, easing));
                    return;
                }
            }

            StartCoroutine(SmoothMove(transform.localPosition, newLocation, easing));
            panelLocation = newLocation;
        }
        else
        {
            StartCoroutine(SmoothMove(transform.localPosition, panelLocation, easing));
        }
    }

    IEnumerator SmoothMove(Vector3 startpos, Vector3 endpos, float seconds)
    {
        float t = 0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / seconds;
            transform.localPosition = Vector3.Lerp(startpos, endpos, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
        LogCurrentPanelName(); // Log after animation completes
    }

    private void LogCurrentPanelName()
    {
        panelIndex = Mathf.RoundToInt(transform.localPosition.x / -Screen.width);
        if (panelIndex >= 0 && panelIndex < panels.Count)
        {
            //Debug.Log("Current Panel: " + panels[panelIndex].name);
            Debug.Log("panel Index: " + panelIndex);
        }
        else
        {
            Debug.LogWarning("Invalid panel index: " + panelIndex);
        }
        
    }
    void Update()
    {
        if (panelIndex == 0)
        {
            scroll1.SetActive(true);
            scroll2.SetActive(false);
            scroll3.SetActive(false);
        }
        else if (panelIndex == 1)
        {
            scroll1.SetActive(false);
            scroll2.SetActive(true);
            scroll3.SetActive(false);
        }
        else if (panelIndex == 2)
        {
            scroll1.SetActive(false);
            scroll2.SetActive(false);
            scroll3.SetActive(true);
        }
    }
}