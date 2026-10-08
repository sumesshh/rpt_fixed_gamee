using UnityEngine;
using TMPro;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;

public class MarginRollEffect : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI displayText;

    [Header("Animation Settings")]
    [SerializeField] private float characterDelay = 0.1f;
    [SerializeField] private float rollDuration = 0.8f;
    [SerializeField] private int numberOfRotations = 3;
    [SerializeField] private float extraRightOffset = 300f; // Distance beyond the right edge

    [Header("Highlight Bulging Settings")]
    [SerializeField] private float bulgeScale = 1.1f;
    [SerializeField] private float bulgeDuration = 0.6f;

    private void Start()
    {
        string originalText = displayText.text;
        displayText.text = "";
        StartRollingEffect(originalText);
    }

    public void StartRollingEffect(string text)
    {
        StopAllCoroutines();
        StartCoroutine(AnimateText(text));
    }

    private IEnumerator AnimateText(string fullText)
    {
        displayText.text = fullText;
        displayText.ForceMeshUpdate();

        // Calculate the right edge of the screen in local space
        Vector3[] corners = new Vector3[4];
        displayText.rectTransform.GetWorldCorners(corners);
        float rightEdge = displayText.rectTransform.rect.width + extraRightOffset;

        TMP_TextInfo textInfo = displayText.textInfo;

        // Initially hide all characters by moving them beyond the right edge
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;

            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;
            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            // Move character beyond right edge
            Vector3 offset = new Vector3(rightEdge, 0, 0);
            for (int j = 0; j < 4; j++)
            {
                vertices[vertexIndex + j] += offset;
            }
        }

        displayText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);

        // Animate each character
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;

            StartCoroutine(RollCharacter(i, rightEdge));
            yield return new WaitForSeconds(characterDelay);
        }

        // Start the bulging highlight effect after the rolling animation
        StartCoroutine(AnimateHighlightedWords());
    }

    private IEnumerator RollCharacter(int charIndex, float startX)
    {
        TMP_TextInfo textInfo = displayText.textInfo;
        if (!textInfo.characterInfo[charIndex].isVisible) yield break;

        int materialIndex = textInfo.characterInfo[charIndex].materialReferenceIndex;
        int vertexIndex = textInfo.characterInfo[charIndex].vertexIndex;
        Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

        // Get original position (final destination)
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

            // Easing function for smoother motion
            float easedProgress = 1f - Mathf.Pow(1f - progress, 3f); // Cubic ease-out

            // Update rotation (multiple complete rotations)
            currentRotation = Mathf.Lerp(0, totalRotation, easedProgress);

            // Update X position with easing
            currentX = Mathf.Lerp(startX, targetX, easedProgress);

            // Apply rotation and position
            for (int i = 0; i < 4; i++)
            {
                Vector3 vertexOffset = originalPos[i] - center;

                // Apply rotation
                float angle = currentRotation * Mathf.Deg2Rad;
                float cos = Mathf.Cos(angle);
                float sin = Mathf.Sin(angle);
                Vector3 rotatedOffset = new Vector3(
                    vertexOffset.x * cos - vertexOffset.y * sin,
                    vertexOffset.x * sin + vertexOffset.y * cos,
                    vertexOffset.z
                );

                // Set position
                vertices[vertexIndex + i] = center + rotatedOffset + new Vector3(currentX, 0, 0);
            }

            displayText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
            yield return null;
        }

        // Ensure final position is exact
        for (int i = 0; i < 4; i++)
        {
            vertices[vertexIndex + i] = originalPos[i];
        }
        displayText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
    }

    private IEnumerator AnimateHighlightedWords()
    {
        TMP_TextInfo textInfo = displayText.textInfo;
        List<int> highlightedIndices = new List<int>();

        // Collect all highlighted character indices
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;

            Color32 charColor = textInfo.characterInfo[i].color;
            if (charColor.r == 255 && charColor.g == 215 && charColor.b == 0) // #FFD700 (Gold)
            {
                highlightedIndices.Add(i);
            }
        }

        while (true)
        {
            // Bulge phase
            foreach (int charIndex in highlightedIndices)
            {
                int materialIndex = textInfo.characterInfo[charIndex].materialReferenceIndex;
                int vertexIndex = textInfo.characterInfo[charIndex].vertexIndex;
                Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

                Vector3 center = (vertices[vertexIndex] + vertices[vertexIndex + 1] + vertices[vertexIndex + 2] + vertices[vertexIndex + 3]) / 4f;

                for (int j = 0; j < 4; j++)
                {
                    vertices[vertexIndex + j] = center + (vertices[vertexIndex + j] - center) * bulgeScale;
                }
            }

            displayText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
            yield return new WaitForSeconds(bulgeDuration / 2);

            // Shrink phase
            foreach (int charIndex in highlightedIndices)
            {
                int materialIndex = textInfo.characterInfo[charIndex].materialReferenceIndex;
                int vertexIndex = textInfo.characterInfo[charIndex].vertexIndex;
                Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

                Vector3 center = (vertices[vertexIndex] + vertices[vertexIndex + 1] + vertices[vertexIndex + 2] + vertices[vertexIndex + 3]) / 4f;

                for (int j = 0; j < 4; j++)
                {
                    vertices[vertexIndex + j] = center + (vertices[vertexIndex + j] - center) / bulgeScale;
                }
            }

            displayText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
            yield return new WaitForSeconds(bulgeDuration / 2);
        }
    }

    private void OnDestroy()
    {
        DOTween.Kill(displayText.transform);
    }
}
