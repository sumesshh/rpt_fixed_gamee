using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Events;

public class anchored : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler, IPointerUpHandler
{
    [Header("Target Settings")]
    public RectTransform target;
    public Canvas canvas;

    [Header("Drag Behavior")]
    public float dragOffset = 120f;
    public float scaleUpFactor = 1.5f;
    public float scaleDuration = 0.5f;

    [Header("Events")]
    public UnityEvent onTargetZoneDrop;
    public UnityEvent onNotTargetZoneDrop;

    private RectTransform rectTransform;
    private Vector2 originalScale;
    private Vector2 dragStartPosition;
    private Outline outline;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;

        // Get outline component if exists
        outline = GetComponentInChildren<Outline>();

        // Ensure anchors are set correctly
        // Recommend setting anchors to center or as needed for your specific layout
        //rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        //rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Store the initial position for potential reset
        dragStartPosition = rectTransform.position;

        // Scale up effect
        rectTransform.DOScale(originalScale * scaleUpFactor, scaleDuration);

        // Show outline if exists
        if (outline != null)
        {
            SetOutlineVisibility(true);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Use RectTransformUtility to convert screen point to local point
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            eventData.position,
            canvas.worldCamera,
            out localPoint
        );

        // Convert local point back to world position with offset
        Vector3 worldPos = canvas.transform.TransformPoint(localPoint);
        worldPos.y += dragOffset;

        // Move the RectTransform
        rectTransform.position = worldPos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Scale back to original size
        rectTransform.DOScale(originalScale, scaleDuration);

        // Hide outline
        if (outline != null)
        {
            SetOutlineVisibility(false);
        }

        // Check if inside target zone
        if (IsInsideTarget())
        {
            // Snap to target
            rectTransform.position = target.position;
            onTargetZoneDrop?.Invoke();
        }
        else
        {
            onNotTargetZoneDrop?.Invoke();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Reset scale and outline
        rectTransform.DOScale(originalScale, scaleDuration);

        if (outline != null)
        {
            SetOutlineVisibility(false);
        }
    }

    private bool IsInsideTarget()
    {
        if (target == null) return false;

        // Get the rect of both the dragged object and the target
        Rect dragRect = rectTransform.rect;
        Rect targetRect = target.rect;

        // Convert to world space
        Vector3[] dragCorners = new Vector3[4];
        Vector3[] targetCorners = new Vector3[4];

        rectTransform.GetWorldCorners(dragCorners);
        target.GetWorldCorners(targetCorners);

        // Compare world space corners
        return RectTransformUtility.RectangleContainsScreenPoint(target, rectTransform.position);
    }

    private void SetOutlineVisibility(bool isVisible)
    {
        if (outline == null) return;

        Color color = outline.effectColor;
        color.a = isVisible ? 1f : 0f;
        outline.effectColor = color;
    }
}
