using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;

public class selector : MonoBehaviour
{
    public enum TextAnimationType
    {
        ZoomEffect,
        WorldPulseEffect,
        MarginRollEffect
    }

    [Header("References")]
    public TMP_Text questionText;
    public AudioClip voiceOverSound;
    public TextAnimationType selectedAnimationType = TextAnimationType.ZoomEffect;

    [Header("Common Settings")]
    public float audioDelay = 0f;
    public float highlightBulgeScale = 1.1f;
    public float highlightAnimationDuration = 0.5f;

    [Header("Zoom Effect Settings")]
    public float zoomTypingSpeed = 0.05f;
    public float zoomAnimationDuration = 0.6f;
    public float zoomStartScale = 0.1f;
    public float zoomBulgeScale = 1.3f;
    public float zoomStartZPosition = -300f;
    public float zoomStartYOffset = -50f;

    [Header("World Pulse Effect Settings")]
    public float pulseTypingSpeed = 0.15f;
    public float pulseFallDuration = 0.5f;
    public float pulseFallHeight = 400f;

    [Header("Margin Roll Effect Settings")]
    public float rollCharacterDelay = 0.1f;
    public float rollDuration = 0.8f;
    public int rollNumberOfRotations = 3;
    public float rollExtraRightOffset = 300f;

    private AudioSource audioSource;
    private Coroutine currentAnimationCoroutine;
    private Button textButton;
    private string fullQuestion;
    private string plainText;
    private bool isAnimating = false;
    private List<(int charIndex, Vector3[] originalPositions)> characterData;
    private List<(int charIndex, Vector3 startPos, Vector3 endPos, float startTime)> worldPulseFallingCharacters;
    private bool worldPulseAnimationComplete = false;

    private void Start()
    {
        SetupAudio();
        SetupButton();
        StartTextAnimation();
    }

    private void SetupButton()
    {
        textButton = questionText.gameObject.GetComponent<Button>();
        if (textButton == null)
        {
            textButton = questionText.gameObject.AddComponent<Button>();
        }
        textButton.onClick.AddListener(OnQuestionTextClicked);
    }

