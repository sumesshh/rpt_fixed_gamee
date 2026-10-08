using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

public class GameDice : MonoBehaviour
{
    public Image dice1Image, dice2Image; // Assign UI Images for both dice
    public Sprite[] diceSprites; // Assign 6 sprites (indexed 0-5 for dice faces)
    public RectTransform dice1Transform, dice2Transform; // RectTransforms for animation
    public float minRollDuration = 0.5f; // Fastest roll (light shake)
    public float maxRollDuration = 2.5f; // Slowest roll (strong shake)
    public float shakeThreshold = 2.5f; // Adjust sensitivity for shake detection

    private Vector3 lastAcceleration;
    private bool isRolling = false; // Prevent multiple rolls at once

    public Button sum5Button, sum6Button, sum7Button, sum9Button; // Buttons for player input
    public UnityEvent Win; // Event triggered when the correct answer is clicked
    public UnityEvent Lose;

    private int correctSum = 0; // Stores the correct sum of the current roll

    // Possible dice values to generate correct sums
    private Dictionary<int, List<(int, int)>> sumCombinations = new Dictionary<int, List<(int, int)>>
    {
        { 5, new List<(int, int)> { (0, 3), (1, 2), (2, 1), (3, 0) } },
        { 6, new List<(int, int)> { (0, 4), (4, 0), (3, 1), (1, 3), (2, 2) } },
        { 7, new List<(int, int)> { (2, 3), (3, 2), (1, 4), (4, 1), (0, 5), (5, 0) } },
        { 9, new List<(int, int)> { (3, 4), (4, 3), (2, 5), (5, 2) } }
    };

    void Start()
    {
        lastAcceleration = Input.acceleration;

        // Assign button click listeners
        sum5Button.onClick.AddListener(() => CheckAnswer(5));
        sum6Button.onClick.AddListener(() => CheckAnswer(6));
        sum7Button.onClick.AddListener(() => CheckAnswer(7));
        sum9Button.onClick.AddListener(() => CheckAnswer(9));
    }

    void Update()
    {
        DetectShake();
    }

    public void RollDice(float rollDuration)
    {
        if (isRolling) return; // Prevent multiple rolls

        isRolling = true;

        // Select a random sum from (5,6,7,9)
        List<int> availableSums = sumCombinations.Keys.ToList();
        correctSum = availableSums[Random.Range(0, availableSums.Count)]; // Pick a random sum

        // Get a **random** dice combination for the chosen sum
        List<(int, int)> possiblePairs = sumCombinations[correctSum];
        (int dice1, int dice2) = possiblePairs[Random.Range(0, possiblePairs.Count)];

        // Play roll animation for both dice
        Sequence rollSequence = DOTween.Sequence();
        rollSequence.Append(dice1Transform.DORotate(new Vector3(0, 0, 720), rollDuration, RotateMode.FastBeyond360));
        rollSequence.Join(dice2Transform.DORotate(new Vector3(0, 0, 720), rollDuration, RotateMode.FastBeyond360));

        rollSequence.OnUpdate(() =>
        {
            // Randomly change dice faces during roll
            dice1Image.sprite = diceSprites[Random.Range(0, diceSprites.Length)];
            dice2Image.sprite = diceSprites[Random.Range(0, diceSprites.Length)];
        });

        rollSequence.OnComplete(() =>
        {
            // Set final dice faces based on the correct sum
            dice1Image.sprite = diceSprites[dice1];
            dice2Image.sprite = diceSprites[dice2];

            isRolling = false; // Allow new rolls
        });
    }

    private void CheckAnswer(int selectedSum)
    {
        if (selectedSum == correctSum)
        {
            Win.Invoke(); // Trigger Win event
        }
        else
        {
            Lose.Invoke();
        }
    }

    private void DetectShake()
    {
        Vector3 acceleration = Input.acceleration;
        float accelerationChange = (acceleration - lastAcceleration).magnitude;

        if (accelerationChange > shakeThreshold)
        {
            // Map shake intensity to roll duration
            float rollDuration = Mathf.Lerp(minRollDuration, maxRollDuration, accelerationChange / 10f);
            RollDice(rollDuration); // Trigger dice roll with dynamic duration
        }

        lastAcceleration = acceleration;
    }
}
