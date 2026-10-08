using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using System.Collections;

public class ImageSelectionQuiz : MonoBehaviour
{
    [System.Serializable]
    public class Option
    {
        public Image image;
        public bool isCorrect;
        public Color selectedColor = new Color(0.8f, 0.8f, 0.8f, 0.2f);
        [HideInInspector]
        public Vector3 originalScale; // Store original scale for each option
        [HideInInspector]
        public Color originalColor; // Store original color for each option
    }

    public Option[] options;
    public UnityEvent Win;
    public UnityEvent Lose;

    [Header("Animation Settings")]
    public float scaleDuration = 0.3f;
    public float scaleAmount = 1.2f;
    public Ease scaleEase = Ease.OutBack;

    [Header("Audio Feedback")]
    public AudioClip correctAnswerSound;
    public AudioClip wrongAnswerSound;
    [Range(0f, 1f)]
    public float soundVolume = 1f;

    [Header("Wrong Answer Feedback")]
    public float vibrationDuration = 0.4f;
    public float vibrationDistance = 15f;
    public int vibrationCount = 5;
    public int phoneVibrationDuration = 100;
    public float wrongAnswerResetDelay = 2f; // Delay before resetting wrong answer color

    [Header("Disabled Button Settings")]
    public Color disabledButtonColor = new Color(0.5f, 0.8f, 0.5f, 1f); // Custom color for disabled buttons
    public bool preserveDisabledButtonColor = true; // If true, will override Unity's default disabled color

    private AudioSource audioSource;
    private int totalCorrectAnswers;
    private int currentCorrectStreak;
    private bool[] correctAnswersSelected;
    private bool gameCompleted = false; // Flag to track if the game is completed

    void Start()
    {
        SetupAudioSource();
        InitializeOptions();
        SetupButtons();
    }

    private void SetupAudioSource()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void InitializeOptions()
    {
        totalCorrectAnswers = 0;
        foreach (Option option in options)
        {
            if (option.isCorrect)
            {
                totalCorrectAnswers++;
            }

            if (option.image != null)
            {
                // Store original scale
                option.originalScale = option.image.transform.localScale;

                // Store original color and ensure the image starts fully visible
                option.originalColor = option.image.color;
                Color startColor = option.originalColor;
                startColor.a = 1f;
                option.image.color = startColor;
            }
        }

        correctAnswersSelected = new bool[options.Length];
        currentCorrectStreak = 0;
        gameCompleted = false; // Reset game completed flag
    }

