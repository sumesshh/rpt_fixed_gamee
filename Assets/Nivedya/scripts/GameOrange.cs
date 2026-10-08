using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public class GameOrange : MonoBehaviour
{
    [SerializeField] private bool autoPlay = true;
    public RectTransform tortoise;  // Assign the UI Image (Tortoise)
    public RectTransform target;    // Assign the Target Image (Destination)
    public float moveDuration = 5f; // Total duration for movement
    public float stepDuration = 0.5f; // Duration for each step
    public float stepHeight = 5f; // Small vertical movement for realism
    public Text counterText; // Assign the UI Text in the Inspector
    public Button plusButton; // Assign the Plus Button in the Inspector
    public Button minusButton; // Assign the Minus Button in the Inspector
    private int counter = 0; // Counter value
    public UnityEvent Win;
    public UnityEvent Lose;

   

    void Start()
    {
        if (autoPlay) 
        {
            MoveTortoise();
        }
           
        UpdateCounterText();

        // Add button click listeners
        plusButton.onClick.AddListener(IncreaseCounter);
        minusButton.onClick.AddListener(DecreaseCounter);
    }

    public void MoveTortoise()
    {
        Sequence tortoiseSequence = DOTween.Sequence();

        // Calculate the total number of steps
        int stepCount = Mathf.FloorToInt(moveDuration / stepDuration);

        for (int i = 0; i < stepCount; i++)
        {
            float stepX = Mathf.Lerp(tortoise.position.x, target.position.x, (float)(i + 1) / stepCount);
            float stepY = (i % 2 == 0) ? tortoise.position.y + stepHeight : tortoise.position.y; // Simulating steps

            // Add movement step
            tortoiseSequence.Append(tortoise.DOMove(new Vector3(stepX, stepY, 0), stepDuration).SetEase(Ease.InOutSine));

            // Add slight rotation for head bobbing effect
            tortoiseSequence.Join(tortoise.DORotate(new Vector3(0, 0, (i % 2 == 0) ? 5f : -5f), stepDuration / 2).SetLoops(2, LoopType.Yoyo));
        }

        // Ensure final position is exactly at the target
        tortoiseSequence.Append(tortoise.DOMove(target.position, stepDuration).SetEase(Ease.OutQuad));
        tortoiseSequence.Join(tortoise.DORotate(Vector3.zero, stepDuration)); // Reset rotation
    }
    
    void IncreaseCounter()
    {
        counter++;
        UpdateCounterText();
    }

    void DecreaseCounter()
    {
        counter--;
        UpdateCounterText();
    }

    void UpdateCounterText()
    {
        counterText.text = counter.ToString();
    }
    public void CheckNumberOfOranges()
    {
        if (counter == 7)
        {
            Win.Invoke();
        }
        else
        {
            Lose.Invoke();
        }
    }
}
