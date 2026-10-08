using UnityEngine;
using DG.Tweening;
using UnityEngine.Events; // Add this if using Unity Events

public class BoyMovement : MonoBehaviour
{
    [SerializeField] private Transform boyTransform;
    [SerializeField] private float moveDistance = 10f;
    [SerializeField] private float moveDuration = 5f;
    [SerializeField] private Ease moveEaseType = Ease.Linear;
    [SerializeField] private bool autoPlay = false;

    // Event that will be triggered when the boy reaches the end
    public UnityEvent onBridgeCrossed;

    private Tween moveTween;

    private void Start()
    {
        if (autoPlay)
        {
            MoveAcrossBridge();
        }
    }

    // This version will appear in Unity Events
    public void MoveAcrossBridge()
    {
        // We'll call the internal method that returns the Tween
        InternalMoveAcrossBridge(moveDuration);
    }

    // This overload can be used from code when you need to specify a custom duration
    public void MoveAcrossBridge(float duration)
    {
        InternalMoveAcrossBridge(duration);
    }

    // Internal method that actually performs the animation and returns the Tween
    private Tween InternalMoveAcrossBridge(float duration)
    {
        // If already moving, kill current tween
        if (moveTween != null && moveTween.IsActive())
        {
            moveTween.Kill();
        }

        // Calculate target position (maintain same Y and Z)
        Vector3 targetPosition = boyTransform.position + new Vector3(moveDistance, 0, 0);

        // Create the tween
        moveTween = boyTransform.DOMove(targetPosition, duration)
            .SetEase(moveEaseType)
            .OnComplete(() => {
                Debug.Log("Boy reached the end of the bridge!");
                onBridgeCrossed?.Invoke();
            });

        return moveTween;
    }

    public void StopMovement()
    {
        if (moveTween != null && moveTween.IsActive())
        {
            moveTween.Pause();
        }
    }
}