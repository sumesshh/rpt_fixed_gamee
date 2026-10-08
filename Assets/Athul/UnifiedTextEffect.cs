using UnityEngine;
using TMPro;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.UI;

public class UnifiedTextEffect : MonoBehaviour
{
    public enum AnimationType
    {
        ZoomEffect,
        TypingEffect,
        MarginRoll,
        WordRain
    }

    [Header("Main Settings")]
    [SerializeField] private AnimationType selectedEffect;
    [SerializeField] private TMP_Text displayText;
    [SerializeField] private AudioClip voiceOverSound;
    [SerializeField] private float voiceOverDelay = 1f; // Added delay for voiceover playback

    [Header("Common Settings")]
    [SerializeField] private float highlightBulgeScale = 1.3f;
    [SerializeField] private float highlightAnimationDuration = 0.5f;

    [Header("Typing Speed Settings")]
    [SerializeField] private float zoomTypingSpeed = 0.05f;
    [SerializeField] private float typingEffectSpeed = 0.05f;
    [SerializeField] private float marginRollTypingSpeed = 0.1f;
    [SerializeField] private AudioClip typingSound;

    [Header("Zoom Effect Settings")]
    [SerializeField] private float zoomAnimationDuration = 0.1f;
    [SerializeField] private float startScale = 0.7f;
    [SerializeField] private float bulgeScale = 1.6f;
    [SerializeField] private float startZPosition = -500f;
    [SerializeField] private float startYOffset = -150f;

    [Header("Typing Effect Settings")]
    [SerializeField] private float typingBulgeScale = 1.1f;
    [SerializeField] private float typingBulgeDuration = 0.6f;

    [Header("Margin Roll Settings")]
    [SerializeField] private float rollDuration = 0.4f;
    [SerializeField] private int numberOfRotations = 8;
    [SerializeField] private float extraRightOffset = 300f;

    [Header("Word Rain Settings")]
    [SerializeField] private float wordRainFallDuration = 0.65f;
    [SerializeField] private float wordRainFallHeight = 200f;
    [SerializeField] private float wordRainBounceStrength = 0.5f;
    [SerializeField] private float delayBetweenWords = 0.3f;
    [SerializeField] private AudioClip dropSound;

    private AudioSource audioSource;
    private string fullText;
    private string plainText;
    private bool isAnimating = false;
    private List<(int charIndex, Vector3[] originalPositions)> characterData;
    private bool animationComplete = false;
    private Button textButton;
    private List<(int startIndex, int endIndex, Vector3[] originalPositions, bool isAnimating)> wordData;
    private bool voiceOverPlayed = false; // Added to track if voiceover has played

    private void Start()
    {
        SetupAudio();
        SetupButton();
        InitializeText();
        StartAnimation();

        // Add click detection
        displayText.gameObject.AddComponent<BoxCollider2D>();
        displayText.gameObject.AddComponent<TextClickHandler>().Initialize(PlayAudio);
    }

    private void SetupButton()
    {
        // Add Button component if it doesn't exist
        textButton = displayText.gameObject.GetComponent<Button>();
        if (textButton == null)
        {
            textButton = displayText.gameObject.AddComponent<Button>();
        }
        textButton.onClick.AddListener(OnTextClicked);
    }

    private void OnTextClicked()
    {
        PlayAudio();
    }

