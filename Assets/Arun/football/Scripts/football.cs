using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class Football : MonoBehaviour
{
    public RectTransform Ball;
    public Canvas canvas;
    public RectTransform targetArea;
    public Camera UICamera; // Reference to the camera used by the canvas

    private float duration = 1f;
    private bool canClick = true;
    private Vector3 originalScale;

    private void Start()
    {
        originalScale = Ball.localScale;

        // Ensure we have a reference to the camera
        if (UICamera == null && canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            UICamera = canvas.worldCamera;
        }
    }

    void Update()
    {
        if (canClick && Input.GetMouseButtonDown(0))
        {
            MoveBall();
            canClick = false;
        }
    }

    void MoveBall()
    {
        // Convert mouse position to world space
        Vector2 mousePos = Input.mousePosition;
        Vector3 worldPoint;

        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            mousePos,
            UICamera,
            out worldPoint))
        {
            // Calculate move direction in world space
            Vector3 moveDirection = (worldPoint - Ball.position).normalized;

            // Check if click is within target area
            if (RectTransformUtility.RectangleContainsScreenPoint(
                targetArea,
                mousePos,
                UICamera))
            {
                // Move to exact click position if inside target area
                Ball.DOMove(worldPoint, duration).SetEase(Ease.OutSine)
                    .OnComplete(() => canClick = false);
                Debug.Log("Inside");
            }
            else
            {
                // Move in direction of click if outside target area
                //Vector3 outsidePosition = Ball.position + (moveDirection);
                //Ball.DOMove(outsidePosition, duration).SetEase(Ease.OutSine)
                Ball.DOMove(worldPoint, duration).SetEase(Ease.OutSine)
                    .OnComplete(() => canClick = false);
                Debug.Log("Outside");
            }

            // Apply rotation and scale animations
            Ball.DOLocalRotate(new Vector3(0, 0, 360), duration, RotateMode.FastBeyond360);
            Ball.DOScale(originalScale * 0.9f, duration)
                .OnComplete(() => Ball.DOScale(originalScale, duration * 0.5f));
        }
    }
}