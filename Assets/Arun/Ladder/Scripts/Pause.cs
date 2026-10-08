using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Audio_Manager;

public class Pause : MonoBehaviour
{

    public GameObject PAUSEpanel;
    [SerializeField] AudioSource musicSource;
    public Audio_Manager audioManager;
    public Sound[] MusicS;


   [System.Serializable]
    
    public class Sound
    {
        public string name;   // Name of the sound (for identification)
        public AudioClip clip; // The AudioClip to be played
        public bool loop;      // Whether the sound should loop or not
    }

    void Start()
    {


    }
    void Update()
    {

    }
    public void PauseMenu()
    {
        PAUSEpanel.SetActive(true);
        Time.timeScale = 0f;

        audioManager.PlaySound(MusicS[0].clip);               // calling audio Manager and playing the sound 

    }
    public void ContinueMenu()
    {
        PAUSEpanel.SetActive(false);
        Time.timeScale = 1f;
        audioManager.PlayMusic("BackGroundMusic");
    
    }
  
}