    private void OnQuestionTextClicked()
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
        audioSource.loop = false;
        audioSource.volume = 1f;
    }

    public void StartTextAnimation()
    {
        if (currentAnimationCoroutine != null)
        {
            StopCoroutine(currentAnimationCoroutine);
        }

        fullQuestion = questionText.text;
        plainText = StripRichTextTags(fullQuestion);
        questionText.text = "";
        characterData = new List<(int, Vector3[])>();
        worldPulseFallingCharacters = new List<(int, Vector3, Vector3, float)>();
        worldPulseAnimationComplete = false;
        isAnimating = false;

        switch (selectedAnimationType)
        {
            case TextAnimationType.ZoomEffect:
                currentAnimationCoroutine = StartCoroutine(ZoomEffectAnimation());
                break;
            case TextAnimationType.WorldPulseEffect:
                currentAnimationCoroutine = StartCoroutine(WorldPulseEffectAnimation());
                break;
            case TextAnimationType.MarginRollEffect:
                currentAnimationCoroutine = StartCoroutine(MarginRollEffectAnimation(fullQuestion));
                break;
        }
    }

    private IEnumerator ZoomEffectAnimation()
    {
        if (isAnimating) yield break;
        isAnimating = true;

        int currentPlainTextLength = 0;
        int fullTextPosition = 0;

        while (currentPlainTextLength < plainText.Length)
        {
            while (fullTextPosition < fullQuestion.Length)
            {
                if (fullQuestion[fullTextPosition] == '<')
                {
                    int tagEnd = fullQuestion.IndexOf('>', fullTextPosition);
                    if (tagEnd != -1)
                    {
                        questionText.text += fullQuestion.Substring(fullTextPosition, tagEnd - fullTextPosition + 1);
                        fullTextPosition = tagEnd + 1;
                    }
                }
                else
                {
                    questionText.text += fullQuestion[fullTextPosition];
                    currentPlainTextLength++;
                    fullTextPosition++;
                    break;
                }
            }

            questionText.ForceMeshUpdate();
            TMP_TextInfo textInfo = questionText.textInfo;

            if (currentPlainTextLength > 0 && currentPlainTextLength <= textInfo.characterCount)
            {
                int charIndex = currentPlainTextLength - 1;
                if (textInfo.characterInfo[charIndex].isVisible)
                {
                    StartCoroutine(AnimateCharacterZoom(charIndex));
                }
            }

            yield return new WaitForSeconds(zoomTypingSpeed);
        }

        isAnimating = false;
        if (voiceOverSound != null)
        {
            StartCoroutine(PlayAudioWithDelay());
        }
        StartCoroutine(AnimateHighlightedWords());
    }

    private IEnumerator AnimateCharacterZoom(int charIndex)
    {
        TMP_TextInfo textInfo = questionText.textInfo;
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

        Vector3 spawnCenter = new Vector3(center.x, center.y + zoomStartYOffset, center.z + zoomStartZPosition);

        for (int i = 0; i < 4; i++)
        {
            Vector3 offset = originalPositions[i] - center;
            Vector3 scaledOffset = new Vector3(
                offset.x,
                offset.y * zoomStartScale,
                offset.z * zoomStartScale
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
                currentScale = Mathf.Lerp(zoomStartScale, zoomBulgeScale, 1 - Mathf.Pow(1 - scaleProgress, 3));
            }
            else
            {
                float scaleDownProgress = (progress - 0.7f) / 0.3f;
                currentScale = Mathf.Lerp(zoomBulgeScale, 1f, scaleDownProgress);
            }

            float zProgress = 1 - Mathf.Pow(1 - progress, 4);
            float currentZ = Mathf.Lerp(zoomStartZPosition, 0, zProgress);

            float yProgress = progress;
            float bounceOffset = Mathf.Sin(progress * Mathf.PI) * 20;
            float currentY = Mathf.Lerp(zoomStartYOffset, 0, yProgress) + (bounceOffset * (1 - progress));

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

            questionText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
            yield return null;
        }

        for (int i = 0; i < 4; i++)
        {
            vertices[vertexIndex + i] = originalPositions[i];
        }
        questionText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
    }

    private IEnumerator WorldPulseEffectAnimation()
    {
        int currentPlainTextLength = 0;
        int fullTextPosition = 0;

        while (currentPlainTextLength < plainText.Length)
        {
            while (fullTextPosition < fullQuestion.Length)
            {
                if (fullQuestion[fullTextPosition] == '<')
                {
                    int tagEnd = fullQuestion.IndexOf('>', fullTextPosition);
                    if (tagEnd != -1)
                    {
                        questionText.text += fullQuestion.Substring(fullTextPosition, tagEnd - fullTextPosition + 1);
                        fullTextPosition = tagEnd + 1;
                    }
                }
                else
                {
                    questionText.text += fullQuestion[fullTextPosition];
                    currentPlainTextLength++;
                    fullTextPosition++;
                    break;
                }
            }

            questionText.ForceMeshUpdate();
            TMP_TextInfo textInfo = questionText.textInfo;

            if (currentPlainTextLength > 0 && currentPlainTextLength <= textInfo.characterCount)
            {
                int lastCharIndex = currentPlainTextLength - 1;
                if (textInfo.characterInfo[lastCharIndex].isVisible)
                {
                    Vector3 endPos = textInfo.characterInfo[lastCharIndex].vertex_BL.position;
                    Vector3 startPos = endPos + Vector3.up * pulseFallHeight;
                    worldPulseFallingCharacters.Add((lastCharIndex, startPos, endPos, Time.time));
                }
            }

            yield return new WaitForSeconds(pulseTypingSpeed);
        }

        while (worldPulseFallingCharacters.Count > 0)
        {
            yield return null;
        }

        worldPulseAnimationComplete = true;
        if (voiceOverSound != null)
        {
            StartCoroutine(PlayAudioWithDelay());
        }
        StartCoroutine(AnimateHighlightedWords());
    }

    private void Update()
    {
        if (!worldPulseAnimationComplete && worldPulseFallingCharacters != null && worldPulseFallingCharacters.Count > 0)
        {
            for (int i = worldPulseFallingCharacters.Count - 1; i >= 0; i--)
            {
                var charData = worldPulseFallingCharacters[i];
                float timeSinceStart = Time.time - charData.startTime;
                float progress = timeSinceStart / pulseFallDuration;

                if (progress >= 1f)
                {
                    UpdateCharacterPosition(charData.charIndex, charData.endPos);
                    worldPulseFallingCharacters.RemoveAt(i);
                    continue;
                }

                float bounceProgress = EaseOutBounce(progress);
                Vector3 currentPos = Vector3.Lerp(charData.startPos, charData.endPos, bounceProgress);
                UpdateCharacterPosition(charData.charIndex, currentPos);
            }

            questionText.UpdateVertexData();
        }
    }

    private void UpdateCharacterPosition(int charIndex, Vector3 position)
    {
        TMP_TextInfo textInfo = questionText.textInfo;
        if (!textInfo.characterInfo[charIndex].isVisible) return;

        Vector3[] vertices = textInfo.meshInfo[0].vertices;
        int vertexIndex = textInfo.characterInfo[charIndex].vertexIndex;

        Vector3 originalPos = textInfo.characterInfo[charIndex].vertex_BL.position;
        Vector3 offset = position - originalPos;

        vertices[vertexIndex + 0] = textInfo.characterInfo[charIndex].vertex_BL.position + offset;
        vertices[vertexIndex + 1] = textInfo.characterInfo[charIndex].vertex_TL.position + offset;
        vertices[vertexIndex + 2] = textInfo.characterInfo[charIndex].vertex_TR.position + offset;
        vertices[vertexIndex + 3] = textInfo.characterInfo[charIndex].vertex_BR.position + offset;
    }

    // MarginRollEffect methods remain unchanged as they're working correctly

    private IEnumerator MarginRollEffectAnimation(string originalText)
    {
        questionText.text = originalText;
        questionText.ForceMeshUpdate();

        TMP_TextInfo textInfo = questionText.textInfo;
        float rightEdge = questionText.rectTransform.rect.width + rollExtraRightOffset;

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

        questionText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;

            StartCoroutine(RollCharacter(i, rightEdge));
            yield return new WaitForSeconds(rollCharacterDelay);
        }

        if (voiceOverSound != null)
        {
            StartCoroutine(PlayAudioWithDelay());
        }

        StartCoroutine(AnimateHighlightedWords());
    }

    private IEnumerator RollCharacter(int charIndex, float startX)
    {
        TMP_TextInfo textInfo = questionText.textInfo;
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

        float totalRotation = 360f * rollNumberOfRotations;
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

            questionText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
            yield return null;
        }

        for (int i = 0; i < 4; i++)
        {
            vertices[vertexIndex + i] = originalPos[i];
        }
        questionText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
    }

    private IEnumerator AnimateHighlightedWords()
    {
        while (true)
        {
            float elapsed = 0f;
            while (elapsed < highlightAnimationDuration / 2)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / (highlightAnimationDuration / 2);
                float scale = Mathf.Lerp(1f, highlightBulgeScale, progress);
                ScaleHighlightedWords(scale);
                yield return null;
            }

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
        questionText.ForceMeshUpdate();
        TMP_TextInfo textInfo = questionText.textInfo;

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

        questionText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
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

    private void PlayAudio()
    {
        if (voiceOverSound != null && audioSource != null)
        {
            audioSource.Stop();
            audioSource.clip = voiceOverSound;
            audioSource.Play();
        }
    }

    private IEnumerator PlayAudioWithDelay()
    {
        yield return new WaitForSeconds(audioDelay);
        PlayAudio();
    }

    private string StripRichTextTags(string input)
    {
        return System.Text.RegularExpressions.Regex.Replace(input, "<.*?>", string.Empty);
    }

    private void OnDestroy()
    {
        if (textButton != null)
        {
            textButton.onClick.RemoveListener(OnQuestionTextClicked);
        }
        DOTween.Kill(gameObject);
    }
}