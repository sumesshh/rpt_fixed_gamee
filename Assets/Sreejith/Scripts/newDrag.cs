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
using static UnityEngine.ParticleSystem;


//READ ME***
// 1. The object to be dragged should be a child of the GameObject this script is attached to.
// 2. Do NOT modify the image of this GameObject directly.
// 3. Avoid scaling the image; use RectTransform to resize the image instead. Adjust the anchors if you resize the image.
public class newDrag : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IDragHandler, IEndDragHandler
{
    [Header("Main Settings")]
    public Transform target;
    public Canvas canvas; //Attach the parent canvas of the object to be dragged
    public bool PreventMovementAfterDropOnTarget = false; //Set as True to prevent the object from being dragged after being dropped on the target area
    public bool SnapToTargetCenter = true;
    public bool showParticles = true;
    public bool ResetOnDropFail = false;
    public UnityEvent onTargetZoneDrop;
    public UnityEvent NotOnTargetZoneDrop;



    [Header("Drag and Scale Configuration")]
    public float offset = 0f; //Offset between mouse pointer and object when dragging
    public float scaleUpOnClick = 1.5f; //Factor to be scaled up relative to the originial size on selecting the object
    public float scaleDownOnClick = 1.5f;//Factor to be scaled down relative to the original size
    public float scaleUpTime = 0.5f;//Time to scale up when clicked
    public float scaleDownTime = 0.3f; //Time to scale down when clicked
    public float scaleDownFinalTime = 0.5f; //Time to scale down when dragging ends
    public float movementTime = 0.1f; //Increase to make movement hard
    public float moveToTargetTime = 1f; //Time to move to target if inside target
    public float resetSpeed = 5f;


    [Header("Audio")]
    public AudioSource OnClickAudio;


    private Vector2 originalScale;
    private Tween scalingTween;
    private RectTransform targetTransform;
    private RectTransform rectTransform;
    public Transform childTransform;
    public RectTransform childRect;
    private Vector2 canvasBotLeft, canvasTopRight;
    private Vector3 innerOffset;
    private Outline outline;
    private ParticleSystem[] particles;
    public Vector3 initialPos;

    
    public bool allowMove = true;

    [HideInInspector]
    public bool isDragging = false;


    // Start is called before the first frame update
    void Start()
    {
        if (target != null) {
            targetTransform = target.GetComponent<RectTransform>();
        }
        rectTransform = transform.GetComponent<RectTransform>();
        originalScale = transform.localScale;
        childTransform = transform.GetChild(0);
        isDragging = false;
        outline = childTransform.GetComponent<Outline>();
        childRect = childTransform.GetComponent<RectTransform>();
        particles = GetComponentsInChildren<ParticleSystem>();
        initialPos = transform.position;

        Debug.Log(canvas.scaleFactor);
        SetSizeOfParticle();
       

        if (outline != null)
        {
            SetOutlineVisibility(false);
        }


        var corners = new Vector3[4];
        canvas.GetComponent<RectTransform>().GetWorldCorners(corners);

        canvasBotLeft = new Vector2(corners[0].x, corners[0].y);
        canvasTopRight = new Vector2(corners[2].x, corners[2].y);
        /*if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
         {
             canvasBotLeft = Camera.main.WorldToScreenPoint(corners[0]);
             canvasTopRight = Camera.main.WorldToScreenPoint(corners[2]);
         }
         else
         {
             canvasBotLeft = new Vector2(corners[0].x, corners[0].y);
             canvasTopRight = new Vector2(corners[2].x, corners[2].y);
         }*/

       

    }
    private void Awake()
    {
       
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void changeTarget() {
        if (target != null)
        {
            targetTransform = target.GetComponent<RectTransform>();
        }
    }

    public void changeInitialPos() { 
        initialPos = transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {



        if (!allowMove) return;

        isDragging = true;
        // Convert the pointer position to world space
        Vector3 worldPos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
            canvas.GetComponent<RectTransform>(), // Use the canvas RectTransform
            eventData.position,                   // Pointer position
            canvas.worldCamera,                   // Camera assigned to the Canvas
            out worldPos))
        {
            worldPos += innerOffset;
            // Add vertical offset to the calculated world position
            worldPos.y += offset;

            // Ensure the object stays within the canvas bounds
            float scaledWidth = childRect.rect.width * childRect.lossyScale.x;
            float scaledHeight = childRect.rect.height * childRect.lossyScale.y;

            float clampedX = Mathf.Clamp(worldPos.x, canvasBotLeft.x + scaledWidth / 2, canvasTopRight.x - scaledWidth / 2);
            float clampedY = Mathf.Clamp(worldPos.y, canvasBotLeft.y + scaledHeight / 2, canvasTopRight.y - scaledHeight / 2);

            // Move the object to the new position with animation
            rectTransform.DOMove(new Vector3(clampedX, clampedY, rectTransform.position.z), movementTime).SetEase(Ease.Linear);
        }
        else
        {
            Debug.LogWarning("Failed to convert ScreenPoint to WorldPoint.");
        }




    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(!allowMove) return;
        isDragging = false;
        Debug.Log("Finished dragging");

        //Remove the outline
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
        StopAll();




        if (targetTransform != null)
        {

            /*float deltax = Mathf.Abs(targetTransform.position.x - childTransform.position.x);
            float deltay = Mathf.Abs(targetTransform.position.y - childTransform.position.y);

            RectTransform targetRect = targetTransform.GetComponent<RectTransform>();
            float targetScaledWidth = targetRect.sizeDelta.x * targetRect.lossyScale.x;
            float targetScaledHeight = targetRect.sizeDelta.y * targetRect.lossyScale.y;

            if (deltax <= targetScaledWidth / 2 && deltay <= targetScaledHeight / 2)
            {
                transform.DOMove(targetTransform.position, moveToTargetTime);
                if (onTargetZoneDrop != null)
                {
                    onTargetZoneDrop.Invoke();

                }
                Debug.Log("Inside the target");
            }
            else
            {
                if (NotOnTargetZoneDrop != null)
                {
                    NotOnTargetZoneDrop.Invoke();
                }
                Debug.Log("Outside the target");
            }
            */
            //Check if dropped object is inside the target area
            if (IsInsideTarget())
            {   //Prevent dragging if PreventMovementAfterDropOnTarget is set as true
                if (PreventMovementAfterDropOnTarget)
                {
                    allowMove = false;
                }
                if (SnapToTargetCenter)
                {
                    transform.DOMove(targetTransform.position, moveToTargetTime);
                }

                if (onTargetZoneDrop != null)
                {
                    onTargetZoneDrop.Invoke();

                }
                Debug.Log("Inside the target");
            }
            else
            {
                if (ResetOnDropFail)
                {
                    Vector3 currentPos = transform.position;
                    float distance = Vector3.Distance(initialPos, currentPos);
                    rectTransform.DOMove(initialPos, distance / resetSpeed).SetEase(Ease.Linear);
                    allowMove = false;
                    StartCoroutine(Delay(distance/resetSpeed));

                }
                if (NotOnTargetZoneDrop != null)
                {
                    NotOnTargetZoneDrop.Invoke();
                }
                Debug.Log("Outside the target");
            }

        }
        else {

            if (ResetOnDropFail)
            {
                Vector3 currentPos = transform.position;
                float distance = Vector3.Distance(initialPos, currentPos);
                rectTransform.DOMove(initialPos, distance / resetSpeed).SetEase(Ease.Linear);
                allowMove = false;
                StartCoroutine(Delay(distance / resetSpeed));

            }
            if (NotOnTargetZoneDrop != null)
            {
                NotOnTargetZoneDrop.Invoke();
            }

        }


    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!allowMove) return;
        if (OnClickAudio != null) { 
            OnClickAudio.Play();
        }
        if (outline != null)
        {
            SetOutlineVisibility(true);
        }
        Debug.Log("Selected");

