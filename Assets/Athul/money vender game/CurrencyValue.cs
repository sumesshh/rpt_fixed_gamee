//add this script each currency and set the value fo the currency in the inspector

using UnityEngine;

public class CurrencyValue : MonoBehaviour
{
    public int value;
    public Vector3 OriginalPosition { get; set; }
    public Vector3 OriginalScale { get; set; }

    void Awake()
    {
        // Store the original local position (relative to the parent canvas)
        OriginalPosition = transform.localPosition;
        Debug.Log("Original Position Set: " + OriginalPosition);
        

        // Ensure BoxCollider2D is attached
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider2D>();
        }
        collider.isTrigger = true; // Enable Trigger

        // Ensure Rigidbody2D is attached
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.bodyType = RigidbodyType2D.Kinematic; // Set to Kinematic to avoid physics issues
    }
    public void Start()
    {
        OriginalScale = transform.localScale;
    }
    
}