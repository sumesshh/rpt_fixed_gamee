using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LevelSelectManager
{
    public static int activeCanvas = 0;
    //public static List<string> FirstCanvasScenes = new List<string>
    //{
    //    "S_Level_1", "S_Level_2", "M_Level_3", "M_Level_4", "S_Level_5", "Level_6" // Add relevant scenes. Ensure this is in order
    //};
    //public static List<string> SecondCanvasScenes = new List<string>
    //{
    //    "Level_7", "Level_8" // Add relevant scenes
    //};

    //Increase length of boolean array below
    public static Dictionary<int, List<string>> categorizedScenes = new Dictionary<int, List<string>>
    {
        { 0, new List<string> { "S_Level_1", "S_Level_2", "M_Level_3", "M_Level_4", "S_Level_5" , "Level_6", "Level_7", "Level_8", "Level_9", "Level_10" } },
        //{ 1, new List<string> { "Level_7", "Level_8", "Level_9", "Level_10", "Level_11" } },
        //{ 2, new List<string> { "Game2_Level1"} }
    };

    
    public static bool[] gameCompleted = new bool[10];

    public static int demoCount = 0;

    public static bool firstLevelCompleted = false;
    public static bool secondLevelCompleted = false;


    //static LevelSelectManager() {
    //    for (int i = 0; i < gameCompleted.Length; i++) { 
    //        gameCompleted[i] = false;
    //    }
    //}
}
