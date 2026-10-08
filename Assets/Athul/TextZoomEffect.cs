using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class TextZoomEffect : MonoBehaviour
{
    public TMP_Text questionText;
    [Header("Animation Settings")]
    public float typingSpeed = 0.05f;
    public float animationDuration = 0.6f;
    public float startScale = 0.1f;
    public float bulgeScale = 1.3f;
    public float startZPosition = -300f;
    public float startYOffset = -50f;
    public float highlightBulgeScale = 1.3f;
    public float highlightAnimationDuration = 0.5f;

    private string fullQuestion;
    private string plainText;
    private bool isAnimating = false;
    private Vector3[] originalVertices;
    private List<(int charIndex, Vector3[] originalPositions)> characterData;

    private void Start()
    {
        fullQuestion = questionText.text;
        plainText = StripRichTextTags(fullQuestion);
        questionText.text = "";
        characterData = new List<(int, Vector3[])>();
        StartCoroutine(AnimateText());
    }

    private string StripRichTextTags(string input)
    {
        return System.Text.RegularExpressions.Regex.Replace(input, "<.*?>", string.Empty);
    }

    private IEnumerator AnimateText()
    {
        if (isAnimating) yield break;
        isAnimating = true;

        int currentPlainTextLength = 0;
        int fullTextPosition = 0;

        while (currentPlainTextLength < plainText.Length)
        {
            // Process tags and visible characters
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

            // Animate only the newest character
            if (currentPlainTextLength > 0 && currentPlainTextLength <= textInfo.characterCount)
            {
                int charIndex = currentPlainTextLength - 1;
                if (textInfo.characterInfo[charIndex].isVisible)
                {
                    StartCoroutine(AnimateCharacter(charIndex));
                }
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        isAnimating = false;
        StartCoroutine(AnimateHighlightedWords());
    }

    private IEnumerator AnimateCharacter(int charIndex)
    {
        TMP_TextInfo textInfo = questionText.textInfo;
        if (!textInfo.characterInfo[charIndex].isVisible) yield break;

        int materialIndex = textInfo.characterInfo[charIndex].materialReferenceIndex;
        int vertexIndex = textInfo.characterInfo[charIndex].vertexIndex;
        Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

        // Store original positions
        Vector3[] originalPositions = new Vector3[4];
        for (int i = 0; i < 4; i++)
        {
            originalPositions[i] = vertices[vertexIndex + i];
        }
        characterData.Add((charIndex, originalPositions));

        // Calculate center point
        Vector3 center = Vector3.zero;
        for (int i = 0; i < 4; i++)
        {
            center += originalPositions[i];
        }
        center /= 4f;

        // Calculate spawn position - Keep X position the same as final position
        Vector3 spawnCenter = new Vector3(center.x, center.y + startYOffset, center.z + startZPosition);

        // Initial state - Spawn at the correct X position
        for (int i = 0; i < 4; i++)
        {
            Vector3 offset = originalPositions[i] - center;
            // Keep the X offset but modify Y and Z
            Vector3 scaledOffset = new Vector3(
                offset.x,  // Keep original X offset
                offset.y * startScale,  // Scale Y
                offset.z * startScale   // Scale Z
            );
            vertices[vertexIndex + i] = spawnCenter + scaledOffset;
        }

        float elapsed = 0f;
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / animationDuration;

            // Calculate scale
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

            // Calculate position
            float zProgress = 1 - Mathf.Pow(1 - progress, 4);
            float currentZ = Mathf.Lerp(startZPosition, 0, zProgress);

            float yProgress = progress;
            float bounceOffset = Mathf.Sin(progress * Mathf.PI) * 20;
            float currentY = Mathf.Lerp(startYOffset, 0, yProgress) + (bounceOffset * (1 - progress));

            // Apply transformation while maintaining X position
            for (int i = 0; i < 4; i++)
            {
                Vector3 offset = originalPositions[i] - center;
                Vector3 newPos = center + new Vector3(
                    offset.x,  // Keep original X offset
                    offset.y * currentScale + currentY,  // Scale and offset Y
                    offset.z * currentScale + currentZ   // Scale and offset Z
                );
                vertices[vertexIndex + i] = newPos;
            }

            questionText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
            yield return null;
        }

        // Ensure final position is exactly correct
        for (int i = 0; i < 4; i++)
        {
            vertices[vertexIndex + i] = originalPositions[i];
        }
        questionText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
    }

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

    private void OnDestroy()
    {
        DOTween.Kill(gameObject);
    }
}