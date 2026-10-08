using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro; // Import TextMeshPro namespace
using System.Collections;

public class BrushTwo : MonoBehaviour
{
    public RectTransform rectTransform;
    public int width;
    public int height;
    public Texture2D maskTexture;
    public int scratchRadius;
    private int erasedPixelCount = 0;
    private int totalOpaquePixels;
    private bool isFullyErased = false;
    public UnityEvent win;

    // References to the brush option images
    public Image thickerBrush;
    public Image thinnerBrush;

    // Track which brush is currently selected
    private bool isThickerBrushSelected = false;
    private bool isAnyBrushSelected = false;

    // UI TextMeshPro references for messages
    public TMP_Text winText;   // Win message
    public TMP_Text retryText; // Retry message
    public TMP_Text feedbackText; // Feedback while playing
    public TMP_Text describe; // TextMeshPro for description when correct brush is chosen

    // Duration for describe text display in seconds
    private float describeDisplayDuration = 3.0f;
    private Coroutine describeTextCoroutine;

    private void Start()
    {
        InitializeValues();

        // Add click listeners to brush images
        if (thickerBrush != null)
        {
            Button thickerButton = thickerBrush.GetComponent<Button>();
            if (thickerButton == null)
            {
                thickerButton = thickerBrush.gameObject.AddComponent<Button>();
            }
            thickerButton.onClick.AddListener(() => SelectBrush(true));
        }

        if (thinnerBrush != null)
        {
            Button thinnerButton = thinnerBrush.GetComponent<Button>();
            if (thinnerButton == null)
            {
                thinnerButton = thinnerBrush.gameObject.AddComponent<Button>();
            }
            thinnerButton.onClick.AddListener(() => SelectBrush(false));
        }

        // Hide texts initially
        if (winText != null) winText.gameObject.SetActive(false);
        if (retryText != null) retryText.gameObject.SetActive(false);
        if (describe != null) describe.gameObject.SetActive(false); // Hide describe text initially
    }

    private void SelectBrush(bool isThicker)
    {
        isThickerBrushSelected = isThicker;
        isAnyBrushSelected = true;

        // Visual feedback
        if (thickerBrush != null)
            thickerBrush.color = isThicker ? Color.gray : Color.white;
        if (thinnerBrush != null)
            thinnerBrush.color = !isThicker ? Color.gray : Color.white;

        // Console log
        Debug.Log(isThicker ? "Thicker brush selected" : "Thinner brush selected");

        // Show appropriate feedback
        UpdateFeedbackText();

        // Handle the describe text based on brush selection
        if (isThicker)
        {
            ShowDescribeTextTemporarily();
        }
        else
        {
            // If thinner brush is selected, make sure describe text is hidden
            if (describe != null)
            {
                describe.gameObject.SetActive(false);
            }

            // Stop any active coroutine
            if (describeTextCoroutine != null)
            {
                StopCoroutine(describeTextCoroutine);
                describeTextCoroutine = null;
            }
        }
    }

    private void ShowDescribeTextTemporarily()
    {
        // Stop any active coroutine first
        if (describeTextCoroutine != null)
        {
            StopCoroutine(describeTextCoroutine);
        }

        // Start new coroutine
        describeTextCoroutine = StartCoroutine(ShowDescribeForDuration());
    }

    private IEnumerator ShowDescribeForDuration()
    {
        if (describe != null)
        {
            // Set text and show
            describe.text = "Wow! Now you may paint the rainbow";
            describe.gameObject.SetActive(true);

            // Wait for the specified duration
            yield return new WaitForSeconds(describeDisplayDuration);

            // Hide the text after duration
            describe.gameObject.SetActive(false);
        }

        describeTextCoroutine = null;
    }

    private void UpdateFeedbackText()
    {
        if (feedbackText != null)
        {
            if (!isThickerBrushSelected)
            {
                feedbackText.text = "Retry!";

                // Show Retry Text if thinner brush is selected
                if (retryText != null) retryText.gameObject.SetActive(true);

                // Log to console
                Debug.Log("Retry: Please use the thicker brush!");
            }
            else
            {
                feedbackText.text = "Correct! You can erase with the thicker brush.";

                // Hide Retry Text if thicker brush is selected
                if (retryText != null) retryText.gameObject.SetActive(false);
            }
        }
    }

    void InitializeValues()
    {
        rectTransform = GetComponent<RectTransform>();
        width = (int)rectTransform.sizeDelta.x;
        height = (int)rectTransform.sizeDelta.y;

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

        Color32[] pixels = new Color32[width * height];
        totalOpaquePixels = 0;

        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = new Color32(255, 255, 255, 255);
            totalOpaquePixels++;
        }

        maskTexture.SetPixels32(pixels);
        maskTexture.Apply(false);

        Debug.Log("Total Non-Transparent Pixels: " + totalOpaquePixels);
    }

    public void ScratchAccordingToMousePosition(int xVal, int yVal)
    {
        // Only allow erasing with thicker brush
        if (!isThickerBrushSelected)
        {
            UpdateFeedbackText();
            return;
        }

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
            if (pixels[index].a != 0)
            {
                pixels[index].a = 0;
                erasedPixelCount++;
                return true;
            }
        }
        return false;
    }

    private void CheckIfFullyErased()
    {
        float eraseThreshold = 0.95f;
        if (!isFullyErased && erasedPixelCount >= totalOpaquePixels * eraseThreshold)
        {
            isFullyErased = true;
            Debug.Log("Image Fully Erased!");

            if (feedbackText != null)
            {
                feedbackText.text = "Great job! Item completely erased!";
            }

            // Show Win Text
            if (winText != null) winText.gameObject.SetActive(true);

            win.Invoke();
        }
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && isAnyBrushSelected)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, Camera.main, out Vector2 localPos))
            {
                int texX = (int)(((localPos.x - rectTransform.rect.x) / rectTransform.rect.width) * width);
                int texY = (int)(((localPos.y - rectTransform.rect.y) / rectTransform.rect.height) * height);

                texY = height - texY;

                ScratchAccordingToMousePosition(texX, texY);
            }
        }
    }
}