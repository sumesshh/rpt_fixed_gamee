using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StarSpawner : MonoBehaviour
{
    [Header("Required References")]
    public GameObject starPrefab;          // Your star UI image prefab
    public RectTransform moonRect;         // The moon's RectTransform
    public TextMeshProUGUI messageText;    // Text component to show the error message


    [Header("Settings")]
    public float tapRange = 100f;          // How close to the moon you need to tap
    public float messageDisplayTime = 2f;   // How long to show the error message

    private float messageTimer;
    

    private void Update()
    {
        // Handle tap/click input
        if (Input.GetMouseButtonDown(0))
        {
            // Get the tap position
            Vector2 tapPosition = Input.mousePosition;

            // Calculate distance from tap to moon
            float distanceToMoon = Vector2.Distance(tapPosition, moonRect.position);

            if (distanceToMoon <= tapRange)
            {
                // Close enough to moon - spawn a star
                SpawnStar(tapPosition);

                // Hide any error message
                messageText.gameObject.SetActive(false);

            }
            else
            {
                // Too far from moon - show error message
                messageText.transform.position = tapPosition;
                messageText.text = "Too far from the moon! Tap closer.";
                messageText.gameObject.SetActive(true);
                messageTimer = messageDisplayTime;
            }
        }

        // Handle error message timer
        if (messageTimer > 0)
        {
            messageTimer -= Time.deltaTime;
            if (messageTimer <= 0)
            {
                messageText.gameObject.SetActive(false);
            }
        }
    }

    private void SpawnStar(Vector2 position)
    {
        // Create new star at the tap position
        GameObject star = Instantiate(starPrefab, transform);
        star.GetComponent<RectTransform>().position = position;
    }
}