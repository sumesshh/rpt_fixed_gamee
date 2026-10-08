using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class level_panel : MonoBehaviour
{
    public void home()
    {
        Debug.Log("Home...");
    }
    public void restart_lvl()
    {
        Debug.Log("restart lvl...");
        ReplayScene();  
    }      
    public void ReplayScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void next_lvl()
    {
        Debug.Log("next lvl...");
    }
    public void Settings_btn()
    {
        Debug.Log(" Settings btn...");

    }
    public void Share_btn()
    {
        Debug.Log("Share btn...");
    }
}
