using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ReplayManager : MonoBehaviour
{
    // Reference to the canvas template in your scene (should be disabled in the hierarchy).
    public GameObject canvasTemplate;
    // Reference to the UI camera (assign in Inspector or default to Camera.main).
    public Camera uiCamera;

    // Reference to the current active canvas instance.
    private GameObject activeCanvas;
    // List to track all instantiated clones.
    private List<GameObject> clonesList = new List<GameObject>();

    private void Start()
    {
        if (uiCamera == null)
        {
            uiCamera = Camera.main;
        }
        InstantiateCanvas();
    }

    // This method resets the canvas by destroying the current instance and instantiating a new one.
    public void ResetCanvas()
    {
        if (activeCanvas != null)
        {
            Destroy(activeCanvas);
            clonesList.Remove(activeCanvas);
        }
        InstantiateCanvas();
    }

    // Instantiate a new canvas clone from the template.
    public void InstantiateCanvas()
    {
        // Instantiate a copy of the template as a child of its parent.
        activeCanvas = Instantiate(canvasTemplate, canvasTemplate.transform.parent);
        activeCanvas.SetActive(true);
        canvasTemplate.SetActive(false);
        // Add the new clone to the tracking list.
        clonesList.Add(activeCanvas);

        // Assign the UI camera to the canvas.
        AssignCameraToCanvas(activeCanvas);

        // Find the replay button in the new canvas and assign its onClick listener.
        Button replayButton = activeCanvas.GetComponentInChildren<Button>();
        if (replayButton != null)
        {
            replayButton.onClick.RemoveAllListeners();
            replayButton.onClick.AddListener(ResetCanvas);
        }
        else
        {
            Debug.LogWarning("Replay button not found in the instantiated canvas.");
        }
    }

    // Helper method to assign the camera to the canvas.
    private void AssignCameraToCanvas(GameObject canvasObject)
    {
        Canvas canvasComponent = canvasObject.GetComponent<Canvas>();
        if (canvasComponent != null)
        {
            canvasComponent.renderMode = RenderMode.ScreenSpaceCamera;
            canvasComponent.worldCamera = uiCamera;
        }
        else
        {
            Debug.LogWarning("Canvas component not found on the instantiated object.");
        }
    }

    // This function destroys every clone that has been instantiated.
    public void onWin()
    {
        // Iterate through the list of clones and destroy each one.
        foreach (GameObject clone in clonesList)
        {
            if (clone != null)
            {
                Destroy(clone);
            }
        }
        // Clear the list after destroying the clones.
        clonesList.Clear();
        activeCanvas = null;
    }
}
