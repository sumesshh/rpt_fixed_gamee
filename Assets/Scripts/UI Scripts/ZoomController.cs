using UnityEngine;

public class ZoomController : MonoBehaviour
{
    [SerializeField] private RectTransform roomImage;  // The big image with cat, tables, etc.
    [SerializeField] private RectTransform targetItem; // The cat
    [SerializeField] private RectTransform viewport;   // The area (Canvas panel) we look through
    [SerializeField] private float zoomScale = 2f;
    [SerializeField] private float zoomDuration = 0.5f;

    // Camera/room zoom has been disabled: gameplay must stay inside the fixed
    // camera frame at all times. The arrow is the only element allowed to guide
    // the player between steps, so ZoomIn/ZoomOut are intentionally no-ops.

    public void ZoomIn()
    {
    }

    public void ZoomOut()
    {
    }
}
