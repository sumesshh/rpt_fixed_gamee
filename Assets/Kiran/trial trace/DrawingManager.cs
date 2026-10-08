using UnityEngine;
using UnityEngine.UI;

public class DrawingManager : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public PolygonCollider2D targetCollider;
    public float minDistance = 0.1f;
    public float validationThreshold = 0.6f; // Reduced for better tolerance
    public float bufferRadius = 0.2f; // Buffer to allow near-matches
    public Text resultText;

    private Vector2 previousPosition;
    private bool isDrawing;
    private int pointsInTarget;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartDrawing();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            EndDrawing();
        }
        else if (isDrawing)
        {
            ContinueDrawing();
        }
    }

    void StartDrawing()
    {
        lineRenderer.positionCount = 0;
        isDrawing = true;
        resultText.text = "";
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        AddPoint(mousePos);
    }

    void ContinueDrawing()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Vector2.Distance(mousePos, previousPosition) > minDistance)
        {
            AddPoint(mousePos);
        }
    }

    void AddPoint(Vector2 newPoint)
    {
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, newPoint);
        previousPosition = newPoint;
    }

    void EndDrawing()
    {
        isDrawing = false;
        ValidateDrawing();
    }

    void ValidateDrawing()
    {
        pointsInTarget = 0;
        int totalPoints = lineRenderer.positionCount;

        for (int i = 0; i < totalPoints; i++)
        {
            Vector3 point = lineRenderer.GetPosition(i);
            Collider2D[] colliders = Physics2D.OverlapCircleAll(point, bufferRadius);

            foreach (Collider2D col in colliders)
            {
                if (col == targetCollider)
                {
                    pointsInTarget++;
                    break;
                }
            }
        }

        float accuracy = (float)pointsInTarget / totalPoints;
        resultText.text = accuracy >= validationThreshold ? "Good Job!" : "Try Again!";
    }

    public void ClearDrawing()
    {
        lineRenderer.positionCount = 0;
        resultText.text = "";
    }
}
