using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class outlineDrawer : MonoBehaviour, IPointerDownHandler
{

    [Header("Outline Settings")]
    [SerializeField] private float outlineWidth = 2f;
    [SerializeField] private Color outlineColor = Color.yellow;
    [SerializeField] private float animationDuration = 1f;
    [SerializeField] private int samplePoints = 100; // Number of points to sample around the image

    private Image targetImage;
    private LineRenderer outlineRenderer;
    private List<Vector2> contourPoints = new List<Vector2>();
    private float currentProgress = 0f;

    private void Awake()
    {
        targetImage = GetComponent<Image>();
        SetupLineRenderer();
        GenerateContourPoints();
    }

    private void SetupLineRenderer()
    {
        GameObject lineObj = new GameObject("ImageOutline");
        lineObj.transform.SetParent(transform);
        lineObj.transform.localPosition = Vector3.zero;

        outlineRenderer = lineObj.AddComponent<LineRenderer>();
        outlineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        outlineRenderer.startColor = outlineRenderer.endColor = outlineColor;
        outlineRenderer.widthMultiplier = outlineWidth;
        outlineRenderer.useWorldSpace = false;
        outlineRenderer.sortingOrder = targetImage.canvas.sortingOrder + 1;
        outlineRenderer.enabled = false;
    }

    private void GenerateContourPoints()
    {
        contourPoints.Clear();
        Sprite sprite = targetImage.sprite;

        // Get the alpha texture of the sprite
        Texture2D texture = GetSpriteTexture(sprite);

        // Sample points around the edge of the non-transparent areas
        float angleStep = 360f / samplePoints;
        Vector2 center = new Vector2(texture.width / 2f, texture.height / 2f);

        for (float angle = 0; angle < 360f; angle += angleStep)
        {
            float radian = angle * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
            Vector2 point = FindEdgePoint(texture, center, direction);

            // Convert texture space to UV space
            point.x /= texture.width;
            point.y /= texture.height;

            // Convert UV to local position
            point = new Vector2(
                (point.x - 0.5f) * targetImage.rectTransform.rect.width,
                (point.y - 0.5f) * targetImage.rectTransform.rect.height
            );

            contourPoints.Add(point);
        }

        // Close the loop
        contourPoints.Add(contourPoints[0]);
    }

    private Texture2D GetSpriteTexture(Sprite sprite)
    {
        // Create a temporary texture from the sprite
        Texture2D texture = new Texture2D(
            (int)sprite.rect.width,
            (int)sprite.rect.height,
            TextureFormat.RGBA32,
            false);

        var pixels = sprite.texture.GetPixels(
            (int)sprite.rect.x,
            (int)sprite.rect.y,
            (int)sprite.rect.width,
            (int)sprite.rect.height);

        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }

    private Vector2 FindEdgePoint(Texture2D texture, Vector2 start, Vector2 direction)
    {
        float maxDistance = Mathf.Max(texture.width, texture.height);
        Vector2 currentPoint = start;

        for (float dist = 0; dist < maxDistance; dist += 1f)
        {
            currentPoint = start + direction * dist;

            int x = Mathf.RoundToInt(currentPoint.x);
            int y = Mathf.RoundToInt(currentPoint.y);

            // Check bounds
            if (x < 0 || x >= texture.width || y < 0 || y >= texture.height)
                break;

            // Found edge if pixel is transparent
            if (texture.GetPixel(x, y).a < 0.1f)
                return currentPoint - direction; // Return the last non-transparent point
        }

        return start;
    }

    public void ShowOutline()
    {
        outlineRenderer.enabled = true;
        outlineRenderer.positionCount = contourPoints.Count;
        currentProgress = 0f;

        // Reset all points to the starting position
        Vector3[] positions = new Vector3[contourPoints.Count];
        for (int i = 0; i < positions.Length; i++)
        {
            positions[i] = new Vector3(contourPoints[0].x, contourPoints[0].y, 0);
        }
        outlineRenderer.SetPositions(positions);

        // Animate the drawing of the outline
        DOTween.To(() => currentProgress, x => {
            currentProgress = x;
            UpdateOutline();
        }, 1f, animationDuration)
        .SetEase(Ease.InOutSine);
    }

    private void UpdateOutline()
    {
        int totalPoints = contourPoints.Count;
        int visiblePoints = Mathf.CeilToInt(totalPoints * currentProgress);
        Vector3[] positions = new Vector3[totalPoints];

        for (int i = 0; i < totalPoints; i++)
        {
            if (i <= visiblePoints)
            {
                positions[i] = new Vector3(contourPoints[i].x, contourPoints[i].y, 0);
            }
            else
            {
                positions[i] = positions[visiblePoints];
            }
        }

        outlineRenderer.SetPositions(positions);
    }

    public void HideOutline()
    {
        DOTween.To(() => currentProgress, x => {
            currentProgress = x;
            UpdateOutline();
        }, 0f, animationDuration / 2)
        .SetEase(Ease.InOutSine)
        .OnComplete(() => outlineRenderer.enabled = false);
    }

    private void OnDestroy()
    {
        if (outlineRenderer != null && outlineRenderer.material != null)
        {
            Destroy(outlineRenderer.material);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ShowOutline();
    }
}
