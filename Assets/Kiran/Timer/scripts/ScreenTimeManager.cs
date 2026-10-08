using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class ScreenTimeTracker : MonoBehaviour
{
    // UI References
    [SerializeField] private TextMeshProUGUI totalTimeText;
    [SerializeField] private TextMeshProUGUI currentSceneText;
    [SerializeField] private TextMeshProUGUI allScenesText;

    private static ScreenTimeTracker instance;
    private Dictionary<string, float> sceneTimes = new Dictionary<string, float>();
    private string currentScene;
    private DateTime lastSaveDate;
    private float updateInterval = 1f;
    private float timer;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSavedData();
            CheckDateReset();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnSceneChanged;
    }

    void OnDestroy()
    {
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    void Update()
    {
        CheckDateReset();
        UpdateSceneTime();
        UpdateUI();
    }

    private void CheckDateReset()
    {
        DateTime now = DateTime.Now;
        if (lastSaveDate.Date != now.Date)
        {
            // Reset times at midnight
            sceneTimes.Clear();
            lastSaveDate = now;
            SaveData();
        }
    }

    private void UpdateSceneTime()
    {
        if (!sceneTimes.ContainsKey(currentScene))
        {
            sceneTimes[currentScene] = 0f;
        }
        sceneTimes[currentScene] += Time.deltaTime;
    }

    private void UpdateUI()
    {
        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            // Update total time
            float totalTime = 0f;
            foreach (float time in sceneTimes.Values)
            {
                totalTime += time;
            }
            totalTimeText.text = $"Total Time: {FormatTime(totalTime)}";

            // Update current scene time
            float currentSceneTime = sceneTimes.ContainsKey(currentScene) ? sceneTimes[currentScene] : 0f;
            currentSceneText.text = $"Current Scene: {currentScene}\n{FormatTime(currentSceneTime)}";

            // Update all scenes time
            string allScenes = "All Scenes:\n";
            foreach (var kvp in sceneTimes)
            {
                allScenes += $"{kvp.Key}: {FormatTime(kvp.Value)}\n";
            }
            allScenesText.text = allScenes;

            timer = 0f;
            SaveData();
        }
    }

    private void OnSceneChanged(UnityEngine.SceneManagement.Scene oldScene, UnityEngine.SceneManagement.Scene newScene)
    {
        currentScene = newScene.name;
        if (!sceneTimes.ContainsKey(currentScene))
        {
            sceneTimes[currentScene] = 0f;
        }
    }

    private string FormatTime(float timeInSeconds)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(timeInSeconds);
        if (timeSpan.Hours > 0)
        {
            return $"{timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
        }
        return $"{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
    }

    void OnApplicationQuit()
    {
        SaveData();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveData();
        }
        else
        {
            LoadSavedData();
            CheckDateReset();
        }
    }

    private void SaveData()
    {
        // Save scene times
        foreach (var kvp in sceneTimes)
        {
            PlayerPrefs.SetFloat($"SceneTime_{kvp.Key}", kvp.Value);
        }

        // Save scene names
        PlayerPrefs.SetString("SceneNames", string.Join(",", sceneTimes.Keys));

        // Save last save date
        PlayerPrefs.SetString("LastSaveDate", DateTime.Now.ToString());
        PlayerPrefs.Save();
    }

    private void LoadSavedData()
    {
        // Load last save date
        string lastSaveDateStr = PlayerPrefs.GetString("LastSaveDate", DateTime.Now.ToString());
        lastSaveDate = DateTime.Parse(lastSaveDateStr);

        // Load scene names and times
        string sceneNames = PlayerPrefs.GetString("SceneNames", "");
        if (!string.IsNullOrEmpty(sceneNames))
        {
            string[] scenes = sceneNames.Split(',');
            sceneTimes.Clear();
            foreach (string scene in scenes)
            {
                float time = PlayerPrefs.GetFloat($"SceneTime_{scene}", 0f);
                sceneTimes[scene] = time;
            }
        }
    }
} 