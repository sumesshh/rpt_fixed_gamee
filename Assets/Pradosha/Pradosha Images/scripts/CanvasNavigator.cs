using UnityEngine;
public class CanvasNavigator : MonoBehaviour
{
    public Canvas[] canvases;
    private int currentCanvasIndex = 0;

    void Start()
    {
        ActivateCanvas(currentCanvasIndex);
    }

    public void SwitchToNextCanvas()
    {
        // Deactivate all canvases
        foreach (Canvas canvas in canvases)
        {
            canvas.gameObject.SetActive(false);
        }

        // Increment index with wrap-around
        currentCanvasIndex = (currentCanvasIndex + 1) % canvases.Length;

        // Activate only the current canvas
        canvases[currentCanvasIndex].gameObject.SetActive(true);
    }

    public void SwitchToPreviousCanvas()
    {
        // Deactivate all canvases
        foreach (Canvas canvas in canvases)
        {
            canvas.gameObject.SetActive(false);
        }

        // Decrement index with wrap-around
        currentCanvasIndex = (currentCanvasIndex - 1 + canvases.Length) % canvases.Length;

        // Activate only the current canvas
        canvases[currentCanvasIndex].gameObject.SetActive(true);
    }

    private void ActivateCanvas(int index)
    {
        canvases[index].gameObject.SetActive(true);
    }
}