using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;


public class Sample : MonoBehaviour
{
    public Image targetImage; // The UI Image to erase
    public float brushSize = 10f; // Brush radius in pixels
    public float eraseStrength = 0.1f; // How much alpha is reduced per stroke
    public Camera uiCamera; // Reference to the camera rendering the UI

    //public RectTransform treeTransform;
    //public Button smallWANDButton;
    //public float shakeDuration = 1f; // Increase this to slow the shake duration
    //public float shakeMagnitude = 10f; // Adjust to control side-to-side shake range
    //public float shakeSpeed = 0.05f; // Delay between each shake (slows down the movement)

    public Image smallWandImage;  // Assign the thinner wand image in the Inspector
    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 8f;

    private RectTransform smallWandRectTransform;
    private Vector3 originalPosition;


    [Header("Completion Settings")]
    [Range(0.0f, 1.0f)]
    public float completionThreshold = 0.95f; // 95% completion threshold
    public UnityEvent onCompletionEvent; // Event to trigger when erasing is complete
    public GameObject particleEffectPrefab; // Particle effect to spawn
    public GameObject winPopupPrefab; // Win popup to show
   // private Vector3 originalPosition;

    private Texture2D editableTexture;
    private RectTransform imageRectTransform;
    private int totalPixels;
    private int erasedPixels;
    private bool completionEventTriggered = false;

    //public void StartShaking()
    //{
    //    Debug.Log("Tree clicked! Shaking started...");
    //    StartCoroutine(ShakeTree());
    //}
    //IEnumerator ShakeTree()
    //{
    //    float elapsedTime = 0f;
    //    while (elapsedTime < shakeDuration)
    //    {
    //        float offsetX = Random.Range(-shakeMagnitude, shakeMagnitude); // Only shaking on X-axis

    //        // Set the Y-position fixed, only change X-position for side-to-side shake
    //        treeTransform.anchoredPosition = new Vector3(originalPosition.x + offsetX, originalPosition.y, originalPosition.z);

    //        elapsedTime += Time.deltaTime;

    //        // Delay between each shake move to slow it down
    //        yield return new WaitForSeconds(shakeSpeed);
    //    }

    //    treeTransform.anchoredPosition = originalPosition; // Reset position
    //    Debug.Log("Shaking Ended!");
     
    //}
    private void Start()
    {
        if (targetImage == null)
        {
            Debug.LogError("No target Image assigned!");
            return;
        }

        if (uiCamera == null)
        {
            Debug.LogError("No UI Camera assigned!");
            return;
        }

        // Duplicate the sprite texture to allow pixel modification
        Sprite sprite = targetImage.sprite;
        if (sprite == null)
        {
            Debug.LogError("Target Image has no sprite!");
            return;
        }

        // Create a copy of the texture to edit
        editableTexture = new Texture2D(sprite.texture.width, sprite.texture.height, TextureFormat.RGBA32, false);
        Graphics.CopyTexture(sprite.texture, editableTexture);
        editableTexture.Apply();

        // Assign new texture to the UI
        targetImage.sprite = Sprite.Create(editableTexture, sprite.rect, new Vector2(0.5f, 0.5f));
        imageRectTransform = targetImage.rectTransform;

        // Calculate total pixels with alpha > 0
        totalPixels = 0;
        for (int x = 0; x < editableTexture.width; x++)
        {
            for (int y = 0; y < editableTexture.height; y++)
            {
                if (editableTexture.GetPixel(x, y).a > 0.1f) // Consider pixels with alpha > 0.1 as visible
                {
                    totalPixels++;
                }
            }
        }

        erasedPixels = 0;
        if (smallWandImage == null)
        {
            Debug.LogError("Small wand image is not assigned!");
            return;
        }

        smallWandRectTransform = smallWandImage.rectTransform;
        originalPosition = smallWandRectTransform.anchoredPosition;

        //if (treeTransform == null)
        //    treeTransform = GetComponent<RectTransform>(); // Get UI transform if not set

        //if (smallWANDButton != null)
        //    smallWANDButton.onClick.AddListener(StartShaking);

        //originalPosition = treeTransform.anchoredPosition;
    }



  

