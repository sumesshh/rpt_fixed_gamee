
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json;
using TMPro;
using UnityEngine.SceneManagement;


//using System.Diagnostics;


[System.Serializable]
public class GameData
{
    public int stars;
    public int coins;
    public int score;
}

[System.Serializable]
public class LevelData
{
    public Dictionary<string, GameData> games = new Dictionary<string, GameData>();
    public int totalStars;
    public int totalScore;
}

[System.Serializable]
public class GlobalData
{
    public int totalCoins;
    public Dictionary<string, LevelData> levels = new Dictionary<string, LevelData>();
}

public class ScoreSaver : MonoBehaviour
{
    [SerializeField] Image countDown;
    [SerializeField] TextMeshProUGUI totalCoinsText;
    [SerializeField] TextMeshProUGUI levelTotalText;
    public float totalTime = 20;
    public float elapsedTime = 20;

    public bool start = true;//by default start is on so that timer start with game if you dont want it make it false and access the bool in  another script
    private string saveFilePath;
    private GlobalData globalData;
    private string currentSceneName;
    private bool pause = false;

    private int lastNewStars;
    private int lastNewCoins;

    public int currentLevelStars;
    private static string globalSaveFilePath => Path.Combine(Application.persistentDataPath, "globalData.json");

    private void Start()
    {
        currentLevelStars = 0;
        currentSceneName = SceneManager.GetActiveScene().name;
        LoadScores();
        UpdateDisplays();
    }

    private void Update()
    {
        if (start && !pause && elapsedTime > 0)
        {
            elapsedTime -= Time.deltaTime;
            countDown.fillAmount = elapsedTime / totalTime;
            
        }
    }
    //Call this at win condition so that timer resets to full
    public void resetTimer()
    {
        elapsedTime = 20;
        totalTime = 20;
        countDown.fillAmount = 20;
        
    }

    public void pauseTimer()
    {
        pause = true;
        Debug.Log("funcpause"+start+","+pause);
    }

    public void continueTimer()
    {
        pause = false;
        Debug.Log("func countinue" + start + "," + pause);

    }

    //call this function at begining of every game to start the timer
    public void startTimer()
    {
        start = true;
       
    }

    //call this at win condition to update the results
    public void TimerStop()
    {
        start = false;
        float quarter = totalTime / 4;
        if (elapsedTime > 3 * quarter)
            ThreeStar();
        else if (elapsedTime > 2 * quarter)
            TwoStar();
        else
            OneStar();
    }

    private void SaveScores()
    {
        string json = JsonConvert.SerializeObject(globalData, Formatting.Indented);
        File.WriteAllText(globalSaveFilePath, json);
    }

    private void LoadScores()
    {
        if (File.Exists(globalSaveFilePath))
        {
            string json = File.ReadAllText(globalSaveFilePath);
            globalData = JsonConvert.DeserializeObject<GlobalData>(json);
        }
        else
        {
            globalData = new GlobalData();
        }

        // Ensure current scene exists in data
        if (!globalData.levels.ContainsKey(currentSceneName))
        {
            globalData.levels[currentSceneName] = new LevelData();
        }
    }

    private int CalculateCoinsForStars(int stars)
    {
        switch (stars)
        {
            case 3: return 30;
            case 2: return 20;
            case 1: return 10;
            default: return 0;
        }
    }

    public void UpdateScore(string gameName, int newStars, int newScore)
    {
        LevelData level = globalData.levels[currentSceneName];
        int newCoins = CalculateCoinsForStars(newStars);

        lastNewCoins = newCoins;
        lastNewStars = newStars;

        if (level.games.ContainsKey(gameName))
        {
            GameData currentGame = level.games[gameName];
            bool starsHigher = newStars > currentGame.stars;
            bool scoreHigher = newScore > currentGame.score;

            if (starsHigher)
            {
                globalData.totalCoins -= currentGame.coins;
                level.totalStars -= currentGame.stars;

                currentGame.stars = newStars;
                currentGame.coins = newCoins;

                globalData.totalCoins += newCoins;
                level.totalStars += newStars;
            }
            else if (newStars == currentGame.stars && newStars > 0)
            {
                currentGame.coins += 3;
                globalData.totalCoins += 3;
            }

            if (scoreHigher)
            {
                level.totalScore -= currentGame.score;
                currentGame.score = newScore;
                level.totalScore += newScore;
            }
        }
        else
        {
            level.games.Add(gameName, new GameData
            {
                stars = newStars,
                coins = newCoins,
                score = newScore
            });
            level.totalStars += newStars;
            globalData.totalCoins += newCoins;
            level.totalScore += newScore;
        }

        SaveScores();
        UpdateDisplays();
    }

    public (int stars, int coin) getnewcoin() {
        return (lastNewStars, lastNewCoins);
    }

    private void UpdateDisplays()
    {
        LevelData level = globalData.levels[currentSceneName];

        if (totalCoinsText != null)
        {
            totalCoinsText.text = $"{globalData.totalCoins}";
        }

        if (levelTotalText != null)
        {
            levelTotalText.text = $"Level Total\nStars: {level.totalStars}\nScore: {level.totalScore}";
        }
    }

    public (int totalStars, int totalCoins, int totalScore) GetGlobalTotals()
    {
        int stars = 0, score = 0;
        foreach (var level in globalData.levels.Values)
        {
            stars += level.totalStars;
            score += level.totalScore;
        }
        return (stars, globalData.totalCoins, score);
    }

    //private string GetCanvasName()
    //{
    //    Canvas parentCanvas = GetComponentInParent<Canvas>();
    //    return parentCanvas != null ? parentCanvas.name : null;
    //}
    string GetCanvasName()
    {
        Canvas[] canvases = FindObjectsOfType<Canvas>();

        foreach (Canvas canvas in canvases)
        {
            if (canvas.gameObject.activeSelf && !canvas.gameObject.name.ToLower().Contains("fps"))
            {
                return canvas.gameObject.name;
            }
        }


        return string.Empty; // Return empty string if no active non-FPS canvas is found
    }


    public void ThreeStar()
    {
        currentLevelStars = currentLevelStars + 3;
        string canvasName = GetCanvasName();
        if (canvasName != null)
        {
            int calculatedScore = Mathf.RoundToInt(elapsedTime * 100);
            UpdateScore(canvasName, 3, calculatedScore);
        }
    }

    public void TwoStar()
    {
        currentLevelStars = currentLevelStars + 2;
        string canvasName = GetCanvasName();
        if (canvasName != null)
        {
            int calculatedScore = Mathf.RoundToInt(elapsedTime * 100);
            UpdateScore(canvasName, 2, calculatedScore);
        }
    }

    public void OneStar()
    {
        currentLevelStars = currentLevelStars + 1;
        string canvasName = GetCanvasName();
        if (canvasName != null)
        {
            int calculatedScore = Mathf.RoundToInt(elapsedTime * 100);
            UpdateScore(canvasName, 1, calculatedScore);
        }
    }
}