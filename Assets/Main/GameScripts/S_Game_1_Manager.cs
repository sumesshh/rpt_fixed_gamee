using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using TMPro;


//The below defined class represents a level select canvas (level select from 1-6 or 7-12)
[System.Serializable] // This makes it visible in the Unity Inspector
public class ButtonGroup
{
    public string groupName; //Optional. Give a name for each buttonGroup 
    public List<Button> buttons; // List of buttons in this group
}
public class S_Game_1_Manager : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject FirstPage;
    public GameObject SecondPage;

    public GameObject[] canvases;
    public GameObject loadingScreen;


    //public Button[] level1First;
    //public Button[] level1Second;
    //public Button[] level2First;

    //public int numberOfButtonGroups;
    //private Button[][] buttonGroups;

    //Always maintain order while assigning buttonGroups
    public List<ButtonGroup> buttonGroups; // User can assign in Inspector
    public TextMeshProUGUI warningText;


    void Start()
    {
        //if (LevelSelectManager.activeCanvas == 0)
        //{
        //    FirstPage.SetActive(true);
        //    SecondPage.SetActive(false);
        //}
        //else
        //{
        //    SecondPage.SetActive(true);
        //    FirstPage.SetActive(false);
        //}

        for (int i = 0; i < canvases.Length; i++) {
            canvases[i].SetActive(i == LevelSelectManager.activeCanvas);
        }

        //Adds listener to every button which comes under every buttonGroup.
        foreach (var group in buttonGroups)
        {
            foreach (var button in group.buttons)
            {
                Debug.Log("Added listeners");
                if (button != null)
                {
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => SceneLoader(button));
                    Debug.Log($"Adding listener to {button.name}");
                }
            }
        }

    }

    private void SceneLoader(Button button) {

    //The below loop aims to find the key to which the scene is attached to in the LevelSelectManager. Again, refer LevelSelectManager for more clarity.
        foreach (var group in buttonGroups)
        {
            if (group.buttons.Contains(button)) // Check if button is in this group
            {
           

                int idx = group.buttons.IndexOf(button);
                Debug.Log("Index of button: " + idx);

                // string moveCheck = PlayerPrefs.GetString("moveToGame", false.ToString());

                // if (!bool.Parse(moveCheck)) {

                //     if (idx == 0 || idx == 1 || idx == 2)
                //     {
                //         LevelSelectManager.demoCount++;
                //     }
                //     else {
                //         if (warningText != null) {
                //             StartCoroutine(PanelShift.ShortDisplay(warningText));
                //         }
                        
                //         return;
                //     }
                
                // }
                string moveCheck = PlayerPrefs.GetString("moveToGame", false.ToString());

if (!bool.Parse(moveCheck))
{
    if (idx == 0 || idx == 1 || idx == 2)
    {
        LevelSelectManager.demoCount++;
    }
    // allow idx 3 and above too
}
                
                //int categoryIdx = group.groupName;
                int categoryIdx = buttonGroups.IndexOf(group);


                //Checks if scene is present in LevelSelectManager categorizedScenes dictionary
                if (LevelSelectManager.categorizedScenes.ContainsKey(categoryIdx) && idx >= 0 && idx < LevelSelectManager.categorizedScenes[categoryIdx].Count)
                {
                    //Find the name of the scene using the categoryIdx and idx
                    string sceneName = LevelSelectManager.categorizedScenes[categoryIdx][idx];

                    LandScapeButtonClicked();
                    SceneManager.LoadScene(sceneName);
                }
           
                return; // Exit loop after finding the group
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void LandScapeButtonClicked()
    {
        StartCoroutine(SwitchToLandscape());
        //Screen.orientation = ScreenOrientation.LandscapeLeft; // or LandscapeRight
        //SceneManager.LoadScene("Level1_LandScape");
    }

    //private IEnumerator SwitchToPortrait()
    //{
    //    Screen.orientation = ScreenOrientation.Portrait;
    //    yield return new WaitForSeconds(0.5f); // Short delay to let Unity apply the orientation
    //    //SceneManager.LoadScene("Level1_Portrait");
    //}

    private IEnumerator SwitchToLandscape()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        canvases[0].SetActive(false);
        if (loadingScreen != null) {
            loadingScreen.SetActive(true);

        }
        
        yield return new WaitForSeconds(0.5f); // Wait for orientation change
        //SceneManager.LoadScene("Level1_LandScape");
    }


    public void NextButtonClick() { 
        //SecondPage.SetActive(true);
        //FirstPage.SetActive(false);

        if (LevelSelectManager.activeCanvas + 1 < canvases.Length) {
            LevelSelectManager.activeCanvas++;
            for (int i = 0; i < canvases.Length; i++)
            {
                canvases[i].SetActive(i == LevelSelectManager.activeCanvas);
            }
        }

    
    }

    public void PreviousButtonClick() {
        //FirstPage.SetActive(true);
        //SecondPage.SetActive(false);
        if (LevelSelectManager.activeCanvas - 1 >= 0) {
            LevelSelectManager.activeCanvas--;
            for (int i = 0; i < canvases.Length; i++)
            {
                canvases[i].SetActive(i == LevelSelectManager.activeCanvas);
            }

        }
    }

    //public void LoadFirstLevel()
    //{
    //    SceneManager.LoadScene("S_Level_1");
    //}

    public void LoadSecondLevel() {
        SceneManager.LoadScene("S_Level_2");
    }

    public void LoadThirdLevel() {
        SceneManager.LoadScene("M_Level_3");
    }

    public void LoadFourthLevel() {
        SceneManager.LoadScene("M_Level_4");
    }

    public void LoadFifthLevel() {
        SceneManager.LoadScene("S_Level_5");
    }

    public void LoadSixthLevel() {
        SceneManager.LoadScene("Level_6");
    }

    public void LoadSeventhLevel() {
        SceneManager.LoadScene("Level_7");
    }

    public void LoadEighthScene() {
        SceneManager.LoadScene("Level_8");
    }

    public void LoadNinthScene() {
        SceneManager.LoadScene("Level_9");
    }

    public void LoadTenthScene() {
        SceneManager.LoadScene("Level_10");
    }

    public void LoadEleventhScene()
    {
        SceneManager.LoadScene("Level_11");
    }
}