    private void SetupButtons()
    {
        foreach (Option option in options)
        {
            if (option.image != null)
            {
                Button btn = option.image.GetComponent<Button>();
                if (btn == null)
                {
                    btn = option.image.gameObject.AddComponent<Button>();
                }

                // Configure button colors including disabled color
                var colors = btn.colors;
                colors.fadeDuration = 0.1f;
                colors.disabledColor = disabledButtonColor; // Set custom disabled color
                btn.colors = colors;

                // Add click listener
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => {
                    // Check if game is already completed - exit if it is
                    if (gameCompleted)
                        return;

                    // Change color to selected color while preserving alpha
                    Color newColor = option.selectedColor;
                    newColor.a = 1f;
                    option.image.color = newColor;

                    if (option.isCorrect)
                    {
                        PlayClickAnimation(option);
                        PlayCorrectAnswerSound();
                    }
                    else
                    {
                        PlayWrongAnswerFeedback(option);
                        ResetOtherButtonColors(option); // Reset all other button colors
                        StartCoroutine(ResetWrongButtonColorAfterDelay(option, wrongAnswerResetDelay)); // Reset wrong button color after delay
                    }

                    CheckAnswer(option);
                });
            }
        }
    }

    // New method to reset all button colors except the specified one
    private void ResetOtherButtonColors(Option excludedOption)
    {
        foreach (Option option in options)
        {
            if (option != excludedOption && option.image != null)
            {
                // Reset to original color
                option.image.color = option.originalColor;
            }
        }
    }

    // New coroutine to reset wrong button color after delay
    private IEnumerator ResetWrongButtonColorAfterDelay(Option wrongOption, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (!gameCompleted && wrongOption.image != null)
        {
            // Reset to original color
            wrongOption.image.color = wrongOption.originalColor;
        }
    }

    private void PlayCorrectAnswerSound()
    {
        if (correctAnswerSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(correctAnswerSound, soundVolume);
        }
    }

    private void PlayClickAnimation(Option option)
    {
        Transform imageTransform = option.image.transform;
        imageTransform.DOKill();

        Vector3 targetScale = option.originalScale * scaleAmount;

        Sequence clickSequence = DOTween.Sequence();
        clickSequence.Append(imageTransform.DOScale(targetScale, scaleDuration / 2)
            .SetEase(scaleEase));
        clickSequence.Append(imageTransform.DOScale(option.originalScale, scaleDuration / 2)
            .SetEase(Ease.OutBack));
    }

    private void PlayWrongAnswerFeedback(Option option)
    {
        Transform imageTransform = option.image.transform;
        imageTransform.DOKill();

        RectTransform rectTransform = imageTransform.GetComponent<RectTransform>();
        Vector2 originalAnchoredPosition = rectTransform.anchoredPosition;

        Sequence shakeSequence = DOTween.Sequence();
        float singleVibrationDuration = vibrationDuration / (vibrationCount * 2);

        for (int i = 0; i < vibrationCount; i++)
        {
            shakeSequence.Append(rectTransform.DOAnchorPosX(
                originalAnchoredPosition.x + vibrationDistance,
                singleVibrationDuration
            ).SetEase(Ease.OutSine));

            shakeSequence.Append(rectTransform.DOAnchorPosX(
                originalAnchoredPosition.x - vibrationDistance,
                singleVibrationDuration
            ).SetEase(Ease.OutSine));
        }

        shakeSequence.Append(rectTransform.DOAnchorPosX(
            originalAnchoredPosition.x,
            singleVibrationDuration
        ).SetEase(Ease.OutSine));

        // Ensure position and scale are reset
        shakeSequence.OnComplete(() => {
            rectTransform.anchoredPosition = originalAnchoredPosition;
            imageTransform.localScale = option.originalScale;
        });

        if (wrongAnswerSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(wrongAnswerSound, soundVolume);
        }

        if (Application.isMobilePlatform)
        {
#if UNITY_ANDROID
            Handheld.Vibrate();
#elif UNITY_IOS
            //Taptic.Heavy();
#endif
        }
    }

    public void CheckAnswer(Option selectedOption)
    {
        int selectedIndex = System.Array.IndexOf(options, selectedOption);

        if (selectedOption.isCorrect)
        {
            if (!correctAnswersSelected[selectedIndex])
            {
                correctAnswersSelected[selectedIndex] = true;
                currentCorrectStreak++;

                if (currentCorrectStreak == totalCorrectAnswers)
                {
                    gameCompleted = true; // Set the game as completed
                    DisableAllButtons(); // Disable all buttons
                    Win?.Invoke();
                }
            }
        }
        else
        {
            ResetProgress();
            Lose?.Invoke();
        }
    }

    // Method to disable all buttons with custom color handling
    private void DisableAllButtons()
    {
        foreach (Option option in options)
        {
            if (option.image != null)
            {
                Button btn = option.image.GetComponent<Button>();
                if (btn != null)
                {
                    // If we want to preserve our custom color instead of Unity's default disabled color
                    if (preserveDisabledButtonColor)
                    {
                        // Manually apply our disabled color directly to the image
                        // This bypasses Unity's target graphic color multiplier for disabled state
                        option.image.color = disabledButtonColor;
                    }

                    // Disable button interactivity
                    btn.interactable = false;
                }
            }
        }
    }

    private void ResetProgress()
    {
        currentCorrectStreak = 0;
        for (int i = 0; i < correctAnswersSelected.Length; i++)
        {
            correctAnswersSelected[i] = false;
        }
    }

    private void OnDestroy()
    {
        // Kill any ongoing tweens when the object is destroyed
        foreach (Option option in options)
        {
            if (option.image != null)
            {
                option.image.transform.DOKill();
            }
        }

        // Stop all coroutines when destroyed
        StopAllCoroutines();
    }
}