using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using JetBrains.Annotations;
using System;

public class PanelTestScript : MonoBehaviour
{
    // Start is called before the first frame update
    //Attach the title screen - Above and under etc
    public Canvas titleScreen;
    //Attach the game canvases
    public Canvas[] games;
    public GameObject popperManager;
    //Attach the commoncanvas which is common for all games and includes the pause button and such
    public Canvas CommonCanvas;
    //ScoreObj which contains a score scirpt
    public GameObject scoreObj;
    //THe pause button which contains pause script
    public GameObject pauseObj;
    public Button replay;
    public GameObject[] demoCanvases;

    public GameObject winningPanel;
    public bool goToPlay = false;




    private GameObject ReplayManagerObj;
    private ReplayManager replayScript;

    public class ParticleAudioPair
    {
        public ParticleSystem particlesType;      // Name of the particle system (child of the parent)
        public AudioClip audioClip;      // Audio clip directly assigned in the Inspector
    }

    // Reference to the canvas template in your scene (should be disabled in the hierarchy).
    private GameObject canvasTemplate;
    // Reference to the UI camera (assign in Inspector or default to Camera.main).
    private Camera uiCamera;

    // Reference to the current active canvas instance.
    public GameObject activeCanvas;
    // List to track all instantiated clones.
    private List<GameObject> clonesList = new List<GameObject>();

    private int count = 0;
    private PopperManagerNew popperScript;
    private ScoreSaver scoreScript;
    private PauseScript pauseScript;



    private void Awake()
    {

        uiCamera = Camera.main;
        canvasTemplate = games[0].gameObject;
        if (titleScreen != null)
        {
            //Activats Title screen and adds delay to switch to game
            titleScreen.gameObject.SetActive(true);
            StartCoroutine(Delay());
        }
        else
        {

            //Activates the common canvas
            CommonCanvas.gameObject.SetActive(true);
            //Make clone
            InstantiateCanvas();

            //Find sliders, speaker button in the scene and attaches it to the corresponding fields in the dont destroy audio common gameobject
            if (AudioCommon.Instance != null)
            {
                AudioCommon.Instance.Searcher();
            }


        }
        //Attach the corresponding scripts to the script variables
        if (ReplayManagerObj != null)
        {
            replayScript = ReplayManagerObj.GetComponent<ReplayManager>();
        }
        if (pauseObj != null)
        {
            pauseScript = pauseObj.GetComponent<PauseScript>();
        }
    }
    void Start()
    {
        popperScript = popperManager.GetComponent<PopperManagerNew>();
        if (scoreObj != null)
        {
            scoreScript = scoreObj.GetComponent<ScoreSaver>();
        }


    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(2f);

        //Deactivates titlescreen and starts the first game
        titleScreen.gameObject.SetActive(false);

        if (demoCanvases != null)
        {
            for (int i = 0; i < demoCanvases.Length; i++)
            {
                demoCanvases[i].SetActive(true);
                yield return new WaitForSeconds(3f);
                demoCanvases[i].SetActive(false);
            }
        }
        games[count].gameObject.SetActive(true);
        CommonCanvas.gameObject.SetActive(true);

        InstantiateCanvas();
        if (AudioCommon.Instance != null)
        {
            AudioCommon.Instance.Searcher();
        }

    }

    private void UpdateElements()
    {
        //if all the games have been played, return to game select scene
        if (count == games.Length)
        {
            if (AudioCommon.Instance != null)
            {
                AudioCommon.Instance.Forget();
            }
            if (goToPlay)
            {
                SceneManager.LoadScene("PlayGame");
                return;
            }
            //The below function decides which canvas to display in the game select scene
            ChooseCanvas();
            Debug.Log("Activate panel");
            winningPanel.SetActive(true);
            PortraitButtonClicked();

            //SceneManager.LoadScene("Level selection portrait");
            return;
        }
        //activate only the game with index count in the games[] array
        for (int i = 0; i < games.Length; i++)
        {
            games[i].gameObject.SetActive(i == count);

        }
        //Destroy the clone of the previous game
        DestroyClones();
        //Assign the canvasTemplate as the current game
        canvasTemplate = games[count].gameObject;
        //Make clone of the current game
        InstantiateCanvas();
        //It makes the replay button interactable which was set to false previously
        replay.interactable = true;

        //Find canvas groups in the scene
        pauseScript.canvases = FindObjectsOfType<CanvasGroup>(true);

        //reset and start the score
        if (scoreScript != null)
        {
            scoreScript.resetTimer();
            scoreScript.startTimer();

        }
        Debug.Log(count);
    }

    public void next()
    {
        if (replayScript != null)
        {
            replayScript.onWin();
        }
        else
        {
            Debug.Log("ReplayScript not found");
        }
        count++;
        UpdateElements();

    }

    public void PopperDelay()
    {
        StartCoroutine(Popper(3f));
        //Makes the replay button non interactable
        replay.interactable = false;
    }

    private IEnumerator Popper(float delay)
    {
        yield return new WaitForSeconds(delay);
        float startVolume = popperScript.audioPopper.volume;
        //fades the sound of the popper
        while (popperScript.audioPopper.volume > 0)
        {
            popperScript.audioPopper.volume -= startVolume * Time.deltaTime / 1f;
            yield return null;
        }
        popperScript.audioPopper.Stop(); // Stop the audio after fading out
        popperScript.audioPopper.volume = startVolume; // Reset volume for future use
        StopAll();
        //yield return new WaitUntil(() => !(popperScript.particleAudioPairs.Any(p => p.particlesType.isPlaying)));
        next();
        Debug.Log("Enumerator worked");

    }


