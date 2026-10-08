using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class StarRenderer : MonoBehaviour
{
    [Header("Required References")]
    public GameObject starPrefab;          // Your star UI image prefab
    public RectTransform moonRect;         // The moon's RectTransform
    public TextMeshProUGUI messageText;    // Text component to show the error message
    public Canvas canvas;                  // Reference to the canvas
    public Camera uiCamera;                // Reference to the UI camera (used in Screen Space - Camera mode)

    [Header("Settings")]
    public float tapRange = 100f;          // How close to the moon you need to tap
    public float messageDisplayTime = 2f;  // How long to show the error message

    public int winCount = 5;

    private int count = 0;
    public UnityEvent OnWin;
    private float messageTimer;

    private bool disableSpawn = false;


    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !disableSpawn)
        {
            // Convert screen position to world position in UI space
            Vector2 tapPosition = Input.mousePosition ;
            //RectTransformUtility.ScreenPointToLocalPointInRectangle(
            //    canvas.transform as RectTransform, Input.mousePosition, uiCamera, out tapPosition);

            // Convert moon position to local UI space
            //Vector2 moonPosition = moonRect.anchoredPosition;

            Vector2 moonScreenPosition = RectTransformUtility.WorldToScreenPoint(uiCamera, moonRect.position);

            // Calculate distance from tap to moon
            float distanceToMoon = Vector2.Distance(tapPosition, moonScreenPosition);

            if (distanceToMoon <= tapRange)
            {
                Vector2 localTapPosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, tapPosition, uiCamera, out localTapPosition);
                // Spawn star at the correct position in UI space
                SpawnStar(localTapPosition);
                count = count + 1;
                messageText.gameObject.SetActive(false);
                if (count == winCount) { 
                    OnWin.Invoke();
                    disableSpawn = true;
                } 
            }
            else
            {
                // Display error message at the tap location
                //messageText.rectTransform.anchoredPosition = tapPosition;
                //messageText.text = "Too far from the moon! Tap closer.";
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

    private void SpawnStar(Vector2 localPosition)
    {
        // Instantiate the star
        GameObject star = Instantiate(starPrefab, canvas.transform);

        // Set position in UI local space
        RectTransform starRect = star.GetComponent<RectTransform>();
        starRect.anchoredPosition = localPosition;
    }
}
