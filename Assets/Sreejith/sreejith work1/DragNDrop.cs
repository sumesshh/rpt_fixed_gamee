using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;
using UnityEngine.Events;

public class DragNDrop : MonoBehaviour, IPointerUpHandler ,IPointerDownHandler, IDragHandler, IEndDragHandler
{
    public GameObject target; 
    public Canvas canvas; //Attach the parent canvas of the object to be dragged
    public UnityEvent onTargetZoneDrop;
    public UnityEvent NotOnTargetZoneDrop;
    public float offset = 120f; //Offset between mouse pointer and object when dragging
    public float scaleUpOnClick = 1.5f; //Factor to be scaled up relative to the originial size on selecting the object
    public float scaleDownOnClick = 1.5f;//Factor to be scaled down relative to the original size
    public float scaleUpTime = 0.5f;//Time to scale up when clicked
    public float scaleDownTime = 0.3f; //Time to scale down when clicked
    public float scaleDownFinalTime = 0.5f; //Time to scale down when dragging ends
    public float movementTime = 0.3f; //Increase to make more smooth
    public float moveToTargetTime = 1f; //Time to move to target if inside target
    private Vector2 originalScale;
    private Tween scalingTween;
    private Transform targetTransform;
    private RectTransform rectTransform;
    private Transform childTransform;
    private RectTransform childRect;
    private Vector2 canvasBotLeft, canvasTopRight;
    private Vector2 innerOffset;
    private Outline outline;
   

    // Start is called before the first frame update
    void Start()
    {   
        targetTransform = target.transform.GetComponent<Transform>(); 
        rectTransform = transform.GetComponent<RectTransform>();
        originalScale = transform.localScale;
        childTransform = transform.GetChild(0);
        outline = childTransform.GetComponent<Outline>();
        childRect = childTransform.GetComponent<RectTransform>();

        if (outline != null)
        {
            SetOutlineVisibility(false);
        }


        var corners = new Vector3[4];
        canvas.GetComponent<RectTransform>().GetWorldCorners(corners);

       
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            canvasBotLeft = Camera.main.WorldToScreenPoint(corners[0]);
            canvasTopRight = Camera.main.WorldToScreenPoint(corners[2]);
        }
        else
        {
            canvasBotLeft = new Vector2(corners[0].x, corners[0].y);
            canvasTopRight = new Vector2(corners[2].x, corners[2].y);
        }

    }

    // Update is called once per frame
    void Update()
    {
    
    }

    public void OnDrag(PointerEventData eventData)
    {

        // Convert the pointer position to world space
        Vector3 worldPos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform,eventData.position + innerOffset, canvas.worldCamera, out worldPos);

        //Add the offset
        worldPos.y += offset;

        

        // Ensure the object stays within the canvas bounds by clamping the position

        float scaledWidth = childRect.rect.width * childRect.lossyScale.x;
        float scaledHeight = childRect.rect.height * childRect.lossyScale.y;


        float clampedX = Mathf.Clamp(worldPos.x, canvasBotLeft.x + scaledWidth / 2, canvasTopRight.x - scaledWidth / 2);
        float clampedY = Mathf.Clamp(worldPos.y, canvasBotLeft.y + scaledHeight / 2, canvasTopRight.y - scaledHeight / 2);

        //Move the object to the new position
        rectTransform.DOMove(new Vector3(clampedX, clampedY, rectTransform.position.z), movementTime).SetEase(Ease.Linear);
        
       
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("Finished dragging");

        if (outline != null)
        {
            SetOutlineVisibility(false);
        }

        if (scalingTween != null && scalingTween.IsActive())
        {
            scalingTween.Kill();
        }

        //Rescale to normal size
        transform.DOScale(originalScale, scaleDownFinalTime);

        //Check if draggable object is near the target(Half the width of target)
        if (targetTransform != null )
        {
            // Snap to the center of the target
            float deltax = Mathf.Abs(targetTransform.position.x - childTransform.position.x);
            float deltay = Mathf.Abs(targetTransform.position.y - childTransform.position.y);

            RectTransform targetRect = targetTransform.GetComponent<RectTransform>();  
            float targetScaledWidth = targetRect.sizeDelta.x * targetRect.lossyScale.x;
            float targetScaledHeight = targetRect.sizeDelta.y * targetRect.lossyScale.y;

            if (deltax <= targetScaledWidth / 2 && deltay <= targetScaledHeight / 2) 
            {
                transform.DOMove(targetTransform.position, moveToTargetTime);
                if(onTargetZoneDrop != null)
                {
                    onTargetZoneDrop.Invoke();

                }
                Debug.Log("Inside the target");
            }
            else
            {
                if (NotOnTargetZoneDrop != null) { 
                    NotOnTargetZoneDrop.Invoke();
                }
                Debug.Log("Outside the target");
            }

        }
        

    }

    public void OnPointerDown(PointerEventData eventData)
    {

        if (outline != null) { 
            SetOutlineVisibility(true);
        }
        Debug.Log("Selected");
        innerOffset = new Vector2(rectTransform.position.x,rectTransform.position.y) - eventData.position;
        //Animation for scaling and adding offset to the draggable object when clicked on the object
        float scaledHeight = rectTransform.rect.height * rectTransform.lossyScale.y;
        float clampedY = Mathf.Clamp(rectTransform.position.y + offset, canvasBotLeft.y + scaledHeight / 2, canvasTopRight.y - scaledHeight / 2);
        scalingTween = DOTween.Sequence()
        .Append(transform.DOScale(originalScale * scaleUpOnClick, scaleUpTime))
        //.Join(transform.DOMoveY(clampedY, scaleUpTime)) 
        .Append(transform.DOScale(originalScale * scaleDownOnClick, scaleDownTime))
        .Play();
        
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //
        if (outline != null)
        {
            SetOutlineVisibility(false);
        }
        if (scalingTween != null && scalingTween.IsActive())
        {
            scalingTween.Kill();
        }
        transform.DOScale(originalScale, scaleDownFinalTime);

        
    }

    private void SetOutlineVisibility(bool isVisible)
    {
        // Adjust the color's alpha based on visibility
        Color color = outline.effectColor;
        color.a = isVisible ? 1f : 0f;
        outline.effectColor = color;
    }





}
