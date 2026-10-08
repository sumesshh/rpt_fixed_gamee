using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameCookie : MonoBehaviour
{
    public RectTransform sarahImage; // The new Sarah image that moves first
    public RectTransform sarahTarget; // The target position for Sarah's movement

    public RectTransform[] images; // Array of UI Images (Assign 3 images in Inspector)
    public RectTransform firstPos; // Common first target position
    public RectTransform secondPos; // Common second target position
    private float moveDuration = 2f; // Time for movement
    private float scaleDuration = 2f; // Time for scaling to zero
    private float delayBetweenImages = 2f; // Delay before next image moves

    public TextMeshProUGUI displayText; // Assign the TextMeshPro UI Text in Inspector
    private string firstMessage = "She gave 3 cookies to her friend, Emma."; // First text message
    private string secondMessage = "How many cookies does Sarah have left?"; // Second text message

    public Text counterText; // Assign the UI Text in the Inspector
    public Button plusButton; // Assign the Plus Button in the Inspector
    public Button minusButton; // Assign the Minus Button in the Inspector
    private int counter = 0; // Counter value
    public UnityEvent Win;
    public UnityEvent Lose;

    void Start()
    {
        plusButton.onClick.AddListener(IncreaseCounter);
        minusButton.onClick.AddListener(DecreaseCounter);

        // Start the sequence: Move Sarah first, then images
        StartCoroutine(StartWithDelay());
    }

    IEnumerator StartWithDelay()
    {
        yield return new WaitForSeconds(2.5f);
        MoveSarahThenStartImages();
    }

    void MoveSarahThenStartImages()
    {
        // Move SarahImage first
        sarahImage.DOMove(sarahTarget.position, moveDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                displayText.text = firstMessage; // Change text when Sarah reaches target
                StartCoroutine(MoveImagesSequentially()); // Start images animation after Sarah
            });
    }

    IEnumerator MoveImagesSequentially()
    {
        for (int i = 0; i < images.Length; i++)
        {
            MoveImage(images[i]); // Move one image at a time
            yield return new WaitForSeconds(delayBetweenImages); // Wait before moving next image
        }

        // When all images have finished moving
        displayText.text = secondMessage; // Change text when all animations are complete
    }

    void MoveImage(RectTransform image)
    {
        Sequence sequence = DOTween.Sequence();

        // Move to first position
        sequence.Append(image.DOMove(firstPos.position, moveDuration).SetEase(Ease.OutQuad));

        // Move to second position while shrinking
        sequence.Append(image.DOMove(secondPos.position, moveDuration).SetEase(Ease.OutQuad))
                .Join(image.DOScale(Vector3.zero, scaleDuration).SetEase(Ease.InOutQuad)); // Shrinking effect
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

    public void CheckNumberOfCookies()
    {
        if (counter == 6)
        {
            Win.Invoke();
        }
        else
        { 
            Lose.Invoke();
        }
    }
}

