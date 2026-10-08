using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioCommon : MonoBehaviour
{
    // Start is called before the first frame update

    public static AudioCommon Instance { get; private set; }

    private Button speakerButton;

    public Slider musicSlider;
    public Slider sfxSlider;

    private Audio_Manager audioManager;
    private PauseScript pauseScript;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Makes the entire GameObject persist
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
        }

        audioManager = GetComponent<Audio_Manager>();

        SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to scene load event
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {

        //Debug.Log("Scene loaded");
        //FindPauseInScene();
        //FindSliderInScene();
        //FindSpeakerInScene();
        //if (!audioManager.isMusicMuted) {
        //    audioManager.PlayMusic("BackGroundMusic");
        //}
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe to avoid memory leaks
    }

    public void FindSliderInScene() {
        

        Slider[] sliders = Resources.FindObjectsOfTypeAll<Slider>();

        foreach (Slider slider in sliders)
        {
            if (slider.name == "volumeSlider") musicSlider = slider;
            if (slider.name == "SFXSlider") sfxSlider = slider;
        }

        if (musicSlider == null || sfxSlider == null) {
            return;
        }
        audioManager.MusicSlider = musicSlider;
        audioManager.SFXSlider = sfxSlider;

        audioManager.MusicSlider.value = PlayerPrefs.GetFloat("MusicVolume", -10);
        audioManager.setMusicVolume();
        Debug.Log("Slider found");

        musicSlider.onValueChanged.AddListener(SetMusicVolume);

        
    }

    public void FindSpeakerInScene()
    {
        //GameObject speaker = GameObject.Find("fps/speaker");
        //GameObject fps = GameObject.Find("fps"); // Find parent object
        //GameObject speaker = fps.transform.Find("speaker")?.gameObject; // Find child
        GameObject fps = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.name == "fps");
        GameObject speaker = fps?.transform.Find("speaker")?.gameObject;
        if (speaker != null)
        {
            speakerButton = speaker.GetComponent<Button>();

            if (speakerButton != null)
            {
                Debug.Log("Speaker found");
                speakerButton.onClick.RemoveAllListeners();   
                //speakerButton.onClick.AddListener(audioManager.SFXMute);
                //speakerButton.onClick.AddListener(audioManager.MusicMute);
                speakerButton.onClick.AddListener(pauseScript.MuteDisplay);
            }
            else
            {
                Debug.Log("Speaker Button not found");
            }
        }
        else
        {
            Debug.Log("Speaker object not found");
        }
    }

    public void FindPauseInScene()
    {
        //GameObject Pause = GameObject.Find("fps/pause");
        GameObject fps = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.name == "fps");
        GameObject pause = fps?.transform.Find("pause")?.gameObject;
        if (pause != null)
        {
            pauseScript = pause.GetComponent<PauseScript>();
        }

        if (pauseScript != null)
        {
            pauseScript.audioManager = this.gameObject;
            Debug.Log("Pause found");
        }
        else
        {
            Debug.Log("Pause not found");
        }



    }

    public void SetMusicVolume(float value) {
        audioManager.MusicMixer.SetFloat("volume", value);
    
    }

    public void Searcher() {
        FindPauseInScene();
        FindSliderInScene();
        FindSpeakerInScene();
        if (!audioManager.isMusicMuted)
        {
            audioManager.PlayMusic("BackGroundMusic");
        }
    }

    public void Forget() {
        speakerButton = null;
        musicSlider = null;
        sfxSlider = null;
        pauseScript = null;
    }
    //public void FindPauseInScene()
    //{
    //    GameObject fps = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.name == "fps");
    //    Transform pauseTransform = fps?.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "pause");

    //    if (pauseTransform != null)
    //    {
    //        GameObject pause = pauseTransform.gameObject;
    //        pauseScript = pause.GetComponent<PauseScript>();

    //        if (pauseScript != null)
    //        {
    //            pauseScript.audioManager = this.gameObject;
    //            Debug.Log("Pause found");
    //        }
    //    }
    //    else
    //    {
    //        Debug.Log("Pause not found");
    //    }
    //}

    //public void FindSpeakerInScene()
    //{
    //    GameObject fps = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.name == "fps");
    //    Transform speakerTransform = fps?.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "speaker");

    //    if (speakerTransform != null)
    //    {
    //        GameObject speaker = speakerTransform.gameObject;
    //        speakerButton = speaker.GetComponent<Button>();

    //        if (speakerButton != null)
    //        {
    //            Debug.Log("Speaker found");
    //            speakerButton.onClick.AddListener(audioManager.SFXMute);
    //            speakerButton.onClick.AddListener(audioManager.MusicMute);
    //            speakerButton.onClick.AddListener(pauseScript.MuteDisplay);
    //        }
    //        else
    //        {
    //            Debug.Log("Speaker Button not found");
    //        }
    //    }
    //    else
    //    {
    //        Debug.Log("Speaker object not found");
    //    }
    //}

}
