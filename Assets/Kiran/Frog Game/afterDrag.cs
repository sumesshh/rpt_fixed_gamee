using UnityEngine;
using DG.Tweening;

public class DoTweenMotion : MonoBehaviour
{
    [SerializeField] private RectTransform[] pathPoints; // Assign RectTransform waypoints in Inspector
    [SerializeField] private float duration = 2f; // Movement duration
    public GameObject nextDrag; private newDrag DragScript;
    public Transform nextTarget;

    public bool hasNext;

    public void PlayAnimationAndAssignTarget()
    {

        RectTransform rectTransform = GetComponent<RectTransform>(); // Get own RectTransform

        Sequence sequence = DOTween.Sequence();

        foreach (RectTransform point in pathPoints)
        {
            sequence.Append(rectTransform.DOAnchorPos(point.anchoredPosition, duration / pathPoints.Length)
                .SetEase(Ease.InOutSine));
        }
        if (hasNext)
        {
              
              DragScript = nextDrag.GetComponent<newDrag>();
              DragScript.target = nextTarget;
        }
    }
}
