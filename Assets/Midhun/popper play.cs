using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class popperplay : MonoBehaviour
{
    public ParticleSystem popper;
    public GameObject gamenext;
    public GameObject gameprev;
    

    // Reference to the coin count
    public static int coinCount = 0;
    public TMP_Text coinText; // UI Text to display the coin count

    void Start()
    {
        UpdateCoinDisplay();
    }

    void Update()
    {
    }

    public void PopperBurst()
    {
        popper.Play();
        Invoke("canvaschange", 2.0f);
        // Increase the coin count by 10 when switching canvases
        coinCount += 10;
        UpdateCoinDisplay();
    }

    public void canvaschange()
    {
        gameprev.SetActive(false);
        gamenext.SetActive(true);

       
    }

    // Update the UI to display the current coin count
    private void UpdateCoinDisplay()
    {
        if (coinText != null)
        {
            coinText.text = "" + coinCount;
        }
    }
}
