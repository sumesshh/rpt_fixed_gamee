using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TraceDrawingGame : MonoBehaviour
{
    [Header("Drawing Settings")]
    [SerializeField] private float lineWidth = 0.1f;
    [SerializeField] private Color playerLineColor = Color.blue;
    [SerializeField] private Color trialLineColor = Color.gray;
    [SerializeField] private float drawingResolution = 0.1f; // Adjust if you need more/fewer points

    [Header("Game Settings")]
    [SerializeField] private float successThreshold = 0.5f;
    [SerializeField][Range(0, 1)] private float requiredAccuracy = 0.7f;

    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Text feedbackText;
    [SerializeField] private Button startButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button checkButton;
    [SerializeField] private LineRenderer trialLineRenderer;

    private LineRenderer playerLineRenderer;
    private List<Vector3> drawnPoints = new List<Vector3>();
    private List<Vector3> trialPoints = new List<Vector3>();
    private bool isDrawing = false;
    private bool gameActive = false;

    private void Start()
    {
        InitializeTrialLine();
        SetupButtons();
        SetGameState(false);
    }

    /// <summary>
    /// Initializes the trial line with preset points.
    /// </summary>
    private void InitializeTrialLine()
    {
        if (trialLineRenderer == null)
        {
            Debug.LogError("Trial Line Renderer not assigned!");
            return;
        }

        // Define trial points (all at z = 0)
        trialPoints = new List<Vector3>(new Vector3[]
        {
            new Vector3(-2, 0, 0),
            new Vector3(-1, 1, 0),
            new Vector3(0, 0, 0),
            new Vector3(1, 1, 0),
            new Vector3(2, 0, 0)
        });

        trialLineRenderer.positionCount = trialPoints.Count;
        trialLineRenderer.SetPositions(trialPoints.ToArray());
        trialLineRenderer.startWidth = lineWidth;
        trialLineRenderer.endWidth = lineWidth;
        trialLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        trialLineRenderer.startColor = trialLineColor;
        trialLineRenderer.endColor = trialLineColor;
    }

    /// <summary>
    /// Sets up button listeners.
    /// </summary>
    private void SetupButtons()
    {
        startButton.onClick.AddListener(StartGame);
        resetButton.onClick.AddListener(ResetDrawing);
        checkButton.onClick.AddListener(EvaluateDrawing);
    }

    private void Update()
    {
        if (!gameActive) return;
        HandleDrawingInput();
    }

    /// <summary>
    /// Processes mouse input for drawing.
    /// </summary>
    private void HandleDrawingInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartNewLine();
        }
        else if (Input.GetMouseButton(0) && isDrawing)
        {
            ContinueLine();
        }
        else if (Input.GetMouseButtonUp(0) && isDrawing)
        {
            EndLine();
        }
    }

    /// <summary>
    /// Starts a new line drawing.
    /// </summary>
    private void StartNewLine()
    {
        isDrawing = true;
        drawnPoints.Clear();

        GameObject lineObject = new GameObject("PlayerLine");
        playerLineRenderer = lineObject.AddComponent<LineRenderer>();

        playerLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        playerLineRenderer.startColor = playerLineColor;
        playerLineRenderer.endColor = playerLineColor;
        playerLineRenderer.startWidth = lineWidth;
        playerLineRenderer.endWidth = lineWidth;
        playerLineRenderer.useWorldSpace = true;

        // Add the first point of the drawing
        AddPoint(GetMouseWorldPosition());
    }

    /// <summary>
    /// Continues the line by adding points when the mouse moves.
    /// </summary>
    private void ContinueLine()
    {
        Vector3 newPoint = GetMouseWorldPosition();
        if (drawnPoints.Count == 0 || Vector3.Distance(newPoint, drawnPoints[drawnPoints.Count - 1]) > drawingResolution)
        {
            AddPoint(newPoint);
        }
    }

    /// <summary>
    /// Adds a point to the drawn line.
    /// </summary>
    private void AddPoint(Vector3 point)
    {
        drawnPoints.Add(point);
        playerLineRenderer.positionCount = drawnPoints.Count;
        playerLineRenderer.SetPosition(drawnPoints.Count - 1, point);
    }

    /// <summary>
    /// Ends the current drawing. Destroys the line if not enough points were drawn.
    /// </summary>
    private void EndLine()
    {
        isDrawing = false;
        if (drawnPoints.Count < 2)
        {
            Destroy(playerLineRenderer.gameObject);
        }
    }

    /// <summary>
    /// Evaluates the player's drawing against the trial line.
    /// </summary>
    private void EvaluateDrawing()
    {
        if (drawnPoints.Count < 2)
        {
            UpdateFeedback("Draw something first!");
            return;
        }

        int matchedPoints = 0;
        foreach (Vector3 trialPoint in trialPoints)
        {
            foreach (Vector3 drawnPoint in drawnPoints)
            {
                if (Vector3.Distance(trialPoint, drawnPoint) <= successThreshold)
                {
                    matchedPoints++;
                    break;
                }
            }
        }

        float accuracy = (float)matchedPoints / trialPoints.Count;
        if (accuracy >= requiredAccuracy)
        {
            UpdateFeedback("Great job! Accuracy: " + (accuracy * 100).ToString("F1") + "%");
        }
        else
        {
            UpdateFeedback("Try again! Accuracy: " + (accuracy * 100).ToString("F1") + "%");
        }
    }

    /// <summary>
    /// Converts the mouse screen position to a world position on the z=0 plane.
    /// </summary>
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        // Set z to the distance from the camera to the z=0 plane.
        mousePos.z = -mainCamera.transform.position.z;
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
        // Force the drawn points onto the z=0 plane.
        worldPos.z = 0f;
        return worldPos;
    }

    /// <summary>
    /// Starts the game, enabling drawing and UI elements.
    /// </summary>
    private void StartGame()
    {
        gameActive = true;
        SetGameState(true);
        UpdateFeedback("Trace the gray line!");
    }

    /// <summary>
    /// Resets the player's drawing.
    /// </summary>
    private void ResetDrawing()
    {
        if (playerLineRenderer != null)
        {
            Destroy(playerLineRenderer.gameObject);
        }
        drawnPoints.Clear();
        UpdateFeedback("Drawing reset!");
    }

    /// <summary>
    /// Activates or deactivates game UI elements.
    /// </summary>
    private void SetGameState(bool active)
    {
        trialLineRenderer.gameObject.SetActive(active);
        resetButton.gameObject.SetActive(active);
        checkButton.gameObject.SetActive(active);
        startButton.gameObject.SetActive(!active);
    }

    /// <summary>
    /// Updates the on-screen feedback text.
    /// </summary>
    private void UpdateFeedback(string message)
    {
        feedbackText.text = message;
    }

    private void OnDestroy()
    {
        startButton.onClick.RemoveAllListeners();
        resetButton.onClick.RemoveAllListeners();
        checkButton.onClick.RemoveAllListeners();
    }
}
