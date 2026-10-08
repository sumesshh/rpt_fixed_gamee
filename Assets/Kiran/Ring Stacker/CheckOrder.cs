using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class OrderChecker : MonoBehaviour
{
    [Header("Ring Settings")]
    public int ringNumber;
    public Transform lockPosition;
    public float moveToLockPositionTime = 0.5f;
    public float snapThreshold = 50f;

    [Header("Win Event Settings")]
    public float winEventDelay = 0.5f;
    public UnityEvent onPuzzleComplete;  // New UnityEvent for win condition

    [Header("Shared Settings - Keep same for all rings")]
    public static int currentCount = 4;

    private Vector3 initialPosition;
    private newDrag dragScript;
    private RectTransform rectTransform;
    private bool isLocked = false;
    private static bool hasWon = false;

    void Start()
    {
        initialPosition = transform.position;
        dragScript = GetComponent<newDrag>();
        rectTransform = GetComponent<RectTransform>();

        if (dragScript != null)
        {
            dragScript.onTargetZoneDrop.AddListener(CheckOrder);
            dragScript.NotOnTargetZoneDrop.AddListener(HandleIncorrectPlacement);
        }

        if (ringNumber == 4)
        {
            currentCount = 4;
            hasWon = false;
        }
    }

    public void CheckOrder()
    {
        if (isLocked) return;

        if (ringNumber == currentCount)
        {
            Debug.Log($"Ring {ringNumber} placed correctly!");
            LockRingInPlace();
            currentCount--;

            if (currentCount <= 0 && !hasWon)
            {
                hasWon = true;
                Invoke("TriggerWinEvent", winEventDelay);
            }
        }
        else
        {
            HandleIncorrectPlacement();
        }
    }

    private void LockRingInPlace()
    {
        isLocked = true;

        rectTransform.DOMove(lockPosition.position, moveToLockPositionTime)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => {
                dragScript.allowMove = false;
                dragScript.PreventMovementAfterDropOnTarget = true;
            });
    }

    private void TriggerWinEvent()
    {
        onPuzzleComplete?.Invoke();
    }

    public void HandleIncorrectPlacement()
    {
        if (isLocked) return;

        Debug.Log($"Wrong ring! Expected ring {currentCount}");
        ResetRing();
    }

    public void ResetRing()
    {
        if (isLocked) return;

        rectTransform.DOMove(initialPosition, dragScript.resetSpeed)
            .SetEase(Ease.OutQuad);
        dragScript.allowMove = true;
    }

    public static void ResetAllRings()
    {
        currentCount = 4;
        hasWon = false;

        GameObject[] rings = GameObject.FindGameObjectsWithTag("Ring");
        foreach (GameObject ring in rings)
        {
            OrderChecker checker = ring.GetComponent<OrderChecker>();
            if (checker != null)
            {
                checker.isLocked = false;
                checker.ResetRing();
            }
        }
    }

    public static bool IsPuzzleComplete()
    {
        return currentCount <= 0;
    }
}