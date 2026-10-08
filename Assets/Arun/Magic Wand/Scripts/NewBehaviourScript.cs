using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

public class S : MonoBehaviour
{
    [Header("Erasing Settings")]
    public Image targetImage; // UI Image to erase
    public float brushSize = 50f; // Brush size normalized for screen scaling
    public float eraseStrength = 0.2f; // Alpha reduction per stroke
    public Camera uiCamera; // Camera rendering the UI

    [Header("Wand Shake Settings")]
    public Image smallWandImage;
    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 8f;

    private RectTransform smallWandRectTransform;
    private Vector3 originalWandPosition;

    [Header("Completion Settings")]
    [Range(0.0f, 1.0f)]
    public float completionThreshold = 0.95f;
    public UnityEvent onCompletionEvent;
    public GameObject particleEffectPrefab;
    public GameObject winPopupPrefab;

    private Texture2D editableTexture;
    private RectTransform imageRectTransform;
    private int totalPixels;
    private int erasedPixels;
    private bool completionEventTriggered = false;

    private void Start()
    {
        if (!targetImage || !uiCamera)
        {
            Debug.LogError("Target Image or UI Camera is not assigned!");
            return;
        }

        // Clone the texture
        CreateEditableTexture();

        // Initialize small wand shake
        if (smallWandImage)
        {
            smallWandRectTransform = smallWandImage.rectTransform;
            originalWandPosition = smallWandRectTransform.anchoredPosition;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) || Input.touchCount > 0)
        {
            EraseUnderCursor();
        }
    }

    private void CreateEditableTexture()
    {
        Sprite sprite = targetImage.sprite;
        if (!sprite)
        {
            Debug.LogError("Target Image has no sprite!");
            return;
        }

        // Duplicate texture for editing
        editableTexture = new Texture2D(sprite.texture.width, sprite.texture.height, TextureFormat.RGBA32, false);
        Graphics.CopyTexture(sprite.texture, editableTexture);
        editableTexture.Apply();

        // Assign new texture to UI image
        targetImage.sprite = Sprite.Create(editableTexture, sprite.rect, new Vector2(0.5f, 0.5f));
        imageRectTransform = targetImage.rectTransform;

        // Calculate the total number of visible pixels
        totalPixels = 0;
        for (int x = 0; x < editableTexture.width; x++)
        {
            for (int y = 0; y < editableTexture.height; y++)
            {
                if (editableTexture.GetPixel(x, y).a > 0.1f)
                {
                    totalPixels++;
                }
            }
        }

        erasedPixels = 0;
        completionEventTriggered = false;
    }

    private void EraseUnderCursor()
    {
        Vector2 localPoint;
        Vector2 screenPos = Input.touchCount > 0 ? (Vector2)Input.GetTouch(0).position : (Vector2)Input.mousePosition;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(imageRectTransform, screenPos, uiCamera, out localPoint))
            return;

        // Convert local position to texture coordinates
        Rect imageRect = imageRectTransform.rect;
        float uvX = Mathf.InverseLerp(imageRect.xMin, imageRect.xMax, localPoint.x);
        float uvY = Mathf.InverseLerp(imageRect.yMin, imageRect.yMax, localPoint.y);
        int texX = Mathf.RoundToInt(uvX * editableTexture.width);
        int texY = Mathf.RoundToInt(uvY * editableTexture.height);

        // Apply erasing effect
        erasedPixels += EraseCircle(texX, texY);
        editableTexture.Apply();

        // Check completion status
        CheckCompletionStatus();

        // Check if the eraser is near the small wand
        CheckWandProximity(localPoint, screenPos);
    }

    private int EraseCircle(int centerX, int centerY)
    {
        int radius = Mathf.RoundToInt(brushSize);
        int width = editableTexture.width;
        int height = editableTexture.height;
        int newlyErasedPixels = 0;

        Color[] pixels = editableTexture.GetPixels();
        int texWidth = editableTexture.width;

        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                int px = centerX + x;
                int py = centerY + y;

                if (px >= 0 && px < width && py >= 0 && py < height)
                {
                    float distance = Mathf.Sqrt(x * x + y * y);
                    if (distance <= radius)
                    {
                        int index = py * texWidth + px;
                        Color pixel = pixels[index];

                        if (pixel.a > 0.1f)
                        {
                            pixel.a -= eraseStrength;
                            pixel.a = Mathf.Clamp01(pixel.a);
                            pixels[index] = pixel;

                            if (pixel.a <= 0.1f)
                                newlyErasedPixels++;
                        }
                    }
                }
            }
        }

        editableTexture.SetPixels(pixels);
        return newlyErasedPixels;
    }

    private void CheckCompletionStatus()
    {
        if (completionEventTriggered) return;

        float completionPercentage = (float)erasedPixels / totalPixels;
        if (completionPercentage >= completionThreshold)
        {
            CompleteErasing();
        }
    }

    private void CompleteErasing()
    {
        completionEventTriggered = true;

        // Fully erase the image
        Color[] pixels = new Color[editableTexture.width * editableTexture.height];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = new Color(0, 0, 0, 0);

        editableTexture.SetPixels(pixels);
        editableTexture.Apply();

        StartCoroutine(ShowCompletionEffects());
    }

    private IEnumerator ShowCompletionEffects()
    {
        onCompletionEvent.Invoke();

        if (particleEffectPrefab)
        {
            Vector3 spawnPosition = imageRectTransform.position;
            GameObject particleInstance = Instantiate(particleEffectPrefab, spawnPosition, Quaternion.identity);
            ParticleSystem ps = particleInstance.GetComponent<ParticleSystem>();
            ps?.Play();
        }

        yield return new WaitForSeconds(1.0f);

        if (winPopupPrefab)
        {
            GameObject winPopup = Instantiate(winPopupPrefab, transform.parent);
            RectTransform popupRect = winPopup.GetComponent<RectTransform>();
            if (popupRect)
                popupRect.anchoredPosition = Vector2.zero;
        }
    }

    private void CheckWandProximity(Vector2 eraserLocalPos, Vector2 eraserScreenPos)
    {
        if (smallWandImage && smallWandRectTransform)
        {
            // Convert wand's world position to screen position
            Vector2 wandScreenPos = uiCamera.WorldToScreenPoint(smallWandRectTransform.position);

            // Calculate distance between eraser and wand in screen space
            float screenDistance = Vector2.Distance(eraserScreenPos, wandScreenPos);

            // Convert brush size to screen space
            Vector2 screenPosWithOffset = new Vector2(eraserScreenPos.x + brushSize, eraserScreenPos.y);
            Vector2 localPosWithOffset;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                imageRectTransform,
                screenPosWithOffset,
                uiCamera,
                out localPosWithOffset
            );

            float screenBrushSize = Vector2.Distance(eraserLocalPos, localPosWithOffset);

            if (screenDistance < screenBrushSize * 1.5f)
            {
                ShakeSmallWand();
                return; // Early return if we trigger shake based on screen distance
            }

            // Backup method: Calculate based on local positions
            Vector2 wandLocalPos;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                imageRectTransform,
                wandScreenPos,
                uiCamera,
                out wandLocalPos))
            {
                float localDistance = Vector2.Distance(eraserLocalPos, wandLocalPos);
                float scaledBrushSize = brushSize * imageRectTransform.lossyScale.x;

                if (localDistance < scaledBrushSize * 1.5f)
                {
                    ShakeSmallWand();
                }
            }
        }
    }

    public void ShakeSmallWand()
    {
        if (smallWandRectTransform == null) return;

        if (!DOTween.IsTweening(smallWandRectTransform))
        {
            smallWandRectTransform.DOShakeAnchorPos(
                shakeDuration,
                shakeMagnitude,
                10,
                90,
                false,
                true
            )
            .SetEase(Ease.OutElastic)
            .OnComplete(() => smallWandRectTransform.DOAnchorPos(originalWandPosition, 0.2f));
        }
    }

    public void ResetTexture()
    {
        CreateEditableTexture();
    }

    public void TriggerCompletionManually()
    {
        if (!completionEventTriggered)
        {
            CompleteErasing();
        }
    }
}