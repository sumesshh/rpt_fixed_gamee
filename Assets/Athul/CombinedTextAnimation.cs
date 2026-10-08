using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CombinedTextAnimation : MonoBehaviour
{
    public enum AnimationType
    {
        SlideIn,
        DescendFromSky
    }

    [Header("Main Settings")]
    [SerializeField] private AnimationType selectedEffect;
    [SerializeField] private TMP_Text displayText;
    [SerializeField] private AudioClip voiceOverSound;
    [SerializeField] private float voiceOverDelay = 1f;

    [Header("Common Settings")]
    [SerializeField] private float highlightBulgeScale = 1.3f;
    [SerializeField] private float highlightAnimationDuration = 0.5f;

    [Header("Slide In Effect Settings")]
    [SerializeField] private float slideAnimationDuration = 0.6f;
    [SerializeField] private float startPositionX = -1000f;
    [SerializeField] private AnimationCurve slideCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private AudioClip slideSound;

    [Header("Descend From Sky Settings")]
    [SerializeField] private float descendAnimationDuration = 2.0f;
    [SerializeField] private float startPositionY = -2000f;
    [SerializeField] private float bounceHeight = 50f;
    [SerializeField] private int numBounces = 0;
    [SerializeField] private AnimationCurve fallCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private AudioClip fallSound;
    [SerializeField] private AudioClip bounceSound;

    private AudioSource effectAudioSource;
    private AudioSource voiceOverAudioSource;
    private RectTransform textRectTransform;
    private Vector3 finalPosition;
    private Vector3 startPosition;
    private bool isAnimating = false;
    private bool voiceOverPlayed = false;
    private Button textButton;
    private string fullText;

    private void Start()
    {
        SetupComponents();
        SetupText();
        StartAnimation();
    }

    private void SetupComponents()
    {
        // Get RectTransform
        textRectTransform = displayText.GetComponent<RectTransform>();

        // Store final position
        finalPosition = textRectTransform.anchoredPosition3D;

        // Setup audio sources
        SetupAudioSources();

        // Setup button for replay
        SetupButton();

        // Add click detection
        AddClickHandler();
    }

    private void SetupAudioSources()
    {
        // Set up audio source for effect sounds
        effectAudioSource = GetComponent<AudioSource>();
        if (effectAudioSource == null)
        {
            effectAudioSource = gameObject.AddComponent<AudioSource>();
        }
        effectAudioSource.playOnAwake = false;

        // Separate audio source for voice-over
        voiceOverAudioSource = gameObject.AddComponent<AudioSource>();
        voiceOverAudioSource.playOnAwake = false;
        voiceOverAudioSource.clip = voiceOverSound;
    }

    private void SetupButton()
    {
        textButton = displayText.gameObject.GetComponent<Button>();
        if (textButton == null)
        {
            textButton = displayText.gameObject.AddComponent<Button>();
        }
        textButton.onClick.AddListener(OnTextClicked);
    }

    private void AddClickHandler()
    {
        displayText.gameObject.AddComponent<BoxCollider2D>();
        displayText.gameObject.AddComponent<TextClickHandler>().Initialize(PlayVoiceOver);
    }

    private void SetupText()
    {
        fullText = displayText.text;

        // Set initial position based on the selected effect
        switch (selectedEffect)
        {
            case AnimationType.SlideIn:
                startPosition = new Vector3(startPositionX, finalPosition.y, finalPosition.z);
                break;

            case AnimationType.DescendFromSky:
                startPosition = new Vector3(finalPosition.x, startPositionY, finalPosition.z);
                break;
        }

        // Set the initial position
        textRectTransform.anchoredPosition3D = startPosition;
    }

    private void OnTextClicked()
    {
        PlayVoiceOver();
    }

    private void PlayVoiceOver()
    {
        if (voiceOverSound != null && voiceOverAudioSource != null)
        {
            voiceOverAudioSource.Stop();
            voiceOverAudioSource.Play();
            voiceOverPlayed = true;
        }
    }

    private void StartAnimation()
    {
        switch (selectedEffect)
        {
            case AnimationType.SlideIn:
                StartCoroutine(SlideInText());
                break;

            case AnimationType.DescendFromSky:
                StartCoroutine(DescendFromSky());
                break;
        }

        // Start voiceover after delay
        StartCoroutine(PlayVoiceOverAfterDelay());
    }

    private IEnumerator PlayVoiceOverAfterDelay()
    {
        yield return new WaitForSeconds(voiceOverDelay);
        if (!voiceOverPlayed)
        {
            PlayVoiceOver();
        }
    }

    #region Slide In Animation
    private IEnumerator SlideInText()
    {
        if (isAnimating) yield break;
        isAnimating = true;

        // Play the slide sound if available
        if (effectAudioSource != null && slideSound != null)
        {
            effectAudioSource.clip = slideSound;
            effectAudioSource.Play();
        }

        float startTime = Time.time;

        // Animate the position from start to finish
        while (Time.time < startTime + slideAnimationDuration)
        {
            // Calculate how far we are through the animation (0 to 1)
            float normalizedTime = (Time.time - startTime) / slideAnimationDuration;

            // Apply the animation curve for smooth motion
            float curvedTime = slideCurve.Evaluate(normalizedTime);

            // Interpolate between start and final position
            textRectTransform.anchoredPosition3D = Vector3.Lerp(startPosition, finalPosition, curvedTime);

            yield return null;
        }

        // Ensure we end exactly at the final position
        textRectTransform.anchoredPosition3D = finalPosition;
        isAnimating = false;

        // Start the bulging highlight effect
        StartCoroutine(AnimateHighlightedWords());
    }
    #endregion

    #region Descend From Sky Animation
    private IEnumerator DescendFromSky()
    {
        if (isAnimating) yield break;
        isAnimating = true;

        // Force text update to ensure proper layout
        displayText.ForceMeshUpdate();

        // Play the falling sound if available
        if (effectAudioSource != null && fallSound != null)
        {
            effectAudioSource.clip = fallSound;
            effectAudioSource.Play();
        }

        float startTime = Time.time;

        // First phase: fall from the sky
        while (Time.time < startTime + descendAnimationDuration * 0.7f) // Use 70% of time for falling
        {
            float normalizedTime = (Time.time - startTime) / (descendAnimationDuration * 0.7f);
            float curvedTime = fallCurve.Evaluate(normalizedTime);

            // Accelerate as it falls (simulating gravity)
            textRectTransform.anchoredPosition3D = Vector3.Lerp(startPosition, finalPosition, curvedTime);

            yield return null;
        }

        // Second phase: bounce a few times
        for (int bounce = 0; bounce < numBounces; bounce++)
        {
            // Play bounce sound if available
            if (effectAudioSource != null && bounceSound != null)
            {
                effectAudioSource.clip = bounceSound;
                effectAudioSource.Play();
            }

            float bounceUpDuration = 0.15f * (numBounces - bounce) / numBounces;
            float bounceDownDuration = 0.25f * (numBounces - bounce) / numBounces;

            // Current bounce height decreases with each bounce
            float currentBounceHeight = bounceHeight * (numBounces - bounce) / numBounces;

            // Bounce up
            float bounceUpStart = Time.time;
            while (Time.time < bounceUpStart + bounceUpDuration)
            {
                float t = (Time.time - bounceUpStart) / bounceUpDuration;
                float curvedT = Mathf.Sin(t * Mathf.PI * 0.5f); // Using sine curve for smooth start

                Vector3 position = finalPosition;
                position.y = finalPosition.y + curvedT * currentBounceHeight;
                textRectTransform.anchoredPosition3D = position;

                yield return null;
            }

            // Bounce down
            float bounceDownStart = Time.time;
            while (Time.time < bounceDownStart + bounceDownDuration)
            {
                float t = (Time.time - bounceDownStart) / bounceDownDuration;
                float curvedT = Mathf.Sin(t * Mathf.PI * 0.5f + Mathf.PI * 0.5f); // Cosine curve for smooth end

                Vector3 position = finalPosition;
                position.y = finalPosition.y + (1 - curvedT) * currentBounceHeight;
                textRectTransform.anchoredPosition3D = position;

                yield return null;
            }
        }

        // Ensure we end exactly at the final position
        textRectTransform.anchoredPosition3D = finalPosition;
        isAnimating = false;

        // Start the bulging highlight effect
        StartCoroutine(AnimateHighlightedWords());
    }
    #endregion

    #region Highlight Animation
    private IEnumerator AnimateHighlightedWords()
    {
        displayText.ForceMeshUpdate();
        TMP_TextInfo textInfo = displayText.textInfo;
        List<int> highlightedIndices = new List<int>();

        // Collect all highlighted character indices - looking for gold color (#FFD700)
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;

            Color32 charColor = textInfo.characterInfo[i].color;
            if (charColor.r == 255 && charColor.g == 215 && charColor.b == 0) // #FFD700 (Gold)
            {
                highlightedIndices.Add(i);
            }
        }

        // If no highlighted characters found, exit the coroutine
        if (highlightedIndices.Count == 0)
        {
            yield break;
        }

        // Start the continuous bulging animation loop
        while (true)
        {
            // Bulge phase - scale up
            float elapsedBulge = 0f;
            while (elapsedBulge < highlightAnimationDuration / 2)
            {
                elapsedBulge += Time.deltaTime;
                float progress = elapsedBulge / (highlightAnimationDuration / 2);
                float scale = Mathf.Lerp(1f, highlightBulgeScale, progress);

                BulgeHighlightedCharacters(highlightedIndices, scale);

                yield return null;
            }

            // Shrink phase - return to normal size
            float elapsedShrink = 0f;
            while (elapsedShrink < highlightAnimationDuration / 2)
            {
                elapsedShrink += Time.deltaTime;
                float progress = elapsedShrink / (highlightAnimationDuration / 2);
                float scale = Mathf.Lerp(highlightBulgeScale, 1f, progress);

                BulgeHighlightedCharacters(highlightedIndices, scale);

                yield return null;
            }

            // Small pause between cycles
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void BulgeHighlightedCharacters(List<int> highlightedIndices, float scale)
    {
        displayText.ForceMeshUpdate();
        TMP_TextInfo textInfo = displayText.textInfo;

        foreach (int charIndex in highlightedIndices)
        {
            if (charIndex >= textInfo.characterCount) continue;
            if (!textInfo.characterInfo[charIndex].isVisible) continue;

            int materialIndex = textInfo.characterInfo[charIndex].materialReferenceIndex;
            int vertexIndex = textInfo.characterInfo[charIndex].vertexIndex;

            // Skip if the vertex data is not available
            if (materialIndex >= textInfo.meshInfo.Length) continue;

            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            // Calculate center of the character
            Vector3 center = (vertices[vertexIndex] + vertices[vertexIndex + 1] +
                              vertices[vertexIndex + 2] + vertices[vertexIndex + 3]) / 4f;

            // Scale vertices from center
            for (int j = 0; j < 4; j++)
            {
                vertices[vertexIndex + j] = center + (vertices[vertexIndex + j] - center) * scale;
            }
        }

        displayText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
    }
    #endregion

    private class TextClickHandler : MonoBehaviour
    {
        private System.Action onClickCallback;

        public void Initialize(System.Action callback)
        {
            onClickCallback = callback;
        }

        private void OnMouseDown()
        {
            onClickCallback?.Invoke();
        }
    }

    private void OnDestroy()
    {
        if (textButton != null)
        {
            textButton.onClick.RemoveListener(OnTextClicked);
        }
        StopAllCoroutines();
    }
}