using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class smoothDrag : MonoBehaviour,IDragHandler
{
    private RectTransform draggingObjectRectTransform;
    private Vector3 velocity = Vector3.zero;
    public float dampingSpeed = 0.5f;

    private void Awake()
    {
        draggingObjectRectTransform = transform as RectTransform;
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(draggingObjectRectTransform, eventData.position,
            eventData.pressEventCamera, out var globalMousePosition)) {
            //draggingObjectRectTransform.position = Vector3.SmoothDamp(draggingObjectRectTransform.position,
                //globalMousePosition,ref velocity, dampingSpeed);
            //draggingObjectRectTransform.position = Vector3.Lerp(draggingObjectRectTransform.position,
              //  globalMousePosition, dampingSpeed);
            float distance = Vector3.Distance(draggingObjectRectTransform.position, globalMousePosition);
            float duration = Mathf.Clamp(distance * 0.1f, 0.01f, 0.2f);
            //transform.DOMove(globalMousePosition, duration);
            transform.Translate(eventData.delta);

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
