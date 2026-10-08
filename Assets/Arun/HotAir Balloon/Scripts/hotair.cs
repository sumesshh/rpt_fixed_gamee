
    using UnityEngine;
    using UnityEngine.UI;
    using DG.Tweening;

public class hotair : MonoBehaviour
{

    //public float floatStrength = 0.5f; // Floating effect strength
    //public float floatSpeed = 0.2f;      // Speed of floating effect
    //public float windSpeed = 0f;     // Horizontal drifting speed

    //private float baseY;  // Starting Y position for floating effect

    //void Start()
    //{
    //    baseY = transform.position.y;
    //}

    //void Update()
    //{
    //    // Floating effect (up & down)
    //    float floatOffset = Mathf.Sin(Time.time * floatSpeed) * floatStrength;
    //    transform.position = new Vector3(transform.position.x + (windSpeed * Time.deltaTime), baseY + floatOffset, 0f);
    //}


    public float targetHeight = 1f;        // Target height for the balloon
    public float horizontalSpeed = 0.1f;     // Units per second for horizontal movement
    public Button Above;                   // Button to trigger the movement
    public RectTransform balloon;          // Reference to the balloon's RectTransform

    private Tween horizontalTween;         // Reference to store horizontal movement tween
    private Tween verticalTween;           // Reference to store vertical movement tween
    private bool isMovingUp = false;       // To track if the balloon is moving up

    private void Start()
    {
        Above.onClick.AddListener(move_UP);
        StartHorizontalMovement();
    }

    private void OnDestroy()
    {
        // Clean up tweens when the object is destroyed
        horizontalTween?.Kill();
        verticalTween?.Kill();
    }

    void StartHorizontalMovement()
    {
        // Kill any existing horizontal movement
        horizontalTween?.Kill();

        // Calculate target position for horizontal movement
        Vector3 currentPos = balloon.transform.position;
        Vector3 targetPos = currentPos + Vector3.right * 10f; // Move 10 units right

        // Create infinite horizontal movement
        horizontalTween = balloon.transform
            .DOMoveX(targetPos.x, 10f / horizontalSpeed)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                // When complete, start a new horizontal movement from current position
                StartHorizontalMovement();
            });
    }

    void move_UP()
    {
        // Prevent multiple clicks during animation
        if (isMovingUp) return;

        isMovingUp = true;

        // Kill existing horizontal movement
        horizontalTween?.Kill();

        // Calculate new position (up and slightly forward)
        Vector3 currentPos = balloon.transform.position;
        Vector3 newLocation = new Vector3(
            currentPos.x + 0.5f,
            currentPos.y + targetHeight,
            currentPos.z
        );

        // Create vertical movement sequence
        verticalTween = balloon.transform
            .DOMove(newLocation, 2f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                isMovingUp = false;
                StartHorizontalMovement();
            });
    }

}


