using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class ShapeType
{
    public string shapeName;
    public GameObject[] shapeObjects;
    public Sprite shapeIcon;
    public AudioClip questionVoiceover; // New field for question voiceover
    [HideInInspector] public int tappedCount = 0;
}

public class ShapeTappingGame : MonoBehaviour
{
    [Header("Game Configuration")]
    [SerializeField] private ShapeType[] shapeTypes;
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private AudioClip popSound;
    [SerializeField] private float delayBetweenShapeTypes = 1.5f;
    [SerializeField] private float colliderPadding = 0.1f;

    [Header("Typewriter Effect")]
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private AudioClip typingSound;
    [SerializeField] private float typingSoundVolume = 0.5f;

    [Header("Shape Display")]
    [SerializeField] private Image shapeIconDisplay;
    [SerializeField] private Vector2 shapeIconSize = new Vector2(100f, 100f);
    [SerializeField] private float shapeAppearDelay = 0.5f;

    private AudioSource audioSource;
    private AudioSource voiceoverAudioSource; // Separate audio source for voiceovers
    private int currentShapeTypeIndex = 0;
    private bool gameActive = false;
    private Coroutine typewriterCoroutine;

    void Start()
    {
        SetupAudioSources();
        SetupShapeIconDisplay();
        SetupShapeColliders();
        StartGame();
    }

    void SetupAudioSources()
    {
        // Setup main audio source for sound effects
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Create a separate audio source for voiceovers
        voiceoverAudioSource = gameObject.AddComponent<AudioSource>();
    }


    void SetupShapeIconDisplay()
    {
        // Make sure shapeIconDisplay is assigned
        if (shapeIconDisplay == null)
        {
            Debug.LogWarning("Shape Icon Display is not assigned in the inspector. Creating one.");

            // Create a new GameObject for the shape icon if it's not assigned
            GameObject iconObject = new GameObject("ShapeIconDisplay");
            iconObject.transform.SetParent(questionText.transform.parent);

            // Position it below the question text
            RectTransform questionRect = questionText.GetComponent<RectTransform>();
            RectTransform iconRect = iconObject.AddComponent<RectTransform>();

            // Set position below question text
            iconRect.anchoredPosition = new Vector2(
                questionRect.anchoredPosition.x,
                questionRect.anchoredPosition.y - questionRect.sizeDelta.y - 20f);

            // Set size
            iconRect.sizeDelta = shapeIconSize;

            // Add Image component
            shapeIconDisplay = iconObject.AddComponent<Image>();

            // Set default values
            shapeIconDisplay.preserveAspect = true;
        }

        // Initially hide the shape icon
        if (shapeIconDisplay != null)
        {
            shapeIconDisplay.gameObject.SetActive(false);
        }
    }

    void SetupShapeColliders()
    {
        foreach (ShapeType shapeType in shapeTypes)
        {
            for (int i = 0; i < shapeType.shapeObjects.Length; i++)
            {
                if (shapeType.shapeObjects[i] != null)
                {
                    GameObject shapeObject = shapeType.shapeObjects[i];

                    AddAppropriateCollider(shapeObject);

                    ShapeIdentifier identifier = shapeObject.AddComponent<ShapeIdentifier>();
                    identifier.Initialize(shapeType.shapeName, OnShapeTapped);
                }
            }
        }
    }

    void AddAppropriateCollider(GameObject shapeObject)
    {
        // Remove any existing colliders
        Collider2D[] existingColliders = shapeObject.GetComponents<Collider2D>();
        foreach (Collider2D collider in existingColliders)
        {
            DestroyImmediate(collider);
        }

        // Check if object has a SpriteRenderer
        SpriteRenderer spriteRenderer = shapeObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && spriteRenderer.sprite != null)
        {
            // For sprite-based shapes, use PolygonCollider2D for more accurate shape matching
            PolygonCollider2D polygonCollider = shapeObject.AddComponent<PolygonCollider2D>();
            polygonCollider.isTrigger = false;
            return;
        }

