using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Climb : MonoBehaviour
{
    public RectTransform ladderBounds;
    public RectTransform canvasRect;
    public RectTransform Player;
    public float moveSpeed = 70f;
    public float climbSpeed = 50f;
    bool isClimbing = false;
    bool canClimb;

    public Button moveUpButton;
    public Button moveDownButton;
    public Button moveLeftButton;
    public Button moveRightButton;

    private Vector2 initialPos;

    void Start()
    {
        initialPos = transform.position;
        moveUpButton.onClick.AddListener(MoveUp);
        moveDownButton.onClick.AddListener(MoveDown);
        moveLeftButton.onClick.AddListener(MoveLeft);
        moveRightButton.onClick.AddListener(MoveRight);
    }

    void Update()
    {
        CheckLadderZone();
    }  

    public void MoveUp()
    {
        Debug.Log("moving up");
        if (canClimb)
        {
          
            isClimbing = true;
            StartClimbing(Vector3.up);
        }
    }

    public void MoveDown()
    {
        Debug.Log("moving Down");
        if (canClimb)
        {
            isClimbing = true;
            StartClimbing(Vector3.down);
        }
    }

    public void MoveLeft()
    {
        Debug.Log("moving Left");
        if (!isClimbing)
            MovePlayer(Vector2.left);
    }

    public void MoveRight()
    {
        Debug.Log("moving Right");
        if (!isClimbing)
            MovePlayer(Vector2.right);
    }

    private void MovePlayer(Vector3 direction)
    {
        Vector3 targetPosition = transform.position + (direction * moveSpeed);
        targetPosition = ClampToCanvasBounds(targetPosition);
        transform.DOMove(targetPosition, 0.5f);
    }

    void CheckLadderZone()
    {
        if (ladderBounds == null) return;

        Vector3[] worldCorners = new Vector3[4];
        ladderBounds.GetWorldCorners(worldCorners);

        Vector3 ladderMin = worldCorners[0];
        Vector3 ladderMax = worldCorners[2];
        float ladderCenter=(ladderMin.x+ladderMax.x)/2;

        Vector3 playerPos = transform.position;

        canClimb = (playerPos.x >= (ladderMin.x+ladderBounds.rect.width/4) && playerPos.x <= (ladderMax.x- ladderBounds.rect.width/4) &&
                    playerPos.y >= ladderMin.y && playerPos.y <= ladderMax.y);

    }

    private void StartClimbing(Vector3 direction)
    {
        if (isClimbing)
        {
            Vector3 targetPosition = transform.position + (direction * climbSpeed);

            if (targetPosition.y <= initialPos.y && direction == Vector3.down)
            {
                targetPosition.y = initialPos.y;
                isClimbing = false;
            }

            targetPosition = ClampToCanvasBounds(targetPosition);
            transform.DOMove(targetPosition, 0.3f);
        }
    }

    private Vector3 ClampToCanvasBounds(Vector3 position)
    {

        Vector3[] canvasCorners = new Vector3[4];
        canvasRect.GetWorldCorners(canvasCorners);

        float minX = canvasCorners[0].x;
        float maxX = canvasCorners[2].x;
        float minY = canvasCorners[0].y;
        float maxY = canvasCorners[2].y;

        position.x = Mathf.Clamp(position.x, minX + Player.rect.width, maxX - Player.rect.width);
        position.y = Mathf.Clamp(position.y, minY, maxY);

        return position;
    }
}
