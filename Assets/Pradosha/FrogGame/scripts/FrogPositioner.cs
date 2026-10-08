using UnityEngine;
using UnityEngine.UI;

public class FrogPositioner : MonoBehaviour
{
    [SerializeField] private Image targetLeaf;
    [SerializeField] private float moveSpeed = 5f; // Control how fast the frog moves
    private RectTransform frogRectTransform;
    private Vector3 startPosition;
    private bool isMoving = true;

    void Start()
    {
        frogRectTransform = GetComponent<RectTransform>();
        startPosition = frogRectTransform.position;
    }

    void Update()
    {
        if (isMoving && targetLeaf != null)
        {
            // Smoothly interpolate between start position and target position
            frogRectTransform.position = Vector3.Lerp(
                frogRectTransform.position,
                targetLeaf.rectTransform.position,
                moveSpeed * Time.deltaTime
            );

            // Stop moving when very close to target
            if (Vector3.Distance(frogRectTransform.position, targetLeaf.rectTransform.position) < 0.1f)
            {
                isMoving = false;
            }
        }
    }
}