using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine.SceneManagement;
using TMPro;

public class DailyScreenTimeTracker : MonoBehaviour
{
    public static DailyScreenTimeTracker instance { get; private set; }
    // Dictionary with the date as the key and total time (in seconds) as the value.
    private Dictionary<string, float> dailyTimes = new Dictionary<string, float>();
    private string filePath;
    public TextMeshProUGUI screenTime;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Define the file path in the persistent data directory.
        filePath = Path.Combine(Application.persistentDataPath, "dailyScreenTime.json");
        LoadData();
    }

    void Update()
    {
        // Use the current date as the key.
        string currentDate = DateTime.Now.ToString("yyyy-MM-dd");

        // If this day hasn't been recorded yet, initialize its timer.
        if (!dailyTimes.ContainsKey(currentDate))
        {
            dailyTimes[currentDate] = 0f;
        }

        // Increment the timer for the current day.
        dailyTimes[currentDate] += Time.deltaTime;
     
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveData();
        }
    }

    void OnApplicationQuit()
    {
        SaveData();
    }

 
    private void SaveData()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("{\n");
        bool first = true;
        foreach (var kvp in dailyTimes)
        {
            if (!first)
                sb.Append(",\n");
            // Ensure the float is converted using InvariantCulture (using a dot as the decimal separator).
            sb.AppendFormat("  \"{0}\": {1}", kvp.Key, kvp.Value.ToString(CultureInfo.InvariantCulture));
            first = false;
        }
        sb.Append("\n}");
        File.WriteAllText(filePath, sb.ToString());
    }

    public void LoadData()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath).Trim();
            if (json.StartsWith("{") && json.EndsWith("}"))
            {
                json = json.Substring(1, json.Length - 2); // Remove the outer braces.
                string[] entries = json.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string entry in entries)
                {
                    // Each entry should be in the format "key": value
                    string[] kv = entry.Split(new char[] { ':' }, 2);
                    if (kv.Length == 2)
                    {
                        // Remove quotes and whitespace from the key.
                        string key = kv[0].Trim().Trim('\"');
                        // Remove whitespace from the value and parse it.
                        string valueStr = kv[1].Trim();
                        if (float.TryParse(valueStr, NumberStyles.Float, CultureInfo.InvariantCulture, out float value))
                        {
                            dailyTimes[key] = value;

                            screenTime.text = dailyTimes[key].ToString();
                        }
                    }
                }
            }
        }
    }
}
