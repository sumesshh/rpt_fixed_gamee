using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class Lvl9_Game5 : MonoBehaviour
{

    public GameObject[] draggableIceCreams;
    public RectTransform target;
    public RectTransform[] smallTargets;
    public UnityEvent onWin;

    private int[] successDrops;
    private int top = 0;

    private bool gameWon = false;

    // Start is called before the first frame update
    void Start()
    {
        successDrops = new int[draggableIceCreams.Length];
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
        if (gameWon) return;
        for (int i = 0; i < draggableIceCreams.Length; i++) {
            GameObject obj = draggableIceCreams[i];
            if (IsInsideTarget(obj) && successDrops[i] != 1) {
                obj.transform.DOMove(smallTargets[top].position, 0.5f);
                top = top + 1;
                successDrops[i] = 1;
                if (top > 2) {
                    gameWon = true;
                    onWin?.Invoke();

                }
            }
        }
    }
}
