//attach the sccript to an empty game object.
//drag and drop the things mentioned.
//now we want to make the coin pass behind the game object.so select the qn text in the hierarchy.in the inspector add component -> canvas.
//check the override sorting.slect order in layer as a high value

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

public class CoinSpawner : MonoBehaviour
{
    public GameObject coinPrefab;
    public Canvas gameCanvas;
    [SerializeField] private AudioClip winSound;  
    [SerializeField] private AudioClip lossSound;  
    public UnityEvent onGameWin;
    public float initialSpawnDelay = 2f;  // Delay before first spawn

    private AudioSource audioSource;
    private RectTransform canvasRect;
    private List<GameObject> currentCoins = new List<GameObject>();
    private bool shouldSpawnNewSet = false;
    private int biggestNumber = 0;
    private bool isFirstSpawnDone = false;
    private float spawnDelay = 1f;
    private float spawnTimer = 0f;
    private int correctAnswersCount = 0;
    private bool gameCompleted = false;

    void Start()
    {
        canvasRect = gameCanvas.GetComponent<RectTransform>();
        // Add AudioSource component if not present
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Start initial spawn with delay
        StartCoroutine(DelayedFirstSpawn());
    }
    IEnumerator DelayedFirstSpawn()
    {
        yield return new WaitForSeconds(initialSpawnDelay);
        SpawnInitialSet();
    }

    void SpawnInitialSet()
    {
        if (!isFirstSpawnDone && !gameCompleted)
        {
            SpawnCoinSet();
            isFirstSpawnDone = true;
            Debug.Log("Initial set spawned");
        }
    }

    void Update()
    {
        if (gameCompleted) return;  // Don't spawn if game is won
        if (!isFirstSpawnDone) return;

        if (currentCoins.Count == 0 && !shouldSpawnNewSet)
        {
            shouldSpawnNewSet = true;
            spawnTimer = 0f;
        }

        if (shouldSpawnNewSet)
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnDelay)
            {
                SpawnCoinSet();
                shouldSpawnNewSet = false;
            }
        }
    }

    void SpawnCoinSet()
    {
        if (gameCompleted) return;

        DestroyAllCoins();
        currentCoins.Clear();

        float canvasWidth = canvasRect.rect.width;
        float spacing = canvasWidth / 5;
        float startX = -canvasWidth * 0.5f;

        HashSet<int> uniqueNumbers = new HashSet<int>();
        while (uniqueNumbers.Count < 4)
        {
            uniqueNumbers.Add(Random.Range(1, 31));
        }

        biggestNumber = uniqueNumbers.Max();
        int index = 0;

        foreach (int number in uniqueNumbers)
        {
            Vector2 spawnPosition = new Vector2(startX + (spacing * (index + 1)), canvasRect.rect.height / 2);

            GameObject coin = Instantiate(coinPrefab, gameCanvas.transform);

            // Set lower order in layer for coin
            Canvas coinCanvas = coin.GetComponent<Canvas>();
            if (coinCanvas != null)
            {
                coinCanvas.sortingOrder = -1;  // Set to lower order
            }

            coin.GetComponent<RectTransform>().anchoredPosition = spawnPosition;
            coin.GetComponent<Coin>().SetNumber(number);
            currentCoins.Add(coin);

            index++;
        }
    }
    public void PlayWinSound()
    {
        if (winSound != null)
        {
            audioSource.clip = winSound;
            audioSource.Play();
        }
    }
    public void PlayLossSound()
    {
        if (lossSound != null)
        {
            audioSource.clip = lossSound;
            audioSource.Play();
        }
    }


    public bool IsBiggestNumber(int number)
    {
        return number == biggestNumber;
    }
    public void CoinCollected(int number)
    {
        if (number == biggestNumber)
        {
            if (winSound != null)
            {
                PlayWinSound();
            }

            correctAnswersCount++;
            Debug.Log($"Correct answer! Total correct: {correctAnswersCount}");

            if (correctAnswersCount >= 4)
            {
                GameWon();
            }
            else
            {
                DestroyAllCoins();
            }
        }
    }

    void GameWon()
    {
        gameCompleted = true;
        Debug.Log("Game Won! All 4 correct answers collected!");

        // Destroy any remaining coins
        DestroyAllCoins();

        // Trigger win event
        if (onGameWin != null)
        {
            onGameWin.Invoke();
        }
    }

    public void DestroyAllCoins()
    {
        foreach (GameObject coin in currentCoins.ToArray())
        {
            if (coin != null)
            {
                Destroy(coin);
            }
        }
        currentCoins.Clear();
    }

    public void RemoveCoinFromList(GameObject coin)
    {
        if (currentCoins.Contains(coin))
        {
            currentCoins.Remove(coin);
        }
    }

    // Optional: Method to reset the game
    public void ResetGame()
    {
        correctAnswersCount = 0;
        gameCompleted = false;
        isFirstSpawnDone = false;
        shouldSpawnNewSet = false;
        DestroyAllCoins();
        SpawnInitialSet();
    }
}