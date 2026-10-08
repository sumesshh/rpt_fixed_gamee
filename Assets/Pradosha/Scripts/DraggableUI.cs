using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;

    void Awake()
    {
        // Get required components
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();

        // Add CanvasGroup if it doesn't exist
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // Store original position
        originalPosition = rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Reduce alpha to show dragging state (optional)
        canvasGroup.alpha = 0.6f;

        // Allow the UI element to be raycast through while dragging (optional)
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Update position based on mouse movement
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Restore original alpha
        canvasGroup.alpha = 1f;

        // Re-enable raycast blocking
        canvasGroup.blocksRaycasts = true;
    }

    // Optional: Method to reset position
    public void ResetPosition()
    {
        rectTransform.anchoredPosition = originalPosition;
    }
}