    private void SetupAudio()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        audioSource.clip = voiceOverSound;
    }

    private void PlayAudio()
    {
        if (voiceOverSound != null)
        {
            audioSource.Stop(); // Stop any currently playing audio
            audioSource.clip = voiceOverSound;
            audioSource.Play();
        }
    }

    private void InitializeText()
    {
        fullText = displayText.text;
        plainText = StripRichTextTags(fullText);
        displayText.text = selectedEffect == AnimationType.WordRain ? fullText : "";
        characterData = new List<(int, Vector3[])>();
        voiceOverPlayed = false; // Reset voiceover played flag

        if (selectedEffect == AnimationType.WordRain)
        {
            InitializeWordRainData();
        }
    }

    private void InitializeWordRainData()
    {
        displayText.ForceMeshUpdate();
        TMP_TextInfo textInfo = displayText.textInfo;
        wordData = new List<(int, int, Vector3[], bool)>();

        for (int i = 0; i < textInfo.wordCount; i++)
        {
            TMP_WordInfo wordInfo = textInfo.wordInfo[i];
            Vector3[] positions = new Vector3[wordInfo.characterCount * 4];

            for (int j = 0; j < wordInfo.characterCount; j++)
            {
                int charIndex = wordInfo.firstCharacterIndex + j;
                if (textInfo.characterInfo[charIndex].isVisible)
                {
                    int vertexIndex = textInfo.characterInfo[charIndex].vertexIndex;
                    positions[j * 4 + 0] = textInfo.meshInfo[0].vertices[vertexIndex + 0];
                    positions[j * 4 + 1] = textInfo.meshInfo[0].vertices[vertexIndex + 1];
                    positions[j * 4 + 2] = textInfo.meshInfo[0].vertices[vertexIndex + 2];
                    positions[j * 4 + 3] = textInfo.meshInfo[0].vertices[vertexIndex + 3];
                }
            }

            wordData.Add((wordInfo.firstCharacterIndex, wordInfo.lastCharacterIndex, positions, false));
        }

        HideAllWords();
    }

    private void HideAllWords()
    {
        Vector3[] vertices = displayText.textInfo.meshInfo[0].vertices;
        for (int i = 0; i < displayText.textInfo.characterCount; i++)
        {
            if (!displayText.textInfo.characterInfo[i].isVisible) continue;

            int vertexIndex = displayText.textInfo.characterInfo[i].vertexIndex;
            Vector3 offset = Vector3.up * 1000f;

            vertices[vertexIndex + 0] += offset;
            vertices[vertexIndex + 1] += offset;
            vertices[vertexIndex + 2] += offset;
            vertices[vertexIndex + 3] += offset;
        }

        displayText.UpdateVertexData();
    }

    private void StartAnimation()
    {
        switch (selectedEffect)
        {
            case AnimationType.ZoomEffect:
                StartCoroutine(AnimateTextZoom());
                break;
            case AnimationType.TypingEffect:
                StartCoroutine(AnimateTextTyping());
                break;
            case AnimationType.MarginRoll:
                StartCoroutine(AnimateTextMarginRoll());
                break;
            case AnimationType.WordRain:
                StartCoroutine(AnimateWordRain());
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
            PlayAudio();
            voiceOverPlayed = true;
        }
    }

    private string StripRichTextTags(string input)
    {
        return System.Text.RegularExpressions.Regex.Replace(input, "<.*?>", string.Empty);
    }

    #region Zoom Effect Animation
    private IEnumerator AnimateTextZoom()
    {
        if (isAnimating) yield break;
        isAnimating = true;

        int currentPlainTextLength = 0;
        int fullTextPosition = 0;

        while (currentPlainTextLength < plainText.Length)
        {
            while (fullTextPosition < fullText.Length)
            {
                if (fullText[fullTextPosition] == '<')
                {
                    int tagEnd = fullText.IndexOf('>', fullTextPosition);
                    if (tagEnd != -1)
                    {
                        displayText.text += fullText.Substring(fullTextPosition, tagEnd - fullTextPosition + 1);
                        fullTextPosition = tagEnd + 1;
                    }
                }
                else
                {
                    displayText.text += fullText[fullTextPosition];
                    currentPlainTextLength++;
                    fullTextPosition++;
                    break;
                }
            }

            displayText.ForceMeshUpdate();
            TMP_TextInfo textInfo = displayText.textInfo;

            if (currentPlainTextLength > 0 && currentPlainTextLength <= textInfo.characterCount)
            {
                int charIndex = currentPlainTextLength - 1;
                if (textInfo.characterInfo[charIndex].isVisible)
                {
                    StartCoroutine(AnimateZoomCharacter(charIndex));
                }
            }

            yield return new WaitForSeconds(zoomTypingSpeed);
        }

        isAnimating = false;
        // Removed PlayAudio() call since it's now triggered by delay
        StartCoroutine(AnimateHighlightedWords());
    }

    private IEnumerator AnimateZoomCharacter(int charIndex)
    {
        TMP_TextInfo textInfo = displayText.textInfo;
        if (!textInfo.characterInfo[charIndex].isVisible) yield break;

        int materialIndex = textInfo.characterInfo[charIndex].materialReferenceIndex;
        int vertexIndex = textInfo.characterInfo[charIndex].vertexIndex;
        Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

        Vector3[] originalPositions = new Vector3[4];
        for (int i = 0; i < 4; i++)
        {
            originalPositions[i] = vertices[vertexIndex + i];
        }
        characterData.Add((charIndex, originalPositions));

        Vector3 center = Vector3.zero;
        for (int i = 0; i < 4; i++)
        {
            center += originalPositions[i];
        }
        center /= 4f;

        Vector3 spawnCenter = new Vector3(center.x, center.y + startYOffset, center.z + startZPosition);

        for (int i = 0; i < 4; i++)
        {
            Vector3 offset = originalPositions[i] - center;
            Vector3 scaledOffset = new Vector3(
                offset.x,
                offset.y * startScale,
                offset.z * startScale
            );
            vertices[vertexIndex + i] = spawnCenter + scaledOffset;
        }

        float elapsed = 0f;
        while (elapsed < zoomAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / zoomAnimationDuration;

            float currentScale;
            if (progress < 0.7f)
            {
                float scaleProgress = progress / 0.7f;
                currentScale = Mathf.Lerp(startScale, bulgeScale, 1 - Mathf.Pow(1 - scaleProgress, 3));
            }
            else
            {
                float scaleDownProgress = (progress - 0.7f) / 0.3f;
                currentScale = Mathf.Lerp(bulgeScale, 1f, scaleDownProgress);
            }

            float zProgress = 1 - Mathf.Pow(1 - progress, 4);
            float currentZ = Mathf.Lerp(startZPosition, 0, zProgress);

            float yProgress = progress;
            float bounceOffset = Mathf.Sin(progress * Mathf.PI) * 20;
            float currentY = Mathf.Lerp(startYOffset, 0, yProgress) + (bounceOffset * (1 - progress));

            for (int i = 0; i < 4; i++)
            {
                Vector3 offset = originalPositions[i] - center;
                Vector3 newPos = center + new Vector3(
                    offset.x,
                    offset.y * currentScale + currentY,
                    offset.z * currentScale + currentZ
                );
                vertices[vertexIndex + i] = newPos;
            }

            displayText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
            yield return null;
        }

        for (int i = 0; i < 4; i++)
        {
            vertices[vertexIndex + i] = originalPositions[i];
        }
        displayText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
    }
    #endregion

    #region Typing Effect Animation
    private IEnumerator AnimateTextTyping()
    {
        if (isAnimating) yield break;
        isAnimating = true;

        // Start the typing sound if available
        if (audioSource != null && typingSound != null)
        {
            audioSource.clip = typingSound;
            audioSource.loop = true;
            audioSource.Play();
        }

        string currentText = "";
        int index = 0;

        while (index < fullText.Length)
        {
            if (fullText[index] == '<') // Detect the start of a rich text tag
            {
                // Extract the full tag until '>'
                while (index < fullText.Length && fullText[index] != '>')
                {
                    currentText += fullText[index];
                    index++;
                }
                // Add the closing '>'
                if (index < fullText.Length)
                {
                    currentText += fullText[index];
                    index++;
                }
            }
            else
            {
                currentText += fullText[index]; // Add one character
                index++;
            }

            displayText.text = currentText;       // Update the displayed text
            yield return new WaitForSeconds(typingEffectSpeed); // Wait before adding the next character
        }

        // Stop the typing sound after the animation completes
        if (audioSource != null && typingSound != null)
        {
            audioSource.Stop();
            audioSource.loop = false;
            audioSource.clip = voiceOverSound;
        }

        isAnimating = false;
        // Removed PlayAudio() call since it's now triggered by delay
        StartCoroutine(AnimateTypingHighlightedWords());
    }

    private IEnumerator AnimateTypingHighlightedWords()
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
            while (elapsedBulge < typingBulgeDuration / 2)
            {
                elapsedBulge += Time.deltaTime;
                float progress = elapsedBulge / (typingBulgeDuration / 2);
                float scale = Mathf.Lerp(1f, typingBulgeScale, progress);

                BulgeHighlightedCharacters(highlightedIndices, scale);

                yield return null;
            }

            // Shrink phase - return to normal size
            float elapsedShrink = 0f;
            while (elapsedShrink < typingBulgeDuration / 2)
            {
                elapsedShrink += Time.deltaTime;
                float progress = elapsedShrink / (typingBulgeDuration / 2);
                float scale = Mathf.Lerp(typingBulgeScale, 1f, progress);

                BulgeHighlightedCharacters(highlightedIndices, scale);

                yield return null;
            }
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

    #region Margin Roll Animation
    private IEnumerator AnimateTextMarginRoll()
    {
        displayText.text = fullText;
        displayText.ForceMeshUpdate();

        float rightEdge = displayText.rectTransform.rect.width + extraRightOffset;
        TMP_TextInfo textInfo = displayText.textInfo;

        // Initially hide all characters
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;

            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;
            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            Vector3 offset = new Vector3(rightEdge, 0, 0);
            for (int j = 0; j < 4; j++)
            {
                vertices[vertexIndex + j] += offset;
            }
        }

        displayText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;

            StartCoroutine(RollCharacter(i, rightEdge));
            yield return new WaitForSeconds(marginRollTypingSpeed);
        }

        // Removed PlayAudio() call since it's now triggered by delay
        StartCoroutine(AnimateHighlightedWords());
    }

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

    private IEnumerator RollCharacter(int charIndex, float startX)
    {
        TMP_TextInfo textInfo = displayText.textInfo;
        if (!textInfo.characterInfo[charIndex].isVisible) yield break;

        int materialIndex = textInfo.characterInfo[charIndex].materialReferenceIndex;
        int vertexIndex = textInfo.characterInfo[charIndex].vertexIndex;
        Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

        Vector3[] originalPos = new Vector3[4];
        Vector3 center = Vector3.zero;

        for (int i = 0; i < 4; i++)
        {
            originalPos[i] = vertices[vertexIndex + i] - new Vector3(startX, 0, 0);
            center += originalPos[i];
        }
        center /= 4f;

        float totalRotation = 360f * numberOfRotations;
        float currentRotation = 0f;
        float currentX = startX;
        float targetX = 0f;
        float elapsedTime = 0f;

        while (elapsedTime < rollDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / rollDuration;

            float easedProgress = 1f - Mathf.Pow(1f - progress, 3f);
            currentRotation = Mathf.Lerp(0, totalRotation, easedProgress);
            currentX = Mathf.Lerp(startX, targetX, easedProgress);

            for (int i = 0; i < 4; i++)
            {
                Vector3 vertexOffset = originalPos[i] - center;

                float angle = currentRotation * Mathf.Deg2Rad;
                float cos = Mathf.Cos(angle);
                float sin = Mathf.Sin(angle);
                Vector3 rotatedOffset = new Vector3(
                    vertexOffset.x * cos - vertexOffset.y * sin,
                    vertexOffset.x * sin + vertexOffset.y * cos,
                    vertexOffset.z
                );

                vertices[vertexIndex + i] = center + rotatedOffset + new Vector3(currentX, 0, 0);
            }

            displayText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
            yield return null;
        }

        for (int i = 0; i < 4; i++)
        {
            vertices[vertexIndex + i] = originalPos[i];
        }
        displayText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
    }
    #endregion

    #region Word Rain Animation
    private IEnumerator AnimateWordRain()
    {
        if (isAnimating) yield break;
        isAnimating = true;

        for (int wordIndex = 0; wordIndex < wordData.Count; wordIndex++)
        {
            StartCoroutine(AnimateWordRainWord(wordIndex));
            yield return new WaitForSeconds(delayBetweenWords);
        }

        yield return new WaitForSeconds(wordRainFallDuration);
        isAnimating = false;
        // Removed PlayAudio() call since it's now triggered by delay
        StartCoroutine(AnimateHighlightedWords());
    }

    private IEnumerator AnimateWordRainWord(int wordIndex)
    {
        var word = wordData[wordIndex];
        word.isAnimating = true;

        Vector3 startOffset = Vector3.up * wordRainFallHeight;
        float startTime = Time.time;

        while (Time.time - startTime < wordRainFallDuration)
        {
            float progress = (Time.time - startTime) / wordRainFallDuration;
            float bounceProgress = EaseOutBounce(progress);

            for (int i = word.startIndex; i <= word.endIndex; i++)
            {
                if (!displayText.textInfo.characterInfo[i].isVisible) continue;

                int vertexIndex = displayText.textInfo.characterInfo[i].vertexIndex;
                Vector3[] vertices = displayText.textInfo.meshInfo[0].vertices;

                Vector3 currentOffset = Vector3.Lerp(startOffset, Vector3.zero, bounceProgress);

                for (int j = 0; j < 4; j++)
                {
                    int baseIndex = (i - word.startIndex) * 4;
                    vertices[vertexIndex + j] = word.originalPositions[baseIndex + j] + currentOffset;
                }
            }

            displayText.UpdateVertexData();
            yield return null;
        }

        if (dropSound != null)
        {
            audioSource.PlayOneShot(dropSound, 0.5f);
        }

        SetWordToFinalPosition(wordIndex);
    }

    private void SetWordToFinalPosition(int wordIndex)
    {
        var word = wordData[wordIndex];
        Vector3[] vertices = displayText.textInfo.meshInfo[0].vertices;

        for (int i = word.startIndex; i <= word.endIndex; i++)
        {
            if (!displayText.textInfo.characterInfo[i].isVisible) continue;

            int vertexIndex = displayText.textInfo.characterInfo[i].vertexIndex;
            int baseIndex = (i - word.startIndex) * 4;

            for (int j = 0; j < 4; j++)
            {
                vertices[vertexIndex + j] = word.originalPositions[baseIndex + j];
            }
        }

        displayText.UpdateVertexData();
    }

    private float EaseOutBounce(float x)
    {
        float n1 = 7.5625f;
        float d1 = 2.75f;

        if (x < 1 / d1)
            return n1 * x * x;
        else if (x < 2 / d1)
        {
            x -= 1.5f / d1;
            return n1 * x * x + 0.75f;
        }
        else if (x < 2.5 / d1)
        {
            x -= 2.25f / d1;
            return n1 * x * x + 0.9375f;
        }
        else
        {
            x -= 2.625f / d1;
            return n1 * x * x + 0.984375f;
        }
    }
    #endregion

    #region Common Highlight Animation
    private IEnumerator AnimateHighlightedWords()
    {
        while (true)
        {
            // Bulge phase
            float elapsed = 0f;
            while (elapsed < highlightAnimationDuration / 2)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / (highlightAnimationDuration / 2);
                float scale = Mathf.Lerp(1f, highlightBulgeScale, progress);
                ScaleHighlightedWords(scale);
                yield return null;
            }

            // Shrink phase
            elapsed = 0f;
            while (elapsed < highlightAnimationDuration / 2)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / (highlightAnimationDuration / 2);
                float scale = Mathf.Lerp(highlightBulgeScale, 1f, progress);
                ScaleHighlightedWords(scale);
                yield return null;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private void ScaleHighlightedWords(float scale)
    {
        displayText.ForceMeshUpdate();
        TMP_TextInfo textInfo = displayText.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;

            Color32 charColor = textInfo.characterInfo[i].color;
            if (charColor.r == 255 && charColor.g == 215 && charColor.b == 0) // #FFD700 (Gold)
            {
                int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;
                Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

                Vector3 center = (vertices[vertexIndex] + vertices[vertexIndex + 1] +
                                vertices[vertexIndex + 2] + vertices[vertexIndex + 3]) / 4f;

                for (int j = 0; j < 4; j++)
                {
                    vertices[vertexIndex + j] = center + (vertices[vertexIndex + j] - center) * scale;
                }
            }
        }

        displayText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
    }
    #endregion

    private void OnDestroy()
    {
        if (textButton != null)
        {
            textButton.onClick.RemoveListener(OnTextClicked);
        }
        DOTween.Kill(gameObject);
        StopAllCoroutines();
    }
}