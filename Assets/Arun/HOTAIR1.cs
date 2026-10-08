using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HotAir1 : MonoBehaviour
{
    [Header("Movement Settings")]
    public float targetHeight = 0.3f;
    public float moveSpeed = 0.2f;
    public float smoothTime = 2f;

    [Header("UI References")]
    public Button Above;
    public Button Below;

    [Header("Game Objects")]
    public GameObject balloon;
    public GameObject collidermountain;
    public GameObject winningCollider;

    [Header("Effects")]
    public ParticleSystem collisionEffect;
    public ParticleSystem sparkEffect;

    public UnityEvent OnWin;

    // Private variables
    private Rigidbody2D balloonRigidbody;
    private PolygonCollider2D balloonCollider; // Using PolygonCollider2D for better detection
    private EdgeCollider2D mountainCollider;
    private PolygonCollider2D winningZoneCollider;
    private bool isColliding = false;
    private Vector3 originalPos;
    private Tween currentMovementTween;
    private bool onewin = true;

    private void Awake()
    {
        // Get or add required components
        balloonRigidbody = balloon.GetComponent<Rigidbody2D>() ?? balloon.gameObject.AddComponent<Rigidbody2D>();
        balloonCollider = balloon.GetComponent<PolygonCollider2D>() ?? balloon.gameObject.AddComponent<PolygonCollider2D>();
        mountainCollider = collidermountain.GetComponent<EdgeCollider2D>();

        if (mountainCollider == null)
        {
            Debug.LogError("Mountain must have an EdgeCollider2D already configured! Please add it manually and shape it to the mountain.");
            mountainCollider = collidermountain.AddComponent<EdgeCollider2D>();
        }

        if (winningCollider != null)
        {
            winningZoneCollider = winningCollider.GetComponent<PolygonCollider2D>() ?? winningCollider.gameObject.AddComponent<PolygonCollider2D>();
            winningZoneCollider.isTrigger = true;
        }

        // Setup Rigidbody2D for balloon
        balloonRigidbody.bodyType = RigidbodyType2D.Kinematic;
        balloonRigidbody.useFullKinematicContacts = true;
        balloonRigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        balloonRigidbody.gravityScale = 0;

        // Configure balloon collider to be a proper balloon shape
        ConfigureBalloonCollider();
    }

    private void ConfigureBalloonCollider()
    {
        // Create a balloon-like shape for better collision detection
        float width = balloon.GetComponent<RectTransform>() ?
                      balloon.GetComponent<RectTransform>().rect.width * 0.5f : 0.5f;
        float height = balloon.GetComponent<RectTransform>() ?
                       balloon.GetComponent<RectTransform>().rect.height * 0.5f : 0.5f;

        // Create balloon shape - you can adjust these points to match your balloon image
        Vector2[] balloonPoints = new Vector2[]
        {
            new Vector2(0, height * 0.8f),          // Top
            new Vector2(width * 0.8f, height * 0.4f),  // Top right
            new Vector2(width * 0.6f, -height * 0.2f), // Bottom right
            new Vector2(0, -height * 0.4f),          // Bottom
            new Vector2(-width * 0.6f, -height * 0.2f), // Bottom left
            new Vector2(-width * 0.8f, height * 0.4f)   // Top left
        };
        balloonCollider.points = balloonPoints;

        // Make sure collider's physics material is bouncy to avoid getting stuck
        PhysicsMaterial2D material = new PhysicsMaterial2D();
        material.friction = 0.1f;
        material.bounciness = 0.2f;
        balloonCollider.sharedMaterial = material;
    }

    private void Start()
    {
        // Set up button listeners
        Above.onClick.AddListener(MoveUp);
        Below.onClick.AddListener(MoveDown);

        // Store original position
        originalPos = balloon.transform.position;

        // Start horizontal movement after a short delay to ensure everything is set up
        Invoke(nameof(StartHorizontalMovement), 0.1f);
    }

    private void Update()
    {
        // Check for winning condition every frame
        CheckWinningCondition();

        // Early collision detection as a backup
        if (!isColliding && mountainCollider != null)
        {
            if (Physics2D.IsTouching(balloonCollider, mountainCollider))
            {
                isColliding = true;
                HandleCollision(GetEstimatedCollisionPoint());
            }
        }
    }

    private Vector2 GetEstimatedCollisionPoint()
    {
        // Find the closest point on the edge collider to the balloon
        Vector2 closestPoint = Physics2D.ClosestPoint(
            balloon.transform.position,
            mountainCollider
        );
        return closestPoint;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (winningZoneCollider != null && collision == winningZoneCollider)
        {
            Win();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the balloon collides with the mountain
        if (collision.collider == mountainCollider)
        {
            if (!isColliding)
            {
                isColliding = true;
                Vector2 contactPoint = collision.contactCount > 0 ?
                    collision.GetContact(0).point :
                    GetEstimatedCollisionPoint();
                HandleCollision(contactPoint);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Reset collision flag when the balloon exits the collision
        if (collision.collider == mountainCollider)
        {
            isColliding = false;
        }
    }

    void MoveUp()
    {
        if (isColliding) return; // Don't move if colliding

        StopAllMovement(); // Stop current movement
        Vector3 newPosition = balloon.transform.position + new Vector3(0.2f, targetHeight, 0);

        // Move the balloon upward with smooth animation
        currentMovementTween = balloon.transform.DOMove(newPosition, smoothTime)
            .SetEase(Ease.OutSine)
            .OnComplete(() => {
                if (!isColliding) // Restart horizontal movement if not colliding
                {
                    StartHorizontalMovement();
                }
            });
    }

    void MoveDown()
    {
        if (isColliding) return; // Don't move if colliding

        StopAllMovement(); // Stop current movement
        Vector3 newPosition = balloon.transform.position + new Vector3(0.2f, -targetHeight, 0);

        // Move the balloon downward with smooth animation
        currentMovementTween = balloon.transform.DOMove(newPosition, smoothTime)
            .SetEase(Ease.OutSine)
            .OnComplete(() => {
                if (!isColliding) // Restart horizontal movement if not colliding
                {
                    StartHorizontalMovement();
                }
            });
    }

    void StartHorizontalMovement()
    {
        if (isColliding) return; // Don't start movement if colliding

        // Move the balloon horizontally with smooth animation
        currentMovementTween = balloon.transform.DOMoveX(balloon.transform.position.x + 5f, moveSpeed)
            .SetEase(Ease.Linear)
            .SetSpeedBased()
            .SetLoops(-1, LoopType.Incremental);
    }

    void StopAllMovement()
    {
        // Stop all movement by killing the current tween
        if (currentMovementTween != null && currentMovementTween.IsActive())
        {
            currentMovementTween.Kill();
        }
    }

    void HandleCollision(Vector2 contactPoint)
    {
        Debug.Log("Collision detected - stopping all movement!");

        // Stop all movement immediately
        StopAllMovement();

        // Disable buttons to prevent further input
        Above.interactable = false;
        Below.interactable = false;

        // Spawn collision particle effect
        if (collisionEffect != null)
        {
            ParticleSystem effectInstance = Instantiate(collisionEffect, contactPoint, Quaternion.identity);
            effectInstance.Play();
            Destroy(effectInstance.gameObject, effectInstance.main.duration + effectInstance.main.startLifetime.constantMax);
        }

        // Spawn spark particle effect
        if (sparkEffect != null)
        {
            ParticleSystem tempEffect = Instantiate(sparkEffect, contactPoint, Quaternion.identity);
            tempEffect.Play();
            Destroy(tempEffect.gameObject, tempEffect.main.duration + tempEffect.main.startLifetime.constantMax);
        }

        // Hide the balloon instead of destroying it
        balloon.SetActive(false);

        // Respawn the balloon after a delay
        Invoke(nameof(RespawnBalloon), 1f);
    }

    void RespawnBalloon()
    {
        Debug.Log("Respawning new balloon!");

        // Reset the balloon's position
        balloon.transform.position = originalPos;

        // Reactivate the balloon
        balloon.SetActive(true);

        // Re-enable buttons
        Above.interactable = true;
        Below.interactable = true;

        // Reset collision flag
        isColliding = false;

        // Restart horizontal movement
        StartHorizontalMovement();
    }

    void CheckWinningCondition()
    {
        if (winningCollider == null || winningZoneCollider == null) return;

        // Check if the balloon is inside the winning zone using Physics2D
        if (Physics2D.IsTouching(balloonCollider, winningZoneCollider))
        {
            Win();
        }
    }

    void Win()
    {
        if (isColliding) return; // Don't trigger win if currently handling collision



        // Stop all movement
        //StopAllMovement();
        if (onewin == true)
        {
            // Trigger the OnWin event
            OnWin.Invoke();
            Debug.Log("Winning!");

            Above.gameObject.SetActive(false);
            Below.gameObject.SetActive(false);

            onewin = false;
        }

    }

    // Draw gizmos to visualize colliders in editor (this helps with debugging)
    private void OnDrawGizmos()
    {
        if (balloon != null && balloonCollider != null)
        {
            Gizmos.color = Color.green;
            Vector2[] points = balloonCollider.points;
            for (int i = 0; i < points.Length; i++)
            {
                Vector2 worldPoint = balloon.transform.TransformPoint(points[i]);
                Vector2 nextWorldPoint = balloon.transform.TransformPoint(points[(i + 1) % points.Length]);
                Gizmos.DrawLine(worldPoint, nextWorldPoint);
            }
        }

        if (collidermountain != null && mountainCollider != null)
        {
            Gizmos.color = Color.red;
            Vector2[] points = mountainCollider.points;
            for (int i = 0; i < points.Length - 1; i++)
            {
                Vector2 worldPoint = collidermountain.transform.TransformPoint(points[i]);
                Vector2 nextWorldPoint = collidermountain.transform.TransformPoint(points[i + 1]);
                Gizmos.DrawLine(worldPoint, nextWorldPoint);
            }
        }
    }
}