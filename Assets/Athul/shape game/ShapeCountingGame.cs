using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class ShapeCountingGame : MonoBehaviour
{
    [System.Serializable]
    public class ShapeType
    {
        public string shapeName;
        public Sprite shapeIcon;
        public int correctCount;
        public Sprite highlightedImage; // Add this field for the highlighted image
    }

    // Game Settings
    public List<ShapeType> shapesToCount = new List<ShapeType>();
    public int currentShapeIndex = 0;

    // UI References
    public Image mainImage; // Reference to the main image
    public Image currentShapeIcon;
    public TextMeshProUGUI counterText;
    public Button decrementButton;
    public Button incrementButton;
    public Button okButton;
    public TextMeshProUGUI newTextLabel;

    // Audio Clips (AudioSource will be added automatically)
    public AudioClip correctSound;
    public AudioClip incorrectSound;
    public AudioClip gameCompleteSound;

    // Win Event - Called when all shapes have been counted correctly
    public UnityEvent onGameComplete = new UnityEvent();

    private int currentCount = 0;
    private AudioSource audioSource;

    void Awake()
    {
        // Add AudioSource component automatically if it doesn't exist
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            Debug.Log("AudioSource component added automatically");
        }
    }

    void Start()
    {
        // Add default shapes if the list is empty
        if (shapesToCount.Count == 0)
        {
            Debug.Log("No shapes defined. Adding example shapes.");
            AddExampleShapes();
        }

        // Set up button listeners
        decrementButton.onClick.AddListener(DecrementCount);
        incrementButton.onClick.AddListener(IncrementCount);
        okButton.onClick.AddListener(CheckAnswer);

        // Initialize game
        currentCount = 0;
        UpdateCounterText();
        SetupCurrentShape();

        // Debug info
        Debug.Log("Game started with " + shapesToCount.Count + " shapes to count");
    }

    // Helper method to add example shapes for testing
    private void AddExampleShapes()
    {
        ShapeType square = new ShapeType();
        square.shapeName = "Square";
        square.correctCount = 10;
        square.highlightedImage = Resources.Load<Sprite>("HighlightedSquare"); // Load highlighted image

        ShapeType circle = new ShapeType();
        circle.shapeName = "Circle";
        circle.correctCount = 8;
        circle.highlightedImage = Resources.Load<Sprite>("HighlightedCircle"); // Load highlighted image

        ShapeType triangle = new ShapeType();
        triangle.shapeName = "Triangle";
        triangle.correctCount = 14;
        triangle.highlightedImage = Resources.Load<Sprite>("HighlightedTriangle"); // Load highlighted image

        shapesToCount.Add(square);
        shapesToCount.Add(circle);
        shapesToCount.Add(triangle);
    }

    private void SetupCurrentShape()
    {
        if (currentShapeIndex < shapesToCount.Count)
        {
            // Set the current shape icon
            if (currentShapeIcon != null && shapesToCount[currentShapeIndex].shapeIcon != null)
            {
                currentShapeIcon.sprite = shapesToCount[currentShapeIndex].shapeIcon;
            }
            else
            {
                Debug.LogWarning("Shape icon missing for: " + shapesToCount[currentShapeIndex].shapeName);
            }

            // Reset the counter
            currentCount = 0;
            UpdateCounterText();

            // Update the text label
            if (newTextLabel != null)
            {
                newTextLabel.text = "Count the number of " + shapesToCount[currentShapeIndex].shapeName + "s";
            }

            Debug.Log("Now counting: " + shapesToCount[currentShapeIndex].shapeName +
                     " (Correct answer: " + shapesToCount[currentShapeIndex].correctCount + ")");
        }
        else
        {
            // Game complete
            GameComplete();
        }
    }

    public void IncrementCount()
    {
        currentCount++;
        UpdateCounterText();
        Debug.Log("Count increased to: " + currentCount);
    }

    public void DecrementCount()
    {
        if (currentCount > 0)
        {
            currentCount--;
            UpdateCounterText();
            Debug.Log("Count decreased to: " + currentCount);
        }
    }

    private void UpdateCounterText()
    {
        if (counterText != null)
        {
            counterText.text = currentCount.ToString();
        }
    }

    public void CheckAnswer()
    {
        Debug.Log("Checking answer: " + currentCount + " vs correct: " +
                 shapesToCount[currentShapeIndex].correctCount);

        if (currentCount == shapesToCount[currentShapeIndex].correctCount)
        {
            // Correct answer
            Debug.Log("CORRECT ANSWER");

            // Play sound
            if (correctSound != null)
            {
                Debug.Log("Playing correct sound");
                audioSource.PlayOneShot(correctSound);
            }
            else
            {
                Debug.LogWarning("Correct sound clip is missing");
            }

            // Move to next shape
            currentShapeIndex++;
            SetupCurrentShape();
        }
        else
        {
            // Incorrect answer
            Debug.Log("INCORRECT ANSWER");

            // Play sound
            if (incorrectSound != null)
            {
                Debug.Log("Playing incorrect sound");
                audioSource.PlayOneShot(incorrectSound);
            }
            else
            {
                Debug.LogWarning("Incorrect sound clip is missing");
            }

            // Show feedback
            StartCoroutine(ShowIncorrectFeedback());
        }
    }

    private IEnumerator ShowIncorrectFeedback()
    {
        if (counterText != null)
        {
            // Simple feedback - you could replace with animation
            Color originalColor = counterText.color;
            counterText.color = Color.red;

            // Change the main image to the highlighted image
            if (mainImage != null && shapesToCount[currentShapeIndex].highlightedImage != null)
            {
                Sprite originalImage = mainImage.sprite;
                mainImage.sprite = shapesToCount[currentShapeIndex].highlightedImage;

                yield return new WaitForSeconds(2f);

                mainImage.sprite = originalImage;
            }

            counterText.color = originalColor;
        }
        else
        {
            yield return null;
        }
    }

    private void GameComplete()
    {
        Debug.Log("Game Complete! Invoking Win event!");

        // Play completion sound
        if (gameCompleteSound != null)
        {
            audioSource.PlayOneShot(gameCompleteSound);
        }

        // Hide the shape icon
        if (currentShapeIcon != null)
        {
            currentShapeIcon.gameObject.SetActive(false);
        }

        // Update text
        if (newTextLabel != null)
        {
            newTextLabel.text = "Well done! All shapes counted!";
        }

        // Disable buttons
        if (decrementButton != null) decrementButton.interactable = false;
        if (incrementButton != null) incrementButton.interactable = false;
        if (okButton != null) okButton.interactable = false;

        // Invoke the win event
        onGameComplete.Invoke();
    }

    // Helper function to manually set up the game from the Inspector
    [ContextMenu("Setup Example Game")]
    public void SetupExampleGame()
    {
        Debug.Log("Setting up example game...");
        shapesToCount.Clear();
        AddExampleShapes();
        currentShapeIndex = 0;
    }
}