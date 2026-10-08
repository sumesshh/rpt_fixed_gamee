using System.Collections;
using UnityEngine;

public class CanvasSwitcherWithDelay : MonoBehaviour
{
    public GameObject[] canvases; // Assign all your canvases here in the inspector
    public float timeLag = 2f; // Time delay in seconds before switching canvases
    private int currentCanvasIndex = 0;

    void Start()
    {
        // Ensure only the first canvas is active at the start
        for (int i = 0; i < canvases.Length; i++)
        {
            canvases[i].SetActive(i == 0);
        }
    }

    public void FinishGame()
    {
        StartCoroutine(SwitchToNextCanvasWithDelay());
    }

    private IEnumerator SwitchToNextCanvasWithDelay()
    {
        

        // Wait for the specified time lag
        yield return new WaitForSeconds(timeLag);

        // Deactivate the current canvas
        canvases[currentCanvasIndex].SetActive(false);

        // Move to the next canvas
        currentCanvasIndex++;

        // Activate the next canvas
        canvases[currentCanvasIndex].SetActive(true);
        Debug.Log($"Switched to Game {currentCanvasIndex + 1}");
    }
}
