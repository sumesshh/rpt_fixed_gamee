using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class GamePattern : MonoBehaviour
{
      
    public RectTransform image; // Assign the UI Image in Inspector
    public Button rotateButton; // Assign the Button in Inspector
    private float currentRotation = 0f; // Track rotation
    public UnityEvent Win;

    void Start()
    {
        rotateButton.onClick.AddListener(Rotate90Degrees);
    }

    void Rotate90Degrees()
    {
        currentRotation -= 90f; // Increase rotation by 90 degrees
        image.DORotate(new Vector3(0, 0, currentRotation), 0.5f).SetEase(Ease.OutQuad); // Smooth rotation
    }
    public void whenCorrectPattern()
    {
        if (Mathf.Approximately(currentRotation % 360, -90f)) // Ensure it works for multiple cycles
        {
            Win.Invoke();
        }
    }
}

