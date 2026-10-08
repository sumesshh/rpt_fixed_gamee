using UnityEngine;
using DG.Tweening;

public class ArrowPointer : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform targetObject;

    [Header("Movement Settings")]
    [SerializeField] private float moveDuration = 1.0f;
    [SerializeField] private float distanceToTravel = 1.0f;
    [SerializeField] private Ease movementEase = Ease.Linear;

    [Header("Loop Settings")]
    [SerializeField] private int loopCount = -1; // -1 for infinite loops
    [SerializeField] private LoopType loopType = LoopType.Yoyo;

    private Tween movementTween;

    private void Start()
    {
        if (targetObject == null)
        {
            Debug.LogWarning("No target object assigned to SimpleArrowLinearMovement script!");
            return;
        }

        StartMovement();
    }

    private void StartMovement()
    {
        // Kill any existing tween
        if (movementTween != null && movementTween.IsActive())
        {
            movementTween.Kill();
        }

        // Calculate the direction to the target
        Vector3 directionToTarget = (targetObject.position - transform.position).normalized;

        // Calculate the target position
        Vector3 targetPosition = transform.position + directionToTarget * distanceToTravel;

        // Create the linear movement tween
        movementTween = transform.DOMove(targetPosition, moveDuration)
            .SetEase(movementEase)
            .SetLoops(loopCount, loopType);
    }

    // Call this method if the target changes position and you want to recalculate
    public void UpdateMovement()
    {
        StartMovement();
    }

    // Public method to set a new target at runtime
    public void SetTarget(Transform newTarget)
    {
        targetObject = newTarget;
        StartMovement();
    }

    // Public method to set loop count at runtime
    public void SetLoopCount(int newLoopCount)
    {
        loopCount = newLoopCount;
        StartMovement();
    }

    // Public method to set move duration at runtime
    public void SetMoveDuration(float newDuration)
    {
        moveDuration = newDuration;
        StartMovement();
    }

    private void OnDestroy()
    {
        // Clean up tween when the object is destroyed
        if (movementTween != null && movementTween.IsActive())
        {
            movementTween.Kill();
        }
    }
}