        // Check if object has an Image component (UI)
        Image image = shapeObject.GetComponent<Image>();
        if (image != null)
        {
            // For UI images, use BoxCollider2D
            BoxCollider2D boxCollider = shapeObject.AddComponent<BoxCollider2D>();
            RectTransform rectTransform = shapeObject.GetComponent<RectTransform>();

            // Set the size based on RectTransform
            boxCollider.size = new Vector2(rectTransform.rect.width + colliderPadding,
                                          rectTransform.rect.height + colliderPadding);
            boxCollider.isTrigger = true;
            return;
        }

        // If no specific renderer is found, use a default BoxCollider2D
        BoxCollider2D defaultCollider = shapeObject.AddComponent<BoxCollider2D>();

        // Try to size it based on any children or parent size
        Renderer[] renderers = shapeObject.GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0)
        {
            // Calculate bounds that encompass all renderers
            Bounds bounds = new Bounds(renderers[0].bounds.center, Vector3.zero);
            foreach (Renderer renderer in renderers)
            {
                bounds.Encapsulate(renderer.bounds);
            }

            // Convert to local space
            Vector3 localCenter = shapeObject.transform.InverseTransformPoint(bounds.center);
            defaultCollider.offset = new Vector2(localCenter.x, localCenter.y);
            defaultCollider.size = new Vector2(bounds.size.x + colliderPadding, bounds.size.y + colliderPadding);
        }
        else
        {
            // Default size if no renderers found
            defaultCollider.size = new Vector2(1f + colliderPadding, 1f + colliderPadding);
        }

        defaultCollider.isTrigger = false;
    }

    void StartGame()
    {
        ResetGame();
        gameActive = true;
        SetCurrentShapeType(0);
    }

    void ResetGame()
    {
        currentShapeTypeIndex = 0;
        gameActive = false;

        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
        }

        if (shapeIconDisplay != null)
        {
            shapeIconDisplay.gameObject.SetActive(false);
        }

        foreach (ShapeType shapeType in shapeTypes)
        {
            shapeType.tappedCount = 0;
            for (int i = 0; i < shapeType.shapeObjects.Length; i++)
            {
                if (shapeType.shapeObjects[i] != null)
                {
                    shapeType.shapeObjects[i].SetActive(true);
                }
            }
        }
    }

    void SetCurrentShapeType(int index)
    {
        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
        }

        if (shapeIconDisplay != null)
        {
            shapeIconDisplay.gameObject.SetActive(false);
        }

        if (index >= shapeTypes.Length)
        {
            string completionText = "Great job! You found all shapes!";
            typewriterCoroutine = StartCoroutine(TypewriterEffect(completionText, null, null));
            gameActive = false;
            return;
        }

        currentShapeTypeIndex = index;
        string questionPrompt = "Tap on all the " + shapeTypes[index].shapeName + "s";

        Sprite currentShapeIcon = shapeTypes[index].shapeIcon;
        AudioClip questionVoiceover = shapeTypes[index].questionVoiceover;

        typewriterCoroutine = StartCoroutine(TypewriterEffect(questionPrompt, currentShapeIcon, questionVoiceover));
    }

    IEnumerator TypewriterEffect(string textToType, Sprite shapeIconToShow, AudioClip voiceoverToPlay)
    {
        questionText.text = "";

        // Play voiceover if available
        if (voiceoverToPlay != null && voiceoverAudioSource != null)
        {
            voiceoverAudioSource.clip = voiceoverToPlay;
            voiceoverAudioSource.Play();
        }

        // Wait a tiny bit before starting
        yield return new WaitForSeconds(0.2f);

        // Type each character one by one
        for (int i = 0; i < textToType.Length; i++)
        {
            // Add next character
            questionText.text += textToType[i];

            // Play typing sound if available
            if (typingSound != null)
            {
                audioSource.PlayOneShot(typingSound, typingSoundVolume);
            }

            // Wait before typing next character
            yield return new WaitForSeconds(typingSpeed);
        }

        // Show the shape icon after typing is complete
        if (shapeIconToShow != null && shapeIconDisplay != null)
        {
            yield return new WaitForSeconds(shapeAppearDelay);

            // Set the sprite and make it visible
            shapeIconDisplay.sprite = shapeIconToShow;
            shapeIconDisplay.gameObject.SetActive(true);
        }

        typewriterCoroutine = null;
    }

    public void OnShapeTapped(string shapeName, GameObject shapeObject)
    {
        if (!gameActive) return;

        if (shapeName == shapeTypes[currentShapeTypeIndex].shapeName)
        {
            if (popSound != null)
            {
                audioSource.PlayOneShot(popSound);
            }

            shapeObject.SetActive(false);

            shapeTypes[currentShapeTypeIndex].tappedCount++;

            if (shapeTypes[currentShapeTypeIndex].tappedCount >= shapeTypes[currentShapeTypeIndex].shapeObjects.Length)
            {
                StartCoroutine(MoveToNextShapeType());
            }
        }
    }

    IEnumerator MoveToNextShapeType()
    {
        // Wait for a short delay
        yield return new WaitForSeconds(delayBetweenShapeTypes);

        // Move to next shape type
        SetCurrentShapeType(currentShapeTypeIndex + 1);
    }

    // Skip the current typewriter animation
    public void SkipTypewriter()
    {
        if (typewriterCoroutine != null)
        {
            // Stop any ongoing voiceover
            if (voiceoverAudioSource != null)
            {
                voiceoverAudioSource.Stop();
            }

            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;

            // Set the full text immediately
            if (currentShapeTypeIndex < shapeTypes.Length)
            {
                questionText.text = "Tap on all the " + shapeTypes[currentShapeTypeIndex].shapeName + "s";

                // Show the shape icon immediately
                if (shapeIconDisplay != null && currentShapeTypeIndex < shapeTypes.Length)
                {
                    shapeIconDisplay.sprite = shapeTypes[currentShapeTypeIndex].shapeIcon;
                    shapeIconDisplay.gameObject.SetActive(true);
                }
            }
            else
            {
                questionText.text = "Great job! You found all shapes!";

                // Hide the shape icon at the end
                if (shapeIconDisplay != null)
                {
                    shapeIconDisplay.gameObject.SetActive(false);
                }
            }
        }
    }

