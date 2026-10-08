using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.Events;

public class IceCreamDrag : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler
{
    public List<Transform> targetPositions; // List of target positions
    public Canvas canvas;
    public bool SnapToTargetCenter = true;
    public UnityEvent onTargetZoneDrop;
    public UnityEvent NotOnTargetZoneDrop;
    public PopperManagerNew popperManager;
    public GameObject winningEffectPrefab;
    public Transform winningEffectSpawnPoint;

    private Vector2 originalScale;
    private RectTransform rectTransform;
    private Vector3 initialPos;
    private bool allowMove = true;
    private static int scoopsPlaced = 0;
    private static HashSet<int> occupiedTargets = new HashSet<int>();
    private int assignedTargetIndex = -1;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = transform.localScale;
        initialPos = transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!allowMove) return;
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            eventData.position,
            canvas.worldCamera,
            out pos);
        transform.position = canvas.transform.TransformPoint(pos);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (assignedTargetIndex == -1)
        {
            int targetIndex = GetNextAvailableTargetIndex();
            if (targetIndex != -1)
            {
                assignedTargetIndex = targetIndex;
                occupiedTargets.Add(targetIndex);
                scoopsPlaced++;

                // Set the sibling index based on which target position was filled
                SetLayerOrder(targetIndex);

                if (SnapToTargetCenter)
                {
                    rectTransform.DOMove(targetPositions[targetIndex].position, 0.5f);
                }
                onTargetZoneDrop?.Invoke();
                allowMove = false;

                if (scoopsPlaced == targetPositions.Count)
                {
                    PlayWinningEffect();
                }
            }
            else
            {
                rectTransform.DOMove(initialPos, 0.5f);
                NotOnTargetZoneDrop?.Invoke();
            }
        }
    }

    private void SetLayerOrder(int targetIndex)
    {
        // Set sibling index to control rendering order
        // Higher sibling index = rendered on top
        switch (targetIndex)
        {
            case 0: // Bottom scoop
                transform.SetSiblingIndex(1);
                break;
            case 1: // Middle scoop
                transform.SetSiblingIndex(2);
                break;
            case 2: // Top scoop
                transform.SetSiblingIndex(3);
                break;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!allowMove) return;
        // Bring the scoop to front while dragging
        transform.SetAsLastSibling();
    }

    private int GetNextAvailableTargetIndex()
    {
        for (int i = 0; i < targetPositions.Count; i++)
        {
            if (!occupiedTargets.Contains(i))
            {
                return i;
            }
        }
        return -1;
    }

    private void PlayWinningEffect()
    {
        if (winningEffectPrefab != null)
        {
            Vector3 spawnPosition = winningEffectSpawnPoint != null
                ? winningEffectSpawnPoint.position
                : targetPositions[targetPositions.Count - 1].position;
            GameObject effect = Instantiate(winningEffectPrefab, spawnPosition, Quaternion.identity);
            Destroy(effect, 3f);
        }
        popperManager.PopperBurst();
    }
}