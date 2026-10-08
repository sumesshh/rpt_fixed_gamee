using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadFirstLevel() {
        SceneManager.LoadScene("S_Level_1");
        LevelSelectManager.demoCount++;
    }

    public void LoadSecondLevel() {
        SceneManager.LoadScene("S_Level_2");
        LevelSelectManager.demoCount++;
    }

    public void LoadThirdLevel() {
        SceneManager.LoadScene("M_Level_3");
        LevelSelectManager.demoCount++;
    }
}
