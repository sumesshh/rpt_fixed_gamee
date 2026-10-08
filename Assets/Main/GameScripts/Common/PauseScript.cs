using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScript : MonoBehaviour
{

    public GameObject pausePanel;
    public GameObject settingsPanel;
    public GameObject speakerCut;
    public GameObject audioManager;

    public CanvasGroup[] canvases;

    private Audio_Manager audioScript;

    // Start is called before the first frame update
    void Start()
    {
        if (audioManager != null) {
            audioScript = audioManager.GetComponent<Audio_Manager>();
        }
        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);
        if (audioScript != null)
        {
            if (audioScript.isMusicMuted)
            {
                speakerCut.SetActive(true);
            }
            else {
                speakerCut.SetActive(false);
            }

        }
        else {
            Debug.Log("audioscript not found in pause script");
        }


        canvases = FindObjectsOfType<CanvasGroup>(true);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPauseClick() {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        AudioListener.pause = true;
        foreach (CanvasGroup canvas in canvases) {

            if (canvas != null)
            {
                canvas.interactable = false;
                canvas.blocksRaycasts = false;
            }
            
        }
    
    }

    public void OnPauseClose() {
        AudioListener.pause = false;
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);
        foreach (CanvasGroup canvas in canvases)
        {
            if (canvas != null) {
                canvas.interactable = true;
                canvas.blocksRaycasts = true;
            }
            
        }
    }

    public void MuteDisplay() {
        if (audioScript == null) {
            Debug.Log("Not muted, audioscript not found");
            return;
           
        }
        audioScript.SFXMute();
        audioScript.MusicMute();
        if (audioScript.isMusicMuted)
        {
            speakerCut.SetActive(true);
            Debug.Log("Music muted");
        }
        else {
            speakerCut.SetActive(false);
            Debug.Log("Music playing");
        }
    }

    public void OpenSettings() { 
        settingsPanel.SetActive(true);
    }

    public void CloseSettings() {
        settingsPanel.SetActive(false);
    }
}
