using UnityEngine;
using System;

public class SessionTracker : MonoBehaviour
{
    public static SessionTracker Instance;

    private float sessionStartTime;

    void Awake()
    {
        // Singleton check
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // destroy duplicates
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // keep alive across scenes
    }

    void Start()
    {
        sessionStartTime = Time.time;
    }

    void OnApplicationQuit()
    {
        SaveSessionTime();
    }
    void OnDisable()
    {
#if UNITY_EDITOR
        SaveSessionTime();
#endif
    }

    void OnApplicationPause(bool paused)
    {
        if (paused) SaveSessionTime();
    }

    void SaveSessionTime()
    {
        float sessionLength = Time.time - sessionStartTime;
        DateTime today = DateTime.Now.Date;
        string key = today.ToString("dd-MM-yyyy");

        float existing = PlayerPrefs.GetFloat(key, 0f);
        PlayerPrefs.SetFloat(key, existing + sessionLength);
        PlayerPrefs.Save();

        CleanupOldEntries();
    }

    void CleanupOldEntries()
    {
        DateTime today = DateTime.Now.Date;

        for (int i = 0; i < 100; i++)
        {
            DateTime day = today.AddDays(-i);
            string key = day.ToString("dd-MM-yyyy");

            if (i < 7) continue;

            if (PlayerPrefs.HasKey(key))
                PlayerPrefs.DeleteKey(key);
        }
    }

    public void DisplayData() {
        for (int i = 0; i < 7; i++)
        {
            DateTime day = DateTime.Now.Date.AddDays(-i);
            string key = day.ToString("dd-MM-yyyy");
            float duration = PlayerPrefs.GetFloat(key, 0f);

            Debug.Log($"{key}: {duration} seconds");
        }


    }
}
