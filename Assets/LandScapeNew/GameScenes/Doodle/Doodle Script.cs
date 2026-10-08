using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DoodlePage : MonoBehaviour
{
    [Header("Drawing Settings")]
    [SerializeField] private float lineWidth = 0.1f;
    [SerializeField] private Color lineColor = Color.blue;
    [SerializeField] private int smoothness = 20;

    [Header("Eraser Settings")]
    [SerializeField] private float eraserRadius = 0.5f;
    [SerializeField] private Color eraserColor = Color.red;

    [Header("UI References")]
    [SerializeField] private Button eraserToggleButton;
    [SerializeField] private Image eraserButtonImage;
    [SerializeField] private Color activeToolColor = Color.green;
    [SerializeField] private Color inactiveToolColor = Color.white;

    private Camera mainCam;
    private GameObject lineObject;
    private LineRenderer currentLine;
    private List<Vector2> points = new List<Vector2>();
    private List<Vector2> controlPoints = new List<Vector2>();
    private bool isDrawing = false;
    private bool isEraserMode = false;
    private GameObject eraserVisual;

    private void Start()
    {
        mainCam = Camera.main;
        SetupEraserButton();
        CreateEraserVisual();
    }

    private void SetupEraserButton()
    {
        if (eraserToggleButton != null)
        {
            eraserToggleButton.onClick.AddListener(ToggleEraserMode);
            UpdateEraserButtonVisual();
        }
    }

    private void CreateEraserVisual()
    {
        eraserVisual = new GameObject("EraserVisual");
        eraserVisual.transform.SetParent(transform);

        LineRenderer circle = eraserVisual.AddComponent<LineRenderer>();
        circle.material = new Material(Shader.Find("Sprites/Default"));
        circle.startColor = eraserColor;
        circle.endColor = eraserColor;
        circle.startWidth = 0.05f;
        circle.endWidth = 0.05f;
        circle.loop = true;

        int segments = 30;
        circle.positionCount = segments + 1;

        for (int i = 0; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            float x = Mathf.Cos(angle) * eraserRadius;
            float y = Mathf.Sin(angle) * eraserRadius;
            circle.SetPosition(i, new Vector3(x, y, 0));
        }

        eraserVisual.SetActive(false);
    }

    private void Update()
    {
        Vector2 mousePos = GetWorldMousePosition();
        UpdateEraserVisual(mousePos);

        if (isEraserMode)
        {
            HandleEraser(mousePos);
        }
        else
        {
            HandleDrawing(mousePos);
        }
    }

    private void UpdateEraserVisual(Vector2 position)
    {
        if (eraserVisual != null)
        {
            eraserVisual.SetActive(isEraserMode);
            eraserVisual.transform.position = new Vector3(position.x, position.y, 0);
        }
    }

    private void HandleDrawing(Vector2 mousePos)
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartDrawing(mousePos);
        }
        else if (Input.GetMouseButton(0) && isDrawing)
        {
            UpdateDrawing(mousePos);
        }
        else if (Input.GetMouseButtonUp(0) && isDrawing)
        {
            EndDrawing();
        }
    }

    private void HandleEraser(Vector2 mousePos)
    {
        if (Input.GetMouseButton(0))
        {
            EraseAtPosition(mousePos);
        }
    }

    private void StartDrawing(Vector2 mousePos)
    {
        lineObject = new GameObject("DrawingLine");
        lineObject.transform.SetParent(transform);
        currentLine = lineObject.AddComponent<LineRenderer>();
        SetupLineRenderer();

        points.Clear();
        controlPoints.Clear();
        isDrawing = true;

        AddPoint(mousePos);
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

    private void UpdateDrawing(Vector2 mousePos)
    {
        AddPoint(mousePos);
    }

    private void EndDrawing()
    {
        isDrawing = false;
        currentLine = null;
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

    private void EraseAtPosition(Vector2 position)
    {
        List<GameObject> linesToDestroy = new List<GameObject>();

        foreach (Transform child in transform)
        {
            if (child.gameObject == eraserVisual) continue;

            LineRenderer line = child.GetComponent<LineRenderer>();
            if (line != null)
            {
                for (int i = 0; i < line.positionCount; i++)
                {
                    Vector3 linePoint = line.GetPosition(i);
                    if (Vector2.Distance(position, linePoint) < eraserRadius)
                    {
                        linesToDestroy.Add(child.gameObject);
                        break;
                    }
                }
            }
        }

        foreach (GameObject obj in linesToDestroy)
        {
            Destroy(obj);
        }
    }

    private void ToggleEraserMode()
    {
        isEraserMode = !isEraserMode;
        UpdateEraserButtonVisual();
    }

    private void UpdateEraserButtonVisual()
    {
        if (eraserButtonImage != null)
        {
            eraserButtonImage.color = isEraserMode ? activeToolColor : inactiveToolColor;
        }
    }

    private Vector2 GetWorldMousePosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -mainCam.transform.position.z;
        return mainCam.ScreenToWorldPoint(mousePos);
    }

    public void ClearCanvas()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject != eraserVisual)
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void OnDestroy()
    {
        if (eraserToggleButton != null)
        {
            eraserToggleButton.onClick.RemoveListener(ToggleEraserMode);
        }
    }
}