        Vector3 pointerWorldPos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventData.position, canvas.worldCamera, out pointerWorldPos);

        // Calculate innerOffset in world space
        innerOffset = rectTransform.position - pointerWorldPos;

        //innerOffset = new Vector2(rectTransform.position.x, rectTransform.position.y) - eventData.position;
        //Animation for scaling and adding offset to the draggable object when clicked on the object
        float scaledHeight = rectTransform.rect.height * rectTransform.lossyScale.y;
        float clampedY = Mathf.Clamp(rectTransform.position.y + offset, canvasBotLeft.y + scaledHeight / 2, canvasTopRight.y - scaledHeight / 2);
        scalingTween = DOTween.Sequence()
        .Append(transform.DOScale(originalScale * scaleUpOnClick, scaleUpTime))
        //.Join(transform.DOMoveY(clampedY, scaleUpTime)) 
        .Append(transform.DOScale(originalScale * scaleDownOnClick, scaleDownTime))
        .Play();

        if (showParticles) {
            PlayAll();
        }



    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //
        isDragging = false;
        if (outline != null)
        {
            SetOutlineVisibility(false);
        }
        if (scalingTween != null && scalingTween.IsActive())
        {
            scalingTween.Kill();
        }
        transform.DOScale(originalScale, scaleDownFinalTime);

        StopAll();





    }

    private void SetOutlineVisibility(bool isVisible)
    {
        // Adjust the color's alpha based on visibility
        Color color = outline.effectColor;
        color.a = isVisible ? 1f : 0f;
        outline.effectColor = color;
    }

    private bool IsInsideTarget()
    {
        if (target == null) return false;


        // Convert to world space
        Vector3[] dragCorners = new Vector3[4];
        Vector3[] targetCorners = new Vector3[4];

        childRect.GetWorldCorners(dragCorners);
        targetTransform.GetWorldCorners(targetCorners);

        // Compare world space corners
        return RectTransformUtility.RectangleContainsScreenPoint(targetTransform, childRect.position);
    }

    private void PlayAll()
    {
        foreach (ParticleSystem ps in particles)
        {
            if (ps != null)
            {
                ps.Play();
            }
        }
    }

    private void StopAll()
    {
        foreach (ParticleSystem ps in particles)
        {
            if (ps != null)
            {
                ps.Stop();
                ps.Clear();
            }
        }



    }

   


    private void SetSizeOfParticle() {

        Vector3[] localCorners = new Vector3[4];
        rectTransform.GetWorldCorners(localCorners);
        //float width = Vector3.Distance(localCorners[0], localCorners[3]);
        float width = localCorners[3].x - localCorners[0].x;
        //float height = Vector3.Distance(localCorners[0], localCorners[1]);
        float height = localCorners[1].y - localCorners[0].y;


        Debug.Log(width);
        Debug.Log(height);
        float radius = Mathf.Max(width,height) * 40.65f;
       

        foreach (ParticleSystem ps in particles)
        {
            if (ps != null)
            {
                
                var shape = ps.shape;
                shape.radius = radius;
                var mainModule = ps.main;
                mainModule.startSize = new ParticleSystem.MinMaxCurve(radius - 20, radius-10);
            }
        }

    }

    private IEnumerator Delay(float time)
    {
        yield return new WaitForSeconds(time);

        allowMove = true;
    }

}