#if UNITY_EDITOR
    // Helper function for debugging colliders
    public void VisualizeColliders()
    {
        foreach (ShapeType shapeType in shapeTypes)
        {
            for (int i = 0; i < shapeTypes[currentShapeTypeIndex].tappedCount; i++)
            {
                if (shapeType.shapeObjects[i] != null)
                {
                    GameObject shapeObject = shapeType.shapeObjects[i];
                    Collider2D collider = shapeObject.GetComponent<Collider2D>();
                    if (collider != null)
                    {
                        Debug.Log($"Shape: {shapeType.shapeName} - Collider type: {collider.GetType().Name}");
                    }
                }
            }
        }
    }
#endif
}

// Component to be added to each shape object
public class ShapeIdentifier : MonoBehaviour
{
    private string shapeName;
    private System.Action<string, GameObject> onTappedCallback;

    public void Initialize(string name, System.Action<string, GameObject> callback)
    {
        shapeName = name;
        onTappedCallback = callback;
    }

    void OnMouseDown()
    {
        onTappedCallback?.Invoke(shapeName, gameObject);
    }
}

// Custom editor for easier setup
#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(ShapeTappingGame))]
public class ShapeTappingGameEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        ShapeTappingGame game = (ShapeTappingGame)target;

        DrawDefaultInspector();

        GUILayout.Space(10);

        if (GUILayout.Button("Reset Game"))
        {
            game.SendMessage("ResetGame");
        }

        if (GUILayout.Button("Visualize Colliders"))
        {
            game.VisualizeColliders();
        }

        if (GUILayout.Button("Refresh Shape Colliders"))
        {
            // Access the private method using reflection (for editor only)
            System.Reflection.MethodInfo method = typeof(ShapeTappingGame).GetMethod("SetupShapeColliders",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method.Invoke(game, null);
        }

        if (GUILayout.Button("Skip Current Typewriter"))
        {
            game.SkipTypewriter();
        }
    }
}
#endif