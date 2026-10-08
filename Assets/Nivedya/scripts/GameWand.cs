using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameWand : MonoBehaviour
    {
        public RectTransform rectTransform;
        public int width;
        public int height;
        public Texture2D maskTexture;
        public int scratchRadius = 100;
        private int erasedPixelCount = 0;
        private int totalOpaquePixels;  // Only count initially opaque pixels
        private bool isFullyErased = false;
        public UnityEvent win;
    //public Texture2D sourceMaskTexture;

        private void Start()
        {
            InitializeValues();
        }

        void InitializeValues()
        {
        rectTransform = GetComponent<RectTransform>();
        width = 150;
        height = 140;

        maskTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Image img = GetComponent<Image>();

        if (img == null)
        {
            Debug.LogError("No Image component found! Add an Image component to this GameObject.");
            return;
        }

        if (img.material == null)
        {
            img.material = new Material(Shader.Find("UI/Default"));
        }

        img.material.mainTexture = maskTexture;

        // Initialize texture with a fully visible white mask
        Color32[] pixels = new Color32[width * height];
        totalOpaquePixels = 0; // Reset counter

        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = new Color32(255, 255, 255, 255); // White, fully opaque
            totalOpaquePixels++; // Count opaque pixels
        }

        maskTexture.SetPixels32(pixels);
        maskTexture.Apply(false);

        Debug.Log("Total Non-Transparent Pixels: " + totalOpaquePixels);
        //rectTransform = GetComponent<RectTransform>();

        //// Load texture from Resources if not set
        //if (sourceMaskTexture == null)
        //{
        //    sourceMaskTexture = Resources.Load<Texture2D>("starMask");
        //}

        //if (sourceMaskTexture == null)
        //{
        //    Debug.LogError("Mask texture not found!");
        //    return;
        //}

        //width = sourceMaskTexture.width;
        //height = sourceMaskTexture.height;

        //maskTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        //Color32[] maskPixels = sourceMaskTexture.GetPixels32();
        //maskTexture.SetPixels32(maskPixels);
        //maskTexture.Apply();

        //Image img = GetComponent<Image>();
        //if (img.material == null)
        //{
        //    img.material = new Material(Shader.Find("UI/Default"));
        //}

        //img.material.mainTexture = maskTexture;

        //// Count total non-transparent pixels
        //totalOpaquePixels = 0;
        //foreach (Color32 px in maskPixels)
        //{
        //    if (px.a != 0)
        //        totalOpaquePixels++;
        //}

        //Debug.Log("Total opaque (white) pixels in mask: " + totalOpaquePixels);
    }

        public void ScratchAccordingToMousePosition(int xVal, int yVal)
        {
            if (isFullyErased) return;

            int xPos, yPos, yRangeforScratch;
            Color32[] tempColorArray = maskTexture.GetPixels32();
            bool isChangedPixel = false;

            for (int xOffsetPos = -scratchRadius; xOffsetPos <= scratchRadius; xOffsetPos++)
            {
                yRangeforScratch = (int)Mathf.Sqrt(scratchRadius * scratchRadius - xOffsetPos * xOffsetPos);
                for (int yoffsetPos = -yRangeforScratch; yoffsetPos <= yRangeforScratch; yoffsetPos++)
                {
                    xPos = xVal + xOffsetPos;
                    yPos = yVal + yoffsetPos;
                    if (CheckForScratch(xPos, yPos, tempColorArray))
                    {
                        isChangedPixel = true;
                    }
                }
            }

            if (isChangedPixel)
            {
                maskTexture.SetPixels32(tempColorArray);
                maskTexture.Apply(false);
                CheckIfFullyErased();
            }
        }

        public bool CheckForScratch(int xPos, int yPos, Color32[] pixels)
        {
            if (xPos >= 0 && xPos < width && yPos >= 0 && yPos < height)
            {
                int index = yPos * width + xPos;
                if (pixels[index].a != 0) // Only count opaque pixels
                {
                    pixels[index].a = 0; // Make it fully transparent
                    erasedPixelCount++;
                    return true;
                }
            }
            return false;
        }

        private void CheckIfFullyErased()
        {
            // Ensure at least 95% of pixels are erase
            if (!isFullyErased && erasedPixelCount >= totalOpaquePixels)
            {
                isFullyErased = true;
                Debug.Log("Image Fully Erased!");
                win.Invoke(); // Trigger event
            }
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, Camera.main, out Vector2 localPos);
                ScratchAccordingToMousePosition((int)localPos.x, (int)localPos.y);
            }
        }
    }


