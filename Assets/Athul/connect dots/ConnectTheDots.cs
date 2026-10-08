using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ConnectTheDots : MonoBehaviour
{
    public List<Transform> dots; // Assign the dots in the Inspector
    public LineRenderer lineRenderer; // Assign the LineRenderer in the Inspector
    public Image winningImage; // Reference to the Image component that will display the winning image
    public Image dotimage;
    public TMP_Text winMessage; // Reference to the TextMeshPro component for the win message
    public TMP_Text lossMessage; // Reference to the TextMeshPro component for the loss message

    // Audio clips
    public AudioClip winSound; // Win sound to play when image is displayed
    public AudioClip lossSound; // Loss sound to play when loss message is displayed
    public AudioClip tickSound; // Sound to play when each dot is connected

    private AudioSource audioSource; // We'll create this at runtime
    private int currentDotIndex = 0;
    private bool gameLost = false;
    private bool gameStarted = false; // New flag to track if the game has started

    // Track which dots have been clicked
    private List<bool> clickedDots;
    private Vector2 lastPosition;

    // Store the correct order of dots
    private List<int> correctOrder = new List<int>();
    private List<int> playerOrder = new List<int>();

    // Camera reference
    private Camera mainCamera;

    void Start()
    {
        // Cache the main camera reference
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found. Please ensure there is a camera tagged as 'MainCamera'.");
            return;
        }

        // Create and configure AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = 1.0f;

        // Initialize the LineRenderer
        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;

        // Configure line renderer for rounded caps
        lineRenderer.numCornerVertices = 15; // More vertices for smoother rounded corners
        lineRenderer.numCapVertices = 15; // Rounded caps

        // Set a material that supports rounded ends
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));

        // Set initial color to green
        SetLineRendererColor(Color.green);

        // Initialize the clickedDots list
        clickedDots = new List<bool>(new bool[dots.Count]);

        // Initialize the winning image and messages to be hidden
        if (winningImage != null)
            winningImage.gameObject.SetActive(false);

        if (winMessage != null)
            winMessage.gameObject.SetActive(false);

        if (lossMessage != null)
            lossMessage.gameObject.SetActive(false);

        // Set up the correct order (by default, it's the order in the dots list)
        for (int i = 0; i < dots.Count; i++)
        {
            correctOrder.Add(i);
        }
    }

    void SetLineRendererColor(Color color)
    {
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }

    void Update()
    {
        // Skip if game is already over or camera is missing
        if (gameLost || currentDotIndex >= dots.Count || mainCamera == null) return;

        // Variable to store the current input position
        Vector2 worldInputPosition = Vector2.zero;
        bool isInputActive = false;
        bool validInput = false;

        // Handle touch input (for mobile devices)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Convert touch position to world position
            try
            {
                worldInputPosition = mainCamera.ScreenToWorldPoint(touch.position);
                isInputActive = touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Stationary;
                validInput = true;
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("Error processing touch input: " + ex.Message);
            }
        }
        // Handle mouse input (for desktop/editor)
        else if (Input.GetMouseButton(0))
        {
            // Convert mouse position to world position
            try
            {
                worldInputPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                isInputActive = true;
                validInput = true;
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("Error processing mouse input: " + ex.Message);
            }
        }

        // Skip the rest if we don't have valid input
        if (!validInput) return;

        // Update the last point of the LineRenderer to follow the input only if game has started
        if (gameStarted && currentDotIndex > 0)
        {
            lineRenderer.positionCount = currentDotIndex + 1;
            lineRenderer.SetPosition(currentDotIndex, worldInputPosition);
        }

        // Check if the dragged line intersects any dot
        if (isInputActive)
        {
            for (int i = 0; i < dots.Count; i++)
            {
                if (!clickedDots[i]) // Consider only unclicked dots
                {
                    if (!gameStarted) // First dot click
                    {
                        // If this is the first dot, just mark it
                        if (Vector2.Distance(worldInputPosition, dots[i].position) < 0.2f)
                        {
                            gameStarted = true;
                            ConnectFirstDot(i);
                            PlaySound(tickSound); // Play tick sound for first dot
                        }
                    }
                    else if (IsLineIntersectingDot(lastPosition, worldInputPosition, dots[i].position))
                    {
                        // Check if this is the last dot
                        bool isLastDot = currentDotIndex == dots.Count - 1;

                        ConnectDot(i);

                        // Don't play the tick sound for the last dot, as we'll play win/loss sound instead
                        if (!isLastDot)
                        {
                            PlaySound(tickSound);
                        }
                    }
                }
            }
        }
    }

    void ConnectFirstDot(int dotIndex)
    {
        // Mark the dot as clicked
        clickedDots[dotIndex] = true;
        lastPosition = dots[dotIndex].position;

        // Add this dot to the player's order
        playerOrder.Add(dotIndex);

        // Initialize the LineRenderer with the first dot
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, dots[dotIndex].position);
        currentDotIndex = 1;
    }

    void ConnectDot(int dotIndex)
    {
        // Mark the dot as clicked
        clickedDots[dotIndex] = true;
        lastPosition = dots[dotIndex].position;

        // Add this dot to the player's order
        playerOrder.Add(dotIndex);

        // Add the dot's position to the LineRenderer
        lineRenderer.positionCount = currentDotIndex + 1;
        lineRenderer.SetPosition(currentDotIndex, dots[dotIndex].position);
        currentDotIndex++;

        // Check if all dots are connected
        if (currentDotIndex == dots.Count)
        {
            CheckResult();
        }
    }

    void CheckResult()
    {
        bool correctSequence = true;

        // Check if the player connected the dots in the correct order
        for (int i = 0; i < correctOrder.Count; i++)
        {
            if (playerOrder[i] != correctOrder[i])
            {
                correctSequence = false;
                break;
            }
        }

        if (correctSequence)
        {
            Debug.Log("All dots connected correctly!");
            DisplayWinningState();
            PlaySound(winSound); // Play winning sound
        }
        else
        {
            Debug.Log("Dots connected in wrong order!");
            DisplayLossState();
            PlaySound(lossSound); // Play losing sound
        }
    }

    void PlaySound(AudioClip clip)
    {
        // Only play if we have a valid clip and AudioSource
        if (clip != null && audioSource != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    bool IsLineIntersectingDot(Vector2 start, Vector2 end, Vector2 dotPosition)
    {
        float distance = DistanceFromPointToLine(start, end, dotPosition);
        return distance < 0.2f; // Adjust this threshold for better accuracy
    }

    float DistanceFromPointToLine(Vector2 start, Vector2 end, Vector2 point)
    {
        float lengthSquared = (end - start).sqrMagnitude;
        if (lengthSquared == 0) return Vector2.Distance(point, start);

        float t = Mathf.Clamp01(Vector2.Dot(point - start, end - start) / lengthSquared);
        Vector2 projection = start + t * (end - start);
        return Vector2.Distance(point, projection);
    }

    void DisplayWinningState()
    {
        // Hide the line renderer when the game is won
        lineRenderer.positionCount = 0;

        // Show the winning image
        if (winningImage != null)
        {
            winningImage.gameObject.SetActive(true);
            dotimage.gameObject.SetActive(false);
        }

        // Show the win message
        if (winMessage != null)
        {
            winMessage.gameObject.SetActive(true);
        }
    }

    void DisplayLossState()
    {
        // Game lost, display the loss message
        gameLost = true;

        // Change line renderer color to red
        SetLineRendererColor(Color.red);

        // Show the loss message
        if (lossMessage != null)
        {
            lossMessage.gameObject.SetActive(true);
        }
    }

    //// Method to restart the game (you can call this from a button)
    //public void RestartGame()
    //{
    //    // Reset variables
    //    currentDotIndex = 0;
    //    gameLost = false;
    //    gameStarted = false; // Reset the game started flag
    //    playerOrder.Clear();

    //    // Reset clicked dots
    //    for (int i = 0; i < clickedDots.Count; i++)
    //    {
    //        clickedDots[i] = false;
    //    }

    //    // Reset line renderer
    //    lineRenderer.positionCount = 0;

    //    // Hide UI elements
    //    if (winningImage != null)
    //        winningImage.gameObject.SetActive(false);

    //    if (winMessage != null)
    //        winMessage.gameObject.SetActive(false);

    //    if (lossMessage != null)
    //        lossMessage.gameObject.SetActive(false);

    //    // Make sure dot image is visible again    
    //    if (dotimage != null)
    //        dotimage.gameObject.SetActive(true);
    //}

    // Set the correct order of dots (can be called from editor scripts or other components)
    public void SetCorrectOrder(List<int> order)
    {
        correctOrder = new List<int>(order);
    }
}