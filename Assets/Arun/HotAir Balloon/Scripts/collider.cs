using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class collider : MonoBehaviour
{
    public float targetHeight = 0.5f;
    public float moveSpeed = 0.2f;
    public Button Above;
    public Button Below;

    public RectTransform balloon;
    public RectTransform collidermountain;
    public RectTransform winningCollider;

    private RectTransform rectTransform;
    private Tween horizontalTween;
    private Tween verticalTween;
    private bool isColliding = false;
    private Vector3 OriginalPos;
    public ParticleSystem collisionEffect; // Assign in Inspector
    public ParticleSystem sparkEffect;

    public UnityEvent OnWin;

    private void Start()
    {
        Above.onClick.AddListener(MoveUp);
        Below.onClick.AddListener(MoveDown);
        rectTransform = GetComponent<RectTransform>();
        StartHorizontalMovement();
        OriginalPos = balloon.transform.position;
    }

    private void Update()
    {
        // Check for collision every frame
        if (CheckRotatedCollision(rectTransform, collidermountain))
        {
            if (!isColliding)
            {
                isColliding = true;
                BalloonCollider();
            }
        }
        else
        {
            isColliding = false;
        }
        checking();
    }

    void MoveUp()
    {
        if (isColliding) return; // Don't start new movement if colliding

        StopHorizontalMovement();
        Vector3 newPosition = balloon.transform.position + new Vector3(0.5f, targetHeight, 0);
        verticalTween = balloon.transform.DOMove(newPosition, 2f)
            .SetEase(Ease.OutSine)
            .OnComplete(() => {
                if (!isColliding) // Only restart horizontal movement if not colliding
                {
                    StartHorizontalMovement();
                }
            });
    }

    void MoveDown()
    {
        if (isColliding) return; // Don't start new movement if colliding

        StopHorizontalMovement();
        Vector3 newPosition = balloon.transform.position + new Vector3(0.5f, -targetHeight, 0);
        verticalTween = balloon.transform.DOMove(newPosition, 2f)
            .SetEase(Ease.OutSine)
            .OnComplete(() => {
                if (!isColliding) // Only restart horizontal movement if not colliding
                {
                    StartHorizontalMovement();
                }
            });
    }

    void StartHorizontalMovement()
    {
        if (isColliding) return; // Don't start movement if colliding

        horizontalTween = balloon.transform.DOMoveX(balloon.transform.position.x + 0.5f, moveSpeed)
            .SetEase(Ease.Linear)
            .SetSpeedBased()
            .SetLoops(-1, LoopType.Incremental);
    }

    void StopHorizontalMovement()
    {
        if (horizontalTween != null && horizontalTween.active)
        {
            horizontalTween.Kill();
        }
    }

    void StopAllMovement()
    {
        // Kill all tweens on the balloon
        DOTween.Kill(balloon.transform);
        horizontalTween = null;
        verticalTween = null;
    }

    bool CheckRotatedCollision(RectTransform rect1, RectTransform rect2)
    {
        Vector3[] corners1 = new Vector3[4];
        Vector3[] corners2 = new Vector3[4];
        rect1.GetWorldCorners(corners1);
        rect2.GetWorldCorners(corners2);

        foreach (Vector3 corner in corners1)
        {
            if (PointInPolygon(corner, corners2)) return true;
        }
        foreach (Vector3 corner in corners2)
        {
            if (PointInPolygon(corner, corners1)) return true;
        }
        return false;
    }

    bool PointInPolygon(Vector3 point, Vector3[] polygonCorners)
    {
        int i, j;
        bool inside = false;
        int n = polygonCorners.Length;

        for (i = 0, j = n - 1; i < n; j = i++)
        {
            if (((polygonCorners[i].y > point.y) != (polygonCorners[j].y > point.y)) &&
                (point.x < (polygonCorners[j].x - polygonCorners[i].x) * (point.y - polygonCorners[i].y) /
                (polygonCorners[j].y - polygonCorners[i].y) + polygonCorners[i].x))
            {
                inside = !inside;
            }
        }
        return inside;
    }



    //private void OnDestroy()
    //{
    //    StopAllMovement();
    //    if (Above != null) Above.onClick.RemoveListener(MoveUp);
    //    if (Below != null) Below.onClick.RemoveListener(MoveDown);
    //}
    void BalloonCollider()
    {
        Debug.Log("Collision detected - stopping all movement!");

        // Stop all movement immediately
        StopAllMovement();

        // Disable buttons
        Above.interactable = false;
        Below.interactable = false;

        // Find the approximate collision point
        Vector3 collisionPoint = (balloon.position + collidermountain.position) / 2;

        // Spawn particle effects
        if (collisionEffect != null)
        {
            ParticleSystem effectInstance = Instantiate(collisionEffect, collisionPoint, Quaternion.identity);
            effectInstance.Play();
            Destroy(effectInstance.gameObject, effectInstance.main.duration + effectInstance.main.startLifetime.constantMax);
        }

        if (sparkEffect != null)
        {
            sparkEffect.transform.position = collisionPoint;
            sparkEffect.Play();

            // Create a temporary clone of the effect
            ParticleSystem tempEffect = Instantiate(sparkEffect, collisionPoint, Quaternion.identity);
            tempEffect.Play();

            // Destroy only the cloned effect after it finishes playing
            Destroy(tempEffect.gameObject, tempEffect.main.duration + tempEffect.main.startLifetime.constantMax);
        }


        // Hide the balloon instead of destroying it
        balloon.gameObject.SetActive(false);

        // Respawn the balloon after a delay
        Invoke(nameof(RespawnBalloon), 1f);
    }

    void RespawnBalloon()
    {
        Debug.Log("Respawning new balloon!");

        
        balloon.transform.position = OriginalPos;

      
        balloon.gameObject.SetActive(true);

      
        Above.interactable = true;
        Below.interactable = true;

        isColliding = false;
        StartHorizontalMovement();
    }

    void checking()
    {
        if (winningCollider == null) return;

        RectTransform rect = winningCollider.GetComponent<RectTransform>();

        Vector3[] worldCorners = new Vector3[4];
        rect.GetWorldCorners(worldCorners);

        Vector3 min = worldCorners[0]; // Bottom-left corner
        Vector3 max = worldCorners[2]; // Top-right corner

        Vector3 balloonPos = balloon.transform.position;

        // Check if the player is inside the UI area
        if (balloonPos.x >= min.x && balloonPos.x <= max.x &&
            balloonPos.y >= min.y && balloonPos.y <= max.y)
        {
            win();
        }

    }

    void win()
    {
        Debug.Log("winningg");
        StopAllMovement();
        OnWin.Invoke();
    }


}