//attach the script to image of piggy bank
//drag and drop the canvas to the inspector
//in the inspector :add component -> rigidbody 2D -> b\change bodytype to 'kinematic'
//in the inspector :add component -> Boxcollider 2D -> check the 'is Trigger' column ( intiallty the collider may be too small.
//so make it bigger enough to cover entire image)


using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PiggyBank : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Canvas gameCanvas;
    public float shakeAmount = 10f;
    public float shakeDuration = 0.5f;

    private RectTransform rectTransform;
    private RectTransform canvasRectTransform;
    private bool isDragging = false;
    private CoinSpawner coinSpawner;
    private bool isShaking = false;
    private Vector2 minPosition;
    private Vector2 maxPosition;
    private float piggyWidth;
    private float piggyHeight;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasRectTransform = gameCanvas.GetComponent<RectTransform>();
        coinSpawner = FindObjectOfType<CoinSpawner>();

        // Calculate pig boundaries
        piggyWidth = rectTransform.rect.width * rectTransform.localScale.x / 2;
        piggyHeight = rectTransform.rect.height * rectTransform.localScale.y / 2;

        // Calculate movement boundaries
        float canvasWidth = canvasRectTransform.rect.width;
        float canvasHeight = canvasRectTransform.rect.height;

        minPosition = new Vector2(-canvasWidth / 2 + piggyWidth, -canvasHeight / 2 + piggyHeight);
        maxPosition = new Vector2(canvasWidth / 2 - piggyWidth, canvasHeight / 2 - piggyHeight);
    }

    void Update()
    {
        if (!isShaking)
        {
            HandleDragging();
        }
    }

    void HandleDragging()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(
                rectTransform,
                Input.mousePosition,
                gameCanvas.worldCamera))
            {
                transform.DOScale(1.3f, 0.5f);
                isDragging = true;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            transform.DOScale(1f, 0.5f);
            isDragging = false;
        }

        if (isDragging)
        {
            Vector2 mousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRectTransform,
                Input.mousePosition,
                gameCanvas.worldCamera,
                out mousePos
            );

            // Clamp position within canvas boundaries
            mousePos.x = Mathf.Clamp(mousePos.x, minPosition.x, maxPosition.x);
            mousePos.y = rectTransform.anchoredPosition.y; // Keep vertical position fixed

            rectTransform.anchoredPosition = mousePos;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin"))
        {
            Coin coin = other.GetComponent<Coin>();
            if (coin != null)
            {
                int coinValue = coin.GetNumber();
                if (coinSpawner.IsBiggestNumber(coinValue))
                {
                    coinSpawner.CoinCollected(coinValue);
                    coinSpawner.PlayWinSound();
                }
                else
                {
                    StartCoroutine(ShakePiggy());
                    coinSpawner.PlayLossSound();
                    coinSpawner.DestroyAllCoins();
                }
                coinSpawner.RemoveCoinFromList(other.gameObject);
                Destroy(other.gameObject);
            }
        }
    }

    IEnumerator ShakePiggy()
    {
        isShaking = true;
        Vector2 originalPosition = rectTransform.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = originalPosition.x + Random.Range(-1f, 1f) * shakeAmount;
            x = Mathf.Clamp(x, minPosition.x, maxPosition.x); // Clamp shake within boundaries
            rectTransform.anchoredPosition = new Vector2(x, originalPosition.y);

            elapsed += Time.deltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition = originalPosition;
        isShaking = false;
    }
}