using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;

public class LadderClimb : MonoBehaviour
{
    public RectTransform ladderBounds;
    public RectTransform canvasRect;
    public RectTransform Player;

    public float moveSpeed = .5f;
    public float climbSpeed = .5f;
    bool isClimbing = false;
    bool canClimb;
    bool won = false;


    public Button moveUpButton;
    public Button moveDownButton;
    public Button moveLeftButton;
    public Button moveRightButton;

    private Vector2 initialPos;
    private Camera cam;

    public UnityEvent Win;

    public GameObject firstText;
    public GameObject secondText;

    void Start()
    {
        cam = canvasRect.GetComponentInParent<Canvas>().worldCamera;
        initialPos = Player.transform.position;

        moveUpButton.onClick.AddListener(MoveUp);
        moveDownButton.onClick.AddListener(MoveDown);
        moveLeftButton.onClick.AddListener(MoveLeft);
        moveRightButton.onClick.AddListener(MoveRight);

        StartCoroutine(TextDisplay());


    }

    void Update()
    {
        if (!won)
        {
            CheckLadderZone();
        }
    }

    public void MoveUp()
    {
        Debug.Log("moving Up");
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
        Vector3 targetPosition = Player.transform.position + (direction * moveSpeed);
        targetPosition = ClampToCanvasBounds(targetPosition);
        Player.transform.DOMove(targetPosition, 0.5f);
    }

    void CheckLadderZone()
    {
        if (ladderBounds == null) return;

        Vector3[] worldCorners = new Vector3[4];
        ladderBounds.GetWorldCorners(worldCorners);

        // Convert to screen space for accuracy
        Vector3 minScreen = RectTransformUtility.WorldToScreenPoint(cam, worldCorners[0]);
        Vector3 maxScreen = RectTransformUtility.WorldToScreenPoint(cam, worldCorners[2]);

        Vector3 playerScreenPos = RectTransformUtility.WorldToScreenPoint(cam, Player.transform.position);

        canClimb = (playerScreenPos.x >= minScreen.x + ladderBounds.rect.width / 4 &&
                    playerScreenPos.x <= maxScreen.x - ladderBounds.rect.width / 4 &&
                    playerScreenPos.y >= minScreen.y && playerScreenPos.y <= maxScreen.y);


        if (playerScreenPos.y > maxScreen.y)
        {
            won = true;
            win();
        }
    }
    void win()
    {
        Win.Invoke();
    }

    private void StartClimbing(Vector3 direction)
    {
        if (isClimbing)
        {
            Vector3 targetPosition = Player.transform.position + (direction * climbSpeed);

            if (targetPosition.y <= initialPos.y && direction == Vector3.down)
            {
                targetPosition.y = initialPos.y;
                isClimbing = false;
            }

            targetPosition = ClampToCanvasBounds(targetPosition);
            Player.transform.DOMove(targetPosition, 0.3f);
        }
    }

    private Vector3 ClampToCanvasBounds(Vector3 position)
    {
        Vector3[] canvasCorners = new Vector3[4];
        canvasRect.GetWorldCorners(canvasCorners);

        Vector3 minScreen = RectTransformUtility.WorldToScreenPoint(cam, canvasCorners[0]);
        Vector3 maxScreen = RectTransformUtility.WorldToScreenPoint(cam, canvasCorners[2]);

        float halfWidth = Player.sizeDelta.x / 2;
        float halfHeight = Player.sizeDelta.y / 2;

        Vector3 minWorld = cam.ScreenToWorldPoint(new Vector3(minScreen.x + halfWidth, minScreen.y + halfHeight, cam.nearClipPlane));
        Vector3 maxWorld = cam.ScreenToWorldPoint(new Vector3(maxScreen.x - halfWidth, maxScreen.y - halfHeight, cam.nearClipPlane));

        position.x = Mathf.Clamp(position.x, minWorld.x, maxWorld.x);
        position.y = Mathf.Clamp(position.y, minWorld.y, maxWorld.y);

        return position;
    }

    private IEnumerator TextDisplay() {
        firstText.SetActive(true);
        secondText.SetActive(false);

        yield return new WaitForSeconds(4f);

        firstText.SetActive(false);
        secondText.SetActive(true);
    }
}