    public void ShakeSmallWand()
    {
        if (DOTween.IsTweening(smallWandRectTransform))
            return;

        Debug.Log("Shaking the small wand...");

        smallWandRectTransform.DOShakeAnchorPos(shakeDuration, shakeMagnitude, 10, 90, false, true)
            .SetEase(Ease.OutElastic)
            .OnComplete(() => smallWandRectTransform.DOAnchorPos(originalPosition, 0.2f));
    }



private void Update()
    {
        if (Input.GetMouseButton(0)) // Left mouse or touch
        {
            EraseUnderCursor();
        }
    }

    private void EraseUnderCursor()
    {
        Vector2 localPoint;

        // Use the UI camera for screen point to local point conversion
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            imageRectTransform,
            Input.mousePosition,
            uiCamera, // Specify the UI camera here
            out localPoint
        );

        // Convert local point to texture coordinates
        Rect imageRect = imageRectTransform.rect;
        float uvX = Mathf.InverseLerp(imageRect.xMin, imageRect.xMax, localPoint.x);
        float uvY = Mathf.InverseLerp(imageRect.yMin, imageRect.yMax, localPoint.y);
        int texX = Mathf.RoundToInt(uvX * editableTexture.width);
        int texY = Mathf.RoundToInt(uvY * editableTexture.height);

        // Apply erasing effect
        int newlyErasedPixels = EraseCircle(texX, texY);
        erasedPixels += newlyErasedPixels;

        // Apply changes to texture
        editableTexture.Apply();

        // Check if we've reached the completion threshold
        CheckCompletionStatus();
    }

    private int EraseCircle(int centerX, int centerY)
    {
        int radius = Mathf.RoundToInt(brushSize);
        int width = editableTexture.width;
        int height = editableTexture.height;
        int newlyErasedPixels = 0;

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
                        Color pixel = editableTexture.GetPixel(px, py);

                        // Check if this pixel is being newly erased (alpha going from visible to invisible)
                        bool wasVisible = pixel.a > 0.1f;

                        pixel.a -= eraseStrength; // Reduce alpha
                        pixel.a = Mathf.Clamp01(pixel.a);
                        editableTexture.SetPixel(px, py, pixel);

                        // If the pixel was visible but is now invisible, count it as newly erased
                        if (wasVisible && pixel.a <= 0.1f)
                        {
                            newlyErasedPixels++;
                        }
                    }
                }
            }
        }

        return newlyErasedPixels;
    }

    private void CheckCompletionStatus()
    {
        if (completionEventTriggered)
            return;

        float completionPercentage = (float)erasedPixels / totalPixels;

        if (completionPercentage >= completionThreshold)
        {
            CompleteErasing();
        }
    }

    private void CompleteErasing()
    {
        completionEventTriggered = true;

        // Completely erase any remaining pixels
        for (int x = 0; x < editableTexture.width; x++)
        {
            for (int y = 0; y < editableTexture.height; y++)
            {
                editableTexture.SetPixel(x, y, new Color(0, 0, 0, 0));
            }
        }

        editableTexture.Apply();

        // Trigger completion effects
        StartCoroutine(ShowCompletionEffects());
    }

    private IEnumerator ShowCompletionEffects()
    {
        // Invoke the UnityEvent to trigger any other custom behaviors
        onCompletionEvent.Invoke();

        // Spawn particle effects if assigned
        if (particleEffectPrefab != null)
        {
            Vector3 spawnPosition = imageRectTransform.position;
            GameObject particleInstance = Instantiate(particleEffectPrefab, spawnPosition, Quaternion.identity);

            // You might want to parent it to a specific object or configure it further
            ParticleSystem particleSystem = particleInstance.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                particleSystem.Play();
            }
        }

        // Wait a short delay before showing the win popup
        yield return new WaitForSeconds(1.0f);

        // Show win popup if assigned
        if (winPopupPrefab != null)
        {
            GameObject winPopup = Instantiate(winPopupPrefab, transform.parent);
            RectTransform popupRect = winPopup.GetComponent<RectTransform>();
            if (popupRect != null)
            {
                // Center the popup in the canvas
                popupRect.anchoredPosition = Vector2.zero;
            }
        }
    }

    // Public method that can be called from other scripts or animation events
    public void TriggerCompletionManually()
    {
        if (!completionEventTriggered)
        {
            CompleteErasing();
        }
    }
}