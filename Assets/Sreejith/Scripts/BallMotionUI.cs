using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BallMotionUI : MonoBehaviour, IPointerClickHandler
{
    [System.Serializable]
    public class Waypoint
    {
        public Vector2 position;
        public Vector2 scale;
        public Ease motionEase = Ease.OutQuad;
        public Ease scaleEase = Ease.OutQuad;
        public float segmentDuration = 1f;
    }

    [Header("References")]
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Image netImage;
    [SerializeField] private Canvas netCanvas;

    [Header("Motion Settings")]
    [SerializeField] private Waypoint[] waypoints;
    [SerializeField] private int netFinalSortingOrder = 2;

    // Events
    public System.Action onMotionStart;
    public System.Action onMotionComplete;

    // State
    private bool isMoving = false;
    private bool isPaused = false;
    private Sequence motionSequence;

    private void Awake()
    {
        InitializeComponents();
        ValidateSetup();
    }

    private void InitializeComponents()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
    }

    private void ValidateSetup()
    {
        if (netImage == null && netCanvas == null)
            Debug.LogWarning("Neither netImage nor netCanvas is assigned!");

        if (waypoints == null || waypoints.Length == 0)
            Debug.LogWarning("No waypoints defined!");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isMoving || waypoints == null || waypoints.Length == 0)
        {
            Debug.LogWarning("Cannot start motion: " +
                (isMoving ? "Already moving" : "No waypoints defined"));
            return;
        }

        StartMotionSequence();
    }

    private void StartMotionSequence()
    {
        isMoving = true;
        isPaused = false;
        onMotionStart?.Invoke();

        motionSequence = DOTween.Sequence();

        // Create the first waypoint tween separately
        if (waypoints.Length > 0)
        {
            CreateFirstWaypointTweens(waypoints[0]);

            // Change net sorting order after reaching first waypoint
            motionSequence.AppendCallback(() => UpdateNetOrder(netFinalSortingOrder));

            // Add remaining waypoints
            for (int i = 1; i < waypoints.Length; i++)
            {
                CreateWaypointTweens(waypoints[i]);
            }
        }

        motionSequence.OnComplete(() => {
            Debug.Log("Motion sequence completed");
            isMoving = false;
            isPaused = false;
            motionSequence = null;
            onMotionComplete?.Invoke();
        });
    }

    private void CreateFirstWaypointTweens(Waypoint waypoint)
    {
        Vector3 targetPosition = new Vector3(waypoint.position.x, waypoint.position.y, 0f);
        Vector3 targetScale = new Vector3(waypoint.scale.x, waypoint.scale.y, 1f);

        // Create and append position tween for first waypoint
        Tween positionTween = rectTransform
            .DOAnchorPos3D(targetPosition, waypoint.segmentDuration)
            .SetEase(waypoint.motionEase);
        motionSequence.Append(positionTween);

        // Create and join scale tween for first waypoint
        motionSequence.Join(
            rectTransform
                .DOScale(targetScale, waypoint.segmentDuration)
                .SetEase(waypoint.scaleEase)
        );
    }

    private void CreateWaypointTweens(Waypoint waypoint)
    {
        Vector3 targetPosition = new Vector3(waypoint.position.x, waypoint.position.y, 0f);
        Vector3 targetScale = new Vector3(waypoint.scale.x, waypoint.scale.y, 1f);

        // Create and append position tween
        Tween positionTween = rectTransform
            .DOAnchorPos3D(targetPosition, waypoint.segmentDuration)
            .SetEase(waypoint.motionEase);
        motionSequence.Append(positionTween);

        // Create and join scale tween
        motionSequence.Join(
            rectTransform
                .DOScale(targetScale, waypoint.segmentDuration)
                .SetEase(waypoint.scaleEase)
        );
    }

    private void UpdateNetOrder(int newOrder)
    {
        Debug.Log($"Attempting to update net order to {newOrder}");

        if (netImage != null)
        {
            Canvas parentCanvas = netImage.canvas;
            if (parentCanvas != null)
            {
                parentCanvas.overrideSorting = true;
                parentCanvas.sortingOrder = newOrder;
                Debug.Log($"Updated net image canvas sorting order to {newOrder}");
            }
            netImage.transform.SetSiblingIndex(newOrder);
            Debug.Log($"Updated net image sibling index to {newOrder}");
        }
        else if (netCanvas != null)
        {
            netCanvas.overrideSorting = true;
            netCanvas.sortingOrder = newOrder;
            Debug.Log($"Updated net canvas sorting order to {newOrder}");
        }
    }

    public void ResetPosition()
    {
        if (motionSequence != null)
        {
            motionSequence.Kill();
            motionSequence = null;
        }

        isMoving = false;
        isPaused = false;

        if (waypoints != null && waypoints.Length > 0)
        {
            rectTransform.anchoredPosition = waypoints[0].position;
            rectTransform.localScale = waypoints[0].scale;
        }
    }

    public void PauseMotion()
    {
        if (motionSequence != null && !isPaused)
        {
            motionSequence.Pause();
            isPaused = true;
            Debug.Log("Motion sequence paused");
        }
    }

    public void ResumeMotion()
    {
        if (motionSequence != null && isPaused)
        {
            motionSequence.Play();
            isPaused = false;
            Debug.Log("Motion sequence resumed");
        }
    }

    private void OnDisable()
    {
        if (motionSequence != null)
        {
            motionSequence.Kill();
            motionSequence = null;
        }
        isMoving = false;
        isPaused = false;
    }

    private void OnDestroy()
    {
        if (motionSequence != null)
        {
            motionSequence.Kill();
            motionSequence = null;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length == 0)
            return;

        Gizmos.color = Color.yellow;

        for (int i = 0; i < waypoints.Length; i++)
        {
            Vector3 position = transform.parent != null
                ? transform.parent.TransformPoint(new Vector3(waypoints[i].position.x, waypoints[i].position.y, 0))
                : new Vector3(waypoints[i].position.x, waypoints[i].position.y, 0);

            Gizmos.DrawWireSphere(position, 0.2f);

            if (i < waypoints.Length - 1)
            {
                Vector3 nextPosition = transform.parent != null
                    ? transform.parent.TransformPoint(new Vector3(waypoints[i + 1].position.x, waypoints[i + 1].position.y, 0))
                    : new Vector3(waypoints[i + 1].position.x, waypoints[i + 1].position.y, 0);

                Gizmos.DrawLine(position, nextPosition);
            }
        }
    }
#endif
}