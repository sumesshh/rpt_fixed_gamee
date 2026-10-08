
using UnityEngine;
using UnityEngine.UI;


public class VolumeButton : MonoBehaviour
{



    public Slider volumeSlider;
    private AndroidJavaObject audioManager;
    private int maxVolume;

    void Start()
    {
        // Get Android AudioManager
        using (AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer")
            .GetStatic<AndroidJavaObject>("currentActivity"))
        {
            audioManager = activity.Call<AndroidJavaObject>("getSystemService", "audio");
        }

        // Get max volume for media stream
        maxVolume = audioManager.Call<int>("getStreamMaxVolume", 3);

        // Set initial volume based on system volume
        int currentVolume = audioManager.Call<int>("getStreamVolume", 3);
        volumeSlider.value = (float)currentVolume / maxVolume;

        volumeSlider.onValueChanged.AddListener(ChangeSystemVolume);
    }

    void Update()
    {
        // Sync volume if the hardware button is used
        int currentVolume = audioManager.Call<int>("getStreamVolume", 3);
        volumeSlider.value = (float)currentVolume / maxVolume;
    }

    void ChangeSystemVolume(float value)
    {
        int newVolume = Mathf.RoundToInt(value * maxVolume);
        audioManager.Call("setStreamVolume", 3, newVolume, 0);
    }
}


