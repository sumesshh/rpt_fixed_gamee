using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class painting : MonoBehaviour
{
    public Image targetImage; // The UI Image to erase
    public float brushSize = 10f; // Brush radius in pixels
    public float eraseStrength = 0.1f; // How much alpha is reduced per stroke
    public Camera uiCamera; // Reference to the camera rendering the UI

    private Texture2D editableTexture;
    private RectTransform imageRectTransform;

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
        EraseCircle(texX, texY);

        // Apply changes to texture
        editableTexture.Apply();
    }

    private void EraseCircle(int centerX, int centerY)
    {
        int radius = Mathf.RoundToInt(brushSize);
        int width = editableTexture.width;
        int height = editableTexture.height;

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
                        pixel.a -= eraseStrength; // Reduce alpha
                        pixel.a = Mathf.Clamp01(pixel.a);
                        editableTexture.SetPixel(px, py, pixel);
                    }
                }
            }
        }
    }
}
