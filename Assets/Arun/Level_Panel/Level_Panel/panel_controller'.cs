using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Import TextMeshPro

public class panel_controller : MonoBehaviour
{
    public TextMeshProUGUI levelCompleteText; // level completed
    public TextMeshProUGUI coinTotal;

    public GameObject oneStar, twoStars, threeStars; 

    private int starCount; // Store obtained stars (set based on game performance)
    private int coinCount;
    private int currentCoin;
    private int currentStar;

    public GameObject scoreObj;
    public GameObject managerObj;
    private ScoreSaver scoreScript;
    private PanelTestScript managerScript;


    private float avg_star; // average star
    private int numLevels;

    


    void Start()
    {
        scoreScript = scoreObj.GetComponent<ScoreSaver>();
        managerScript = managerObj.GetComponent<PanelTestScript>();
        UpdateLevelText();
        //DisplayStars();
        numLevels = managerScript.games.Length;
        Debug.Log(numLevels);
    }

    public void home()
    {
        Debug.Log("Home...");
    }

    public void restart_lvl()
    {
        Debug.Log("Restarting level...");
       
    }

    public void ReplayScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void next_lvl()
    {
        Debug.Log("Next level...");
       // LoadNextLevel();
    }


    public void LoadNextLevel()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("No more levels available.");
        }
    }

    public void Settings_btn()
    {
        Debug.Log("Settings button...");
    }

    public void Share_btn()
    {
        Debug.Log("Share button...");
    }

    void UpdateLevelText()
    {
        string currentLevelName = "Level " + (SceneManager.GetActiveScene().buildIndex + 1);
        levelCompleteText.text = currentLevelName;

    }
    void UpdateLevelCoin()
    {
        string currentLevelCoin = "+" + (SceneManager.GetActiveScene().buildIndex + 1) +"Coins";
        coinTotal.text = currentLevelCoin;

    }
    // count the total star and coins through "scoreSaver Script"
    public void CountStar()
    {
        (int currentStar, int currentCoin) = scoreScript.getnewcoin();

        starCount = starCount+currentStar;
        coinCount = coinCount+currentCoin;
    }
    // Count reset to 0 for next level

    public void CountReset ()
    {
        starCount = 0;
        coinCount = 0;  
    }
    public void findAvgStar()
    {
        avg_star= (float)starCount / numLevels;
        Debug.Log("Average star" + avg_star);
    }

    public void DisplayStars()
    {
       

        // Hide all stars initially
        oneStar.SetActive(false);
        twoStars.SetActive(false);
        threeStars.SetActive(false);
        findAvgStar();

        // Enable GameObjects based on star count
        if (avg_star <=1)
            oneStar.SetActive(true);
        else if (avg_star <=2&& avg_star>1)
            twoStars.SetActive(true);
        else if (avg_star <= 3 && avg_star>2)
            threeStars.SetActive(true);

        // showing the level in text mesh pro

        UpdateLevelText();
        UpdateLevelCoin(); 

    }
}
