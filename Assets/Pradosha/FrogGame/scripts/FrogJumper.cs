using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FrogJumper : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Image targetTwo; // Reference to the target leaf image
    private RectTransform frogRectTransform;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private int currentPrefabIndex = 0; // Track which prefab we're on

    void Start()
    {
        frogRectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (isMoving)
        {
            frogRectTransform.position = Vector3.Lerp(
                frogRectTransform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            if (Vector3.Distance(frogRectTransform.position, targetPosition) < 0.1f)
            {
                isMoving = false;
                // After reaching position, check if we need to prepare for next prefab
                PrepareForNextPrefab();
            }
        }
    }

    public void OnPrefabMatch(RectTransform targetZone, Image targetLeaf, Image nextLeaf)
    {
        if (targetLeaf == null || nextLeaf == null || targetTwo == null)
        {
            Debug.LogError("Required references not assigned!");
            return;
        }

        // Make targetTwo leaf disappear
        targetTwo.gameObject.SetActive(false);

        // Make the target leaf disappear
        targetLeaf.gameObject.SetActive(false);

        // Lock the prefab in the target zone
        LockPrefabInTargetZone(targetZone);

        // Move frog to the target zone position immediately
        JumpToPosition(targetZone.position);

        // Increment the prefab counter
        currentPrefabIndex++;
    }

    private void JumpToPosition(Vector3 newPosition)
    {
        targetPosition = newPosition;
        isMoving = true;
    }

    private void LockPrefabInTargetZone(RectTransform targetZone)
    {
        Debug.Log($"Prefab {currentPrefabIndex} locked in target zone: {targetZone.name}");
        // Add any additional locking logic here
    }

    private void PrepareForNextPrefab()
    {
        // Add any setup needed for the next prefab
        Debug.Log($"Ready for prefab {currentPrefabIndex + 1}");
    }

    // Method to reset the game state if needed
    public void ResetGame()
    {
        currentPrefabIndex = 0;
        if (targetTwo != null)
        {
            targetTwo.gameObject.SetActive(true);
        }
        // Add any additional reset logic here
    }
}