using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

public class Game_12_Manager : MonoBehaviour
{
    public GameObject[] draggableBananas;
    public GameObject[] bananas;

    public RectTransform target;

    
    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject obj in bananas) { 
            obj.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsInsideTarget(GameObject obj)
    {
        RectTransform objRect = obj.GetComponent<RectTransform>();
        if (objRect == null) return false;

        // Convert world position of object to screen position
        Vector3 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, objRect.position);

        // Get the world corners of the target RectTransform
        Vector3[] corners = new Vector3[4];
        target.GetWorldCorners(corners);

        // Convert corners to screen space
        Vector3 bottomLeft = RectTransformUtility.WorldToScreenPoint(Camera.main, corners[0]);
        Vector3 topRight = RectTransformUtility.WorldToScreenPoint(Camera.main, corners[2]);

        // Check if the object's screen position is inside the target bounds
        return screenPoint.x >= bottomLeft.x && screenPoint.x <= topRight.x &&
               screenPoint.y >= bottomLeft.y && screenPoint.y <= topRight.y;
    }

    public void OnDrop() {
        foreach (GameObject obj in draggableBananas) {
            if (obj.activeSelf && IsInsideTarget(obj)) { 
                int count = Array.IndexOf(draggableBananas, obj);
                bananas[count].SetActive(true);
            }
        }
    
    }
}
