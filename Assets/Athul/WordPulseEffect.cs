using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WorldPulseEffect : MonoBehaviour
{
    public TMP_Text questionText;
    public float typingSpeed = 0.15f;
    public float fallDuration = 0.5f;
    public float fallHeight = 400f;
    public AudioClip voiceOverSound;
    public float bulgeScale = 1.2f;
    public float animationDuration = 0.5f;

    private AudioSource audioSource;
    private string fullQuestion;
    private string plainText;  // Store the text without markup
    private TMP_TextInfo textInfo;
    private Vector3[] originalCharPositions;
    private Color32[] originalColors;
    private List<(int charIndex, Vector3 startPos, Vector3 endPos, float startTime)> fallingCharacters;
    private bool fallingAnimationComplete = false;
    private bool hasPlayedVoiceOver = false;

    private void Start()
    {
        // Store the full text with markup and get plain text version
        fullQuestion = questionText.text;
        plainText = StripRichTextTags(fullQuestion);
        questionText.text = "";
        questionText.ForceMeshUpdate();

        // Initialize arrays using plainText length
        originalCharPositions = new Vector3[plainText.Length];
        originalColors = new Color32[plainText.Length];
        fallingCharacters = new List<(int, Vector3, Vector3, float)>();

        SetupAudio();
        StartCoroutine(AnimateLetters());
    }

    private string StripRichTextTags(string input)
    {
        // Remove all rich text tags while preserving the actual text
        return System.Text.RegularExpressions.Regex.Replace(input, "<.*?>", string.Empty);
    }

    private IEnumerator AnimateLetters()
    {
        if (voiceOverSound != null && !hasPlayedVoiceOver)
        {
            audioSource.clip = voiceOverSound;
            audioSource.Play();
            hasPlayedVoiceOver = true;
        }

        int currentPlainTextLength = 0;
        int fullTextPosition = 0;

        while (currentPlainTextLength < plainText.Length)
        {
            // Find the next character that isn't part of a tag
            while (fullTextPosition < fullQuestion.Length)
            {
                if (fullQuestion[fullTextPosition] == '<')
                {
                    // Copy the entire tag
                    int tagEnd = fullQuestion.IndexOf('>', fullTextPosition);
                    if (tagEnd != -1)
                    {
                        questionText.text += fullQuestion.Substring(fullTextPosition, tagEnd - fullTextPosition + 1);
                        fullTextPosition = tagEnd + 1;
                    }
                }
                else
                {
                    // Add the next visible character
                    questionText.text += fullQuestion[fullTextPosition];
                    currentPlainTextLength++;
                    fullTextPosition++;
                    break;
                }
            }

            questionText.ForceMeshUpdate();
            textInfo = questionText.textInfo;

            // Process only the newest visible character
            if (currentPlainTextLength > 0 && currentPlainTextLength <= textInfo.characterCount)
            {
                int lastCharIndex = currentPlainTextLength - 1;
                if (textInfo.characterInfo[lastCharIndex].isVisible)
                {
                    // Store original position and initiate falling
                    originalCharPositions[lastCharIndex] = textInfo.characterInfo[lastCharIndex].vertex_BL.position;
                    Vector3 endPos = originalCharPositions[lastCharIndex];
                    Vector3 startPos = endPos + Vector3.up * fallHeight;
                    fallingCharacters.Add((lastCharIndex, startPos, endPos, Time.time));
                }
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        while (fallingCharacters.Count > 0)
        {
            yield return null;
        }

        fallingAnimationComplete = true;
        StartCoroutine(AnimateHighlightedWords());
    }

    // Rest of the methods remain the same...
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

    private void Update()
    {
        if (!fallingAnimationComplete && fallingCharacters.Count > 0)
        {
            for (int i = fallingCharacters.Count - 1; i >= 0; i--)
            {
                var charData = fallingCharacters[i];
                float timeSinceStart = Time.time - charData.startTime;
                float progress = timeSinceStart / fallDuration;

                if (progress >= 1f)
                {
                    UpdateCharacterPosition(charData.charIndex, charData.endPos);
                    fallingCharacters.RemoveAt(i);
                    continue;
                }

                float bounceProgress = EaseOutBounce(progress);
                Vector3 currentPos = Vector3.Lerp(charData.startPos, charData.endPos, bounceProgress);
                UpdateCharacterPosition(charData.charIndex, currentPos);
            }

            questionText.UpdateVertexData();
        }
    }

    // Include all other existing methods without changes...

    private void UpdateCharacterPosition(int charIndex, Vector3 position)
    {
        if (!textInfo.characterInfo[charIndex].isVisible) return;

        Vector3[] vertices = textInfo.meshInfo[0].vertices;
        int vertexIndex = textInfo.characterInfo[charIndex].vertexIndex;

        Vector3 offset = position - originalCharPositions[charIndex];

        vertices[vertexIndex + 0] = textInfo.characterInfo[charIndex].vertex_BL.position + offset;
        vertices[vertexIndex + 1] = textInfo.characterInfo[charIndex].vertex_TL.position + offset;
        vertices[vertexIndex + 2] = textInfo.characterInfo[charIndex].vertex_TR.position + offset;
        vertices[vertexIndex + 3] = textInfo.characterInfo[charIndex].vertex_BR.position + offset;
    }

    private IEnumerator AnimateHighlightedWords()
    {
        while (true)
        {
            float elapsed = 0f;
            while (elapsed < animationDuration / 2)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / (animationDuration / 2);
                float scale = Mathf.Lerp(1f, bulgeScale, progress);
                ScaleHighlightedWords(scale);
                yield return null;
            }

            elapsed = 0f;
            while (elapsed < animationDuration / 2)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / (animationDuration / 2);
                float scale = Mathf.Lerp(bulgeScale, 1f, progress);
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

                Vector3 center = (vertices[vertexIndex] + vertices[vertexIndex + 1] + vertices[vertexIndex + 2] + vertices[vertexIndex + 3]) / 4f;

                for (int j = 0; j < 4; j++)
                {
                    vertices[vertexIndex + j] = center + (vertices[vertexIndex + j] - center) * scale;
                }
            }
        }

        questionText.UpdateVertexData();
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
}