    public void StopAll()
    {
        //Stops all the particleAudioPairs
        foreach (var pt in popperScript.particleAudioPairs)
        {
            if (pt != null)
            {
                pt.particlesType.Stop();
                pt.particlesType.Clear();

            }
        }
    }

    private IEnumerator AudioFade()
    {
        float startVolume = popperScript.audioPopper.volume;

        while (popperScript.audioPopper.volume > 0)
        {
            popperScript.audioPopper.volume -= startVolume * Time.deltaTime / 1f;
            yield return null;
        }

        popperScript.audioPopper.Stop(); // Stop the audio after fading out
        popperScript.audioPopper.volume = startVolume; // Reset volume for future use
    }

    private void InstantiateCanvas()
    {
        // Instantiate a copy of the template as a child of its parent.
        activeCanvas = Instantiate(canvasTemplate, canvasTemplate.transform.parent);
        activeCanvas.SetActive(true);



        //Deactivates the template
        canvasTemplate.SetActive(false);
        // Add the new clone to the tracking list.
        clonesList.Add(activeCanvas);

        // Assign the UI camera to the canvas.
        AssignCameraToCanvas(activeCanvas);

        // Find the replay button in the new canvas and assign its onClick listener.
        //Button replayButton = activeCanvas.GetComponentInChildren<Button>();
        //if (replayButton != null)
        //{
        //    replayButton.onClick.RemoveAllListeners();
        //    replayButton.onClick.AddListener(ResetCanvas);
        //}
        //else
        //{
        //    Debug.LogWarning("Replay button not found in the instantiated canvas.");
        //}
    }

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

    public void DestroyClones()
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


    public void ResetCanvas()
    {
        if (activeCanvas != null)
        {
            Destroy(activeCanvas);
            clonesList.Remove(activeCanvas);
        }
        InstantiateCanvas();
        pauseScript.canvases = FindObjectsOfType<CanvasGroup>(true);
        scoreScript.resetTimer();
        pauseScript.OnPauseClose();
        scoreScript.continueTimer();




        Debug.Log("Reset pressed");

    }

    public void ResetCanvasWithoutTimerReset()
    {

        if (activeCanvas != null)
        {
            Destroy(activeCanvas);
            clonesList.Remove(activeCanvas);
        }
        InstantiateCanvas();
        pauseScript.canvases = FindObjectsOfType<CanvasGroup>(true);

    }

    public void GoHome()
    {
        if (goToPlay)
        {
            SceneManager.LoadScene("PlayGame");
            return;
        }
        string currentScene = SceneManager.GetActiveScene().name;

        foreach (var category in LevelSelectManager.categorizedScenes)
        {
            List<string> scenes = category.Value;

            if (scenes.Contains(currentScene))
            {
                LevelSelectManager.activeCanvas = category.Key;
            }


        }
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        SceneManager.LoadScene("NewestLevelSelection");
    }

    private void ChooseCanvas()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        foreach (var category in LevelSelectManager.categorizedScenes)
        {
            List<string> scenes = category.Value;

            if (scenes.Contains(currentScene))
            {
                int currentIndex = scenes.IndexOf(currentScene);
                if (currentIndex == scenes.Count - 1 && LevelSelectManager.activeCanvas + 1 < LevelSelectManager.categorizedScenes.Count)
                {
                    Debug.Log(currentScene + " is the last scene in the category: " + category.Key);

                    LevelSelectManager.activeCanvas = category.Key + 1;

                }
                else
                {
                    //Debug.Log(currentScene + " is NOT the last scene in " + category.Key + ". Next scene: " + scenes[currentIndex + 1]);
                    LevelSelectManager.activeCanvas = category.Key;
                }
                LevelSelectManager.gameCompleted[currentIndex] = true;
                Debug.Log(LevelSelectManager.gameCompleted);
                return;
            }
        }

        Debug.Log(currentScene + " is NOT found in any category.");

        //if (LevelSelectManager.FirstCanvasScenes.Contains(currentScene))
        //{
        //    LevelSelectManager.activeCanvas = 0;
        //}
        //else if (LevelSelectManager.SecondCanvasScenes.Contains(currentScene)) {
        //    LevelSelectManager.activeCanvas = 1;

        //}

    }

    public void PortraitButtonClicked()
    {

        StartCoroutine(SwitchToPortrait());
        //Screen.orientation = ScreenOrientation.Portrait;
        //SceneManager.LoadScene("Level1_Portrait");
    }

    public void LandScapeButtonClicked()
    {
        StartCoroutine(SwitchToLandscape());
        //Screen.orientation = ScreenOrientation.LandscapeLeft; // or LandscapeRight
        //SceneManager.LoadScene("Level1_LandScape");
    }

    private IEnumerator SwitchToPortrait()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        yield return new WaitForSeconds(0.5f); // Short delay to let Unity apply the orientation
        //SceneManager.LoadScene("Level1_Portrait");
    }

    private IEnumerator SwitchToLandscape()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        yield return new WaitForSeconds(0.5f); // Wait for orientation change
        //SceneManager.LoadScene("Level1_LandScape");
    }



}
