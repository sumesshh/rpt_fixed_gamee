using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;
public enum DrawingTag
{
    Circle,
    Rectangle,
    None
}

public class CircleDetector : MonoBehaviour
{
    [System.Serializable]
    public class TaggedImage
    {
        public Image image;
        public DrawingTag expectedTag;
        public bool isRequired;
        public bool isWrongAnswer;  // New field to mark wrong answers
    }

    [Header("Setup")]
    [SerializeField] private List<TaggedImage> taggedImages;
    [SerializeField] private TMPro.TextMeshProUGUI feedbackText;

    [Header("Drawing Settings")]
    [SerializeField] private float lineWidth = 0.1f;
    [SerializeField] private Color lineColor = Color.blue;
    [SerializeField] private float minClosingDistance = 0.5f;
    [SerializeField] private int smoothness = 20;
    [SerializeField] private DrawingTag currentTag = DrawingTag.Circle;
    public UnityEvent onWin;
    public UnityEvent onWrong;

    private Camera mainCam;
    private GameObject lineObject;
    private LineRenderer currentLine;
    private List<Vector2> points = new List<Vector2>();
    private List<Vector2> controlPoints = new List<Vector2>();
    private bool isDrawing = false;
    private bool allowDrawing = true;

    private void Start()
    {
        mainCam = Camera.main;
        allowDrawing = true;

        if (taggedImages == null || taggedImages.Count == 0)
            Debug.LogError("No tagged images assigned to CircleDetector!");
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0) && allowDrawing)
        {
            StartDrawing();
        }
        else if (Input.GetMouseButton(0) && isDrawing && allowDrawing)
        {
            UpdateDrawing();
        }
        else if (Input.GetMouseButtonUp(0) && isDrawing && allowDrawing)
        {
            EndDrawing();
        }
    }

    private void StartDrawing()
    {
        DestroyLine();

        lineObject = new GameObject("DrawingLine");
        currentLine = lineObject.AddComponent<LineRenderer>();
        SetupLineRenderer();

        points.Clear();
        controlPoints.Clear();
        isDrawing = true;

        AddPoint(GetWorldMousePosition());
    }

    private void SetupLineRenderer()
    {
        currentLine.material = new Material(Shader.Find("Sprites/Default"));
        currentLine.startColor = lineColor;
        currentLine.endColor = lineColor;
        currentLine.startWidth = lineWidth;
        currentLine.endWidth = lineWidth;
        currentLine.positionCount = 0;
        currentLine.useWorldSpace = true;
    }

    private void UpdateDrawing()
    {
        AddPoint(GetWorldMousePosition());
    }

    private void EndDrawing()
    {
        isDrawing = false;

        if (points.Count > 10 && IsShapeClosed())
        {
            CheckTaggedObjects();
        }
        else
        {
            if (feedbackText != null)
            {
                feedbackText.text = points.Count <= 10 ? "Draw a longer line!" : "Close the shape!";
            }
            DestroyLine();
        }
    }

    private void AddPoint(Vector2 point)
    {
        if (controlPoints.Count == 0 || Vector2.Distance(point, controlPoints[controlPoints.Count - 1]) > 0.1f)
        {
            controlPoints.Add(point);

            if (controlPoints.Count >= 4)
            {
                GenerateSmoothCurve();
            }
            else
            {
                points.Add(point);
                UpdateLineRenderer();
            }
        }
    }

    private void GenerateSmoothCurve()
    {
        points.Clear();

        for (int i = 0; i < controlPoints.Count - 3; i++)
        {
            Vector2 p0 = controlPoints[i];
            Vector2 p1 = controlPoints[i + 1];
            Vector2 p2 = controlPoints[i + 2];
            Vector2 p3 = controlPoints[i + 3];

            for (int j = 0; j < smoothness; j++)
            {
                float t = j / (float)smoothness;
                points.Add(CatmullRomPoint(p0, p1, p2, p3, t));
            }
        }

        UpdateLineRenderer();
    }

    private Vector2 CatmullRomPoint(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        return 0.5f * (
            (-t3 + 2f * t2 - t) * p0 +
            (3f * t3 - 5f * t2 + 2f) * p1 +
            (-3f * t3 + 4f * t2 + t) * p2 +
            (t3 - t2) * p3
        );
    }

    private void UpdateLineRenderer()
    {
        if (currentLine != null)
        {
            currentLine.positionCount = points.Count;
            for (int i = 0; i < points.Count; i++)
            {
                currentLine.SetPosition(i, new Vector3(points[i].x, points[i].y, 0));
            }
        }
    }

    private void CheckTaggedObjects()
    {
        bool allRequiredFound = true;
        bool anyWrongFound = false;
        List<string> wrongAnswers = new List<string>();  // Track wrong answers for feedback

        foreach (TaggedImage taggedImage in taggedImages)
        {
            if (taggedImage.image == null) continue;

            Vector3[] corners = new Vector3[4];
            taggedImage.image.rectTransform.GetWorldCorners(corners);
            Vector2 imageCenter = (corners[0] + corners[2]) / 2f;
            bool isInside = IsPointInShape(imageCenter);

            // Check if this is a wrong answer
            if (taggedImage.isWrongAnswer && isInside)
            {
                anyWrongFound = true;
                wrongAnswers.Add(taggedImage.image.name);  // Add the name of the wrong answer
                continue;
            }

            // Check for required correct answers
            if (taggedImage.expectedTag == currentTag)
            {
                if (taggedImage.isRequired && !isInside)
                {
                    allRequiredFound = false;
                }
            }
            else if (isInside)
            {
                anyWrongFound = true;
            }
        }

        HandleDrawingResult(allRequiredFound, anyWrongFound, wrongAnswers);
    }

    private void HandleDrawingResult(bool allRequiredFound, bool anyWrongFound, List<string> wrongAnswers)
    {
        if (allRequiredFound && !anyWrongFound)
        {
            if (feedbackText != null)
                feedbackText.text = "Correct!";

            Debug.Log("Correct!");
            allowDrawing = false;
            if (onWin != null) {
                onWin.Invoke();
            }
        }
        else
        {
            if (feedbackText != null)
            {
                if (!allRequiredFound)
                {
                    feedbackText.text = "Wrong! Missing required objects!";
                }
                else if (wrongAnswers.Count > 0)
                {
                    // Create detailed feedback for wrong answers
                    string wrongItems = string.Join(", ", wrongAnswers);
                    feedbackText.text = $"Wrong! Incorrect items selected: {wrongItems}";
                }
                else
                {
                    feedbackText.text = "Wrong! Check your selection!";
                }
            }
            Debug.Log("Wrong Answer!");
            if (onWrong != null) {
                onWrong.Invoke();
            }
            DestroyLine();
        }
    }

    private bool IsShapeClosed()
    {
        if (points.Count < 10) return false;
        return Vector2.Distance(points[0], points[points.Count - 1]) < minClosingDistance;
    }

    private bool IsPointInShape(Vector2 point)
    {
        int intersections = 0;

        for (int i = 0; i < points.Count - 1; i++)
        {
            if (IsLineIntersecting(point, new Vector2(point.x + 1000, point.y), points[i], points[i + 1]))
            {
                intersections++;
            }
        }

        if (IsLineIntersecting(point, new Vector2(point.x + 1000, point.y), points[points.Count - 1], points[0]))
        {
            intersections++;
        }

        return (intersections % 2) == 1;
    }

    private bool IsLineIntersecting(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        float denominator = (p2.x - p1.x) * (p4.y - p3.y) - (p2.y - p1.y) * (p4.x - p3.x);

        if (denominator == 0)
            return false;

        float t = ((p1.y - p3.y) * (p4.x - p3.x) - (p1.x - p3.x) * (p4.y - p3.y)) / denominator;
        float u = ((p1.y - p3.y) * (p2.x - p1.x) - (p1.x - p3.x) * (p2.y - p1.y)) / denominator;

        return t >= 0 && t <= 1 && u >= 0 && u <= 1;
    }

    private void DestroyLine()
    {
        if (lineObject != null)
        {
            Destroy(lineObject);
            lineObject = null;
            currentLine = null;
        }
    }

    private Vector2 GetWorldMousePosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -mainCam.transform.position.z;
        return mainCam.ScreenToWorldPoint(mousePos);
    }
}