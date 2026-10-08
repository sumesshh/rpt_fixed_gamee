using UnityEngine;
using DG.Tweening;

public class CoachMovementManager : MonoBehaviour
{
    [Header("Coach References")]
    public GameObject coach50;
    public GameObject coach70;
    public GameObject coach30;

    [Header("Areas and Positions")]
    public RectTransform area1;
    public Transform target1;
    public Transform target2;
    public Transform target3;
    public RectTransform area2;

    [Header("Movement Settings")]
    public float moveToTargetTime = 0.5f;
    public float fastMoveTime = 0.3f;

    private Vector3 coach50OriginalPosition;
    private bool isCoach50Moving = false;
    private bool isCoach70Moving = false;
    private bool isCoach30InArea1 = false; // New flag for coach30 in area1
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        if (coach50)
        {
            coach50OriginalPosition = coach50.transform.position;
        }
    }

    void Update()
    {
        // Check if mouse is being held down
        if (Input.GetMouseButton(0))
        {
            CheckCoach70Position();
            CheckCoach30Positions(); // New method to check both area1 and area2
        }

        // Check if mouse is released
        if (Input.GetMouseButtonUp(0))
        {
            if (isCoach50Moving && !isCoach30InArea1) // Only return if not triggered by coach30
            {
                ReturnCoach50ToOriginal();
            }
            if (isCoach70Moving && !isCoach30InArea1) // Only return if not triggered by coach30
            {
                ReturnCoach70ToOriginal();
            }
        }
    }

    void CheckCoach70Position()
    {
        if (!coach70 || !area1 || !coach50) return;

        Vector2 mousePosition = Input.mousePosition;
        Vector3[] corners = new Vector3[4];
        area1.GetWorldCorners(corners);
        Rect areaRect = new Rect(
            mainCamera.WorldToScreenPoint(corners[0]),
            mainCamera.WorldToScreenPoint(corners[2]) - mainCamera.WorldToScreenPoint(corners[0])
        );

        Vector2 coach70ScreenPos = mainCamera.WorldToScreenPoint(coach70.transform.position);
        float distanceToMouse = Vector2.Distance(mousePosition, coach70ScreenPos);

        if (distanceToMouse < 100f)
        {
            if (areaRect.Contains(mousePosition) && !isCoach50Moving && !isCoach30InArea1)
            {
                MoveCoach50ToTarget2();
            }
        }
    }

    void CheckCoach30Positions()
    {
        if (!coach30 || !area1 || !area2) return;

        Vector2 mousePosition = Input.mousePosition;
        Vector2 coach30ScreenPos = mainCamera.WorldToScreenPoint(coach30.transform.position);
        float distanceToMouse = Vector2.Distance(mousePosition, coach30ScreenPos);

        if (distanceToMouse < 100f)
        {
            // Check for area1
            Vector3[] cornersArea1 = new Vector3[4];
            area1.GetWorldCorners(cornersArea1);
            Rect area1Rect = new Rect(
                mainCamera.WorldToScreenPoint(cornersArea1[0]),
                mainCamera.WorldToScreenPoint(cornersArea1[2]) - mainCamera.WorldToScreenPoint(cornersArea1[0])
            );

            // Check for area2
            Vector3[] cornersArea2 = new Vector3[4];
            area2.GetWorldCorners(cornersArea2);
            Rect area2Rect = new Rect(
                mainCamera.WorldToScreenPoint(cornersArea2[0]),
                mainCamera.WorldToScreenPoint(cornersArea2[2]) - mainCamera.WorldToScreenPoint(cornersArea2[0])
            );

            // If coach30 enters area1
            if (area1Rect.Contains(mousePosition))
            {
                MoveCoachesForCoach30Area1();
            }
            // If coach30 enters area2
            else if (area2Rect.Contains(mousePosition) && !isCoach70Moving)
            {
                MoveCoach70ToTarget3();
            }
        }
    }

    public void MoveCoachesForCoach30Area1()
    {
        isCoach30InArea1 = true;
        isCoach50Moving = true;
        isCoach70Moving = true;

        // Move both coaches to their positions
        if (coach50 && target2)
        {
            coach50.transform.DOMove(target2.position, fastMoveTime).SetEase(Ease.InOutQuad);
        }
        if (coach70 && target3)
        {
            coach70.transform.DOMove(target3.position, fastMoveTime).SetEase(Ease.InOutQuad);
        }
    }

    void MoveCoach50ToTarget2()
    {
        if (coach50 && target2)
        {
            isCoach50Moving = true;
            coach50.transform.DOMove(target2.position, fastMoveTime).SetEase(Ease.InOutQuad);
        }
    }

    public void ReturnCoach50ToOriginal()
    {
        if (coach50)
        {
            isCoach50Moving = false;
            coach50.transform.DOMove(target1.position, moveToTargetTime).SetEase(Ease.InOutQuad);
        }
    }

    void MoveCoach70ToTarget3()
    {
        if (coach70 && target3)
        {
            isCoach70Moving = true;
            coach70.transform.DOMove(target3.position, fastMoveTime).SetEase(Ease.InOutQuad);
        }
    }

    public void ReturnCoach70ToOriginal()
    {
        if (coach70)
        {
            isCoach70Moving = false;
            coach70.transform.DOMove(target2.position, moveToTargetTime).SetEase(Ease.InOutQuad);
        }
